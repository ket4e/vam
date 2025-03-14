using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Numerics;
using UnityEngine;
using System.Collections;

namespace VAB_Weight_Editor
{
    public static class Program
    {
        private static readonly StringBuilder debugReport = new StringBuilder();
        private static readonly char[] SplitChars = new char[] { ',' };
        private static readonly Regex DeformerRegex = new Regex(@"Deformer::\s*([^,}\s]+)");

        public class VabData
        {
            public string NodeId { get; set; }
            public string SceneNodeId { get; set; }
            public string GeometryId { get; set; }
            public string SceneGeometryId { get; set; }
            public List<Vector3> Vertices { get; private set; }
            public List<string> Materials { get; private set; }
            public List<int[]> PolygonIndices { get; private set; }
            public List<int[]> UVIndices { get; private set; }
            public List<Vector2> UVs { get; private set; }
            public Dictionary<string, List<VertexWeight>> BoneWeights { get; private set; }

            public VabData()
            {
                Vertices = new List<Vector3>();
                Materials = new List<string>();
                PolygonIndices = new List<int[]>();
                UVIndices = new List<int[]>();
                UVs = new List<Vector2>();
                BoneWeights = new Dictionary<string, List<VertexWeight>>();
            }
        }

        public class VertexWeight
        {
            public int VertexIndex { get; set; }
            public float Weight { get; set; }
        }

        public class FbxData
        {
            public List<Vector3> Vertices { get; private set; }
            public Dictionary<string, List<VertexWeight>> BoneWeights { get; private set; }

            public FbxData()
            {
                Vertices = new List<Vector3>();
                BoneWeights = new Dictionary<string, List<VertexWeight>>();
            }
        }

        public static void Main()
        {
            try
            {
                Console.WriteLine("VAM 骨骼权重编辑器 v1.0");
                Console.WriteLine("------------------------");

                // 读取 VAB 文件
                Console.Write("请拖入源 .vab 文件: ");
                string vabPath = Console.ReadLine();
                if (vabPath != null)
                {
                    vabPath = vabPath.Trim('"');
                }
                if (string.IsNullOrEmpty(vabPath) || !vabPath.EndsWith(".vab"))
                {
                    throw new Exception("无效的 .vab 文件路径");
                }

                // 读取 FBX 文件
                Console.Write("请拖入参考 .fbx 文件: ");
                string fbxPath = Console.ReadLine();
                if (fbxPath != null)
                {
                    fbxPath = fbxPath.Trim('"');
                }
                if (string.IsNullOrEmpty(fbxPath) || !fbxPath.EndsWith(".fbx"))
                {
                    throw new Exception("无效的 .fbx 文件路径");
                }

                // 读取并分析文件
                Console.WriteLine("\n正在读取 VAB 文件...");
                var vabData = ReadVabFile(vabPath);

                Console.WriteLine("正在读取 FBX 文件...");
                var fbxData = ReadFbxFile(fbxPath);

                // 分析权重对应关系并生成新的VAB文件
                Console.WriteLine("正在生成新的 VAB 文件...");
                string outputPath = Path.Combine(
                    Path.GetDirectoryName(vabPath) ?? "",
                    Path.GetFileNameWithoutExtension(vabPath) + "_modified.vab"
                );

                WriteModifiedVab(outputPath, vabData, fbxData);

                // 保存调试报告
                string reportPath = Path.Combine(
                    Path.GetDirectoryName(vabPath) ?? "",
                    Path.GetFileNameWithoutExtension(vabPath) + "_weight_report.txt"
                );
                File.WriteAllText(reportPath, debugReport.ToString());

                Console.WriteLine("\n处理完成!");
                Console.WriteLine("新的VAB文件已保存至: {0}", outputPath);
                Console.WriteLine("调试报告已保存至: {0}", reportPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n错误: {0}", ex.Message);
                debugReport.AppendLine("\n错误: " + ex.Message);
                debugReport.AppendLine(ex.StackTrace);

                try
                {
                    File.WriteAllText("error_report.txt", debugReport.ToString());
                    Console.WriteLine("错误报告已保存至: error_report.txt");
                }
                catch
                {
                    Console.WriteLine("无法保存错误报告");
                }
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        public static VabData ReadVabFile(string path)
        {
            var vabData = new VabData();
            debugReport.AppendLine("=== VAB 文件分析 ===");

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(stream))
                {
                    // 验证文件头
                    string header = new string(reader.ReadChars(11)); // DynamicStore
                    if (header != "DynamicStore")
                    {
                        throw new Exception("不支持的VAB文件版本: " + header);
                    }

                    string version = new string(reader.ReadChars(3)); // 1.0
                    debugReport.AppendLine("VAB版本: " + version);

                    string meshType = new string(reader.ReadChars(7)); // DAZMesh
                    if (meshType != "DAZMesh")
                    {
                        throw new Exception("无效的VAB文件格式: " + meshType);
                    }

                    string meshVersion = new string(reader.ReadChars(3)); // 1.0
                    debugReport.AppendLine("DAZMesh版本: " + meshVersion);

                    // 读取基本信息
                    vabData.NodeId = new string(reader.ReadChars(reader.ReadInt32()));
                    vabData.SceneNodeId = new string(reader.ReadChars(reader.ReadInt32()));
                    vabData.GeometryId = new string(reader.ReadChars(reader.ReadInt32()));
                    vabData.SceneGeometryId = new string(reader.ReadChars(reader.ReadInt32()));

                    debugReport.AppendLine("NodeId: " + vabData.NodeId);
                    debugReport.AppendLine("GeometryId: " + vabData.GeometryId);

                    // 读取顶点数据
                    int vertexCount = reader.ReadInt32();
                    debugReport.AppendLine("顶点数量: " + vertexCount);

                    for (int i = 0; i < vertexCount; i++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        vabData.Vertices.Add(new Vector3(x, y, z));
                    }

                    // 读取材质
                    int materialCount = reader.ReadInt32();
                    debugReport.AppendLine("材质数量: " + materialCount);

                    for (int i = 0; i < materialCount; i++)
                    {
                        int strLen = reader.ReadInt32();
                        vabData.Materials.Add(new string(reader.ReadChars(strLen)));
                    }

                    // 读取多边形索引
                    int polyCount = reader.ReadInt32();
                    debugReport.AppendLine("多边形数量: " + polyCount);

                    for (int i = 0; i < polyCount; i++)
                    {
                        int vertCount = reader.ReadInt32();
                        int[] indices = new int[vertCount];
                        for (int j = 0; j < vertCount; j++)
                        {
                            indices[j] = reader.ReadInt32();
                        }
                        vabData.PolygonIndices.Add(indices);
                    }

                    // 读取UV索引
                    for (int i = 0; i < polyCount; i++)
                    {
                        int vertCount = reader.ReadInt32();
                        int[] indices = new int[vertCount];
                        for (int j = 0; j < vertCount; j++)
                        {
                            indices[j] = reader.ReadInt32();
                        }
                        vabData.UVIndices.Add(indices);
                    }

                    // 读取UV坐标
                    int uvCount = reader.ReadInt32();
                    debugReport.AppendLine("UV坐标数量: " + uvCount);

                    for (int i = 0; i < uvCount; i++)
                    {
                        float u = reader.ReadSingle();
                        float v = reader.ReadSingle();
                        vabData.UVs.Add(new Vector2(u, v));
                    }

                    // 读取骨骼权重
                    if (stream.Position < stream.Length)
                    {
                        int boneCount = reader.ReadInt32();
                        debugReport.AppendLine("骨骼数量: " + boneCount);

                        for (int i = 0; i < boneCount; i++)
                        {
                            int nameLen = reader.ReadInt32();
                            string boneName = new string(reader.ReadChars(nameLen));
                            int weightCount = reader.ReadInt32();
                            var weights = new List<VertexWeight>();

                            for (int j = 0; j < weightCount; j++)
                            {
                                var weight = new VertexWeight
                                {
                                    VertexIndex = reader.ReadInt32(),
                                    Weight = reader.ReadSingle()
                                };
                                weights.Add(weight);
                            }

                            vabData.BoneWeights[boneName] = weights;
                            debugReport.AppendLine("骨骼 " + boneName + ": " + weightCount + " 个权重");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debugReport.AppendLine("读取VAB文件时出错: " + ex.Message);
                throw;
            }

            return vabData;
        }

        public static FbxData ReadFbxFile(string path)
        {
            var fbxData = new FbxData();
            debugReport.AppendLine("\n=== FBX 文件分析 ===");

            try
            {
                string[] lines = File.ReadAllLines(path);
                bool inGeometry = false;
                bool inVertices = false;
                bool inDeformer = false;
                string currentBone = "";
                var vertexData = new StringBuilder();

                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmedLine = lines[i].Trim();

                    // 检测Geometry部分开始
                    if (trimmedLine.StartsWith("Geometry:"))
                    {
                        inGeometry = true;
                        continue;
                    }

                    // 在Geometry部分内处理顶点数据
                    if (inGeometry)
                    {
                        // 检测顶点数组开始
                        if (trimmedLine.StartsWith("Vertices:") && !trimmedLine.Contains("PolygonVertexIndex"))
                        {
                            inVertices = true;
                            vertexData.Clear();
                            continue;
                        }

                        if (inVertices)
                        {
                            if (trimmedLine[0] == '}' || trimmedLine[0] == ']')
                            {
                                // 处理收集到的顶点数据
                                ProcessVertexData(vertexData.ToString(), fbxData);
                                inVertices = false;
                                inGeometry = false;
                                continue;
                            }

                            if (trimmedLine.StartsWith("a:"))
                            {
                                vertexData.Append(trimmedLine.Substring(2).Trim());
                                vertexData.Append(',');
                            }
                        }
                    }

                    // 处理骨骼权重数据
                    if (trimmedLine.Contains("Deformer:"))
                    {
                        var match = DeformerRegex.Match(trimmedLine);
                        if (match.Success && trimmedLine.Contains("Skin") && !trimmedLine.Contains("Cluster"))
                        {
                            currentBone = match.Groups[1].Value.Trim();
                            inDeformer = true;
                            fbxData.BoneWeights[currentBone] = new List<VertexWeight>();
                            continue;
                        }
                    }

                    if (inDeformer && !string.IsNullOrEmpty(currentBone))
                    {
                        if (trimmedLine.StartsWith("Indexes:"))
                        {
                            ProcessWeightData(lines, ref i, currentBone, fbxData);
                            inDeformer = false;
                        }
                    }
                }

                // 记录读取结果
                LogReadResults(fbxData);
            }
            catch (Exception ex)
            {
                debugReport.AppendLine("读取FBX文件时出错: " + ex.Message);
                throw;
            }

            return fbxData;
        }

        private static void ProcessVertexData(string vertexData, FbxData fbxData)
        {
            string[] allCoords = vertexData
                .Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            for (int i = 0; i < allCoords.Length; i += 3)
            {
                if (i + 2 < allCoords.Length)
                {
                    float x, y, z;
                    if (float.TryParse(allCoords[i], NumberStyles.Any, CultureInfo.InvariantCulture, out x) &&
                        float.TryParse(allCoords[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
                        float.TryParse(allCoords[i + 2], NumberStyles.Any, CultureInfo.InvariantCulture, out z))
                    {
                        fbxData.Vertices.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        private static void ProcessWeightData(string[] lines, ref int lineIndex, string boneName, FbxData fbxData)
        {
            var indices = new List<int>();
            var weights = new List<float>();
            bool readingWeights = false;

            while (++lineIndex < lines.Length)
            {
                string line = lines[lineIndex].Trim();

                if (line.StartsWith("Weights:"))
                {
                    readingWeights = true;
                    continue;
                }

                if (!line.Contains(','))
                    break;

                if (line.StartsWith("a:"))
                {
                    string data = line.Substring(2).Trim();
                    if (!readingWeights)
                    {
                        int index;
                        if (int.TryParse(data, out index))
                            indices.Add(index);
                    }
                    else
                    {
                        float weight;
                        if (float.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out weight))
                            weights.Add(weight);
                    }
                }
            }

            // 添加权重数据
            for (int i = 0; i < Math.Min(indices.Count, weights.Count); i++)
            {
                if (weights[i] > 0.001f)
                {
                    var weight = new VertexWeight
                    {
                        VertexIndex = indices[i],
                        Weight = weights[i]
                    };
                    fbxData.BoneWeights[boneName].Add(weight);
                }
            }
        }

        private static void LogReadResults(FbxData fbxData)
        {
            debugReport.AppendLine("读取到 " + fbxData.Vertices.Count + " 个顶点");
            debugReport.AppendLine("读取到 " + fbxData.BoneWeights.Count + " 个骨骼的权重数据");
            foreach (var bone in fbxData.BoneWeights)
            {
                debugReport.AppendLine("骨骼 " + bone.Key + ": " + bone.Value.Count + " 个权重");
            }
        }

        public static void WriteModifiedVab(string outputPath, VabData vabData, FbxData fbxData)
        {
            try
            {
                debugReport.AppendLine("\n=== 开始写入修改后的VAB文件 ===");

                using (var writer = new BinaryWriter(File.Create(outputPath)))
                {
                    // 写入文件头
                    writer.Write("DynamicStore".ToCharArray());
                    writer.Write("1.0".ToCharArray());
                    writer.Write("DAZMesh".ToCharArray());
                    writer.Write("1.0".ToCharArray());

                    // 写入基本信息
                    writer.Write(vabData.NodeId.Length);
                    writer.Write(vabData.NodeId.ToCharArray());
                    writer.Write(vabData.SceneNodeId.Length);
                    writer.Write(vabData.SceneNodeId.ToCharArray());
                    writer.Write(vabData.GeometryId.Length);
                    writer.Write(vabData.GeometryId.ToCharArray());
                    writer.Write(vabData.SceneGeometryId.Length);
                    writer.Write(vabData.SceneGeometryId.ToCharArray());

                    // 写入顶点数据
                    writer.Write(vabData.Vertices.Count);
                    foreach (var vertex in vabData.Vertices)
                    {
                        writer.Write(vertex.X);
                        writer.Write(vertex.Y);
                        writer.Write(vertex.Z);
                    }

                    // 写入材质数据
                    writer.Write(vabData.Materials.Count);
                    foreach (var material in vabData.Materials)
                    {
                        writer.Write(material.Length);
                        writer.Write(material.ToCharArray());
                    }

                    // 写入多边形索引
                    writer.Write(vabData.PolygonIndices.Count);
                    foreach (var indices in vabData.PolygonIndices)
                    {
                        writer.Write(indices.Length);
                        foreach (var index in indices)
                        {
                            writer.Write(index);
                        }
                    }

                    // 写入UV索引
                    foreach (var indices in vabData.UVIndices)
                    {
                        writer.Write(indices.Length);
                        foreach (var index in indices)
                        {
                            writer.Write(index);
                        }
                    }

                    // 写入UV坐标
                    writer.Write(vabData.UVs.Count);
                    foreach (var uv in vabData.UVs)
                    {
                        writer.Write(uv.X);
                        writer.Write(uv.Y);
                    }

                    // 写入骨骼权重数据
                    writer.Write(vabData.BoneWeights.Count);
                    foreach (var boneWeight in vabData.BoneWeights)
                    {
                        writer.Write(boneWeight.Key.Length);
                        writer.Write(boneWeight.Key.ToCharArray());
                        writer.Write(boneWeight.Value.Count);
                        foreach (var weight in boneWeight.Value)
                        {
                            writer.Write(weight.VertexIndex);
                            writer.Write(weight.Weight);
                        }
                    }

                    debugReport.AppendLine("VAB文件写入完成");
                }
            }
            catch (Exception ex)
            {
                debugReport.AppendLine("写入VAB文件时出错: " + ex.Message);
                throw;
            }
        }
    }

    public class DAZImportLogger : MonoBehaviour
    {
        private static ImportLogger logger;
        private bool isEnabled = true;
        
        // 配置选项
        private bool enableLogging = true;
        private string logName = "daz_import_log";
        private bool logResourceLoading = true;
        private bool logMemoryUsage = true;
        private bool logBoneMapping = true;
        private bool logWeightTransfer = true;
        
        // 监控计时器
        private float monitorInterval = 1f;
        
        // 状态跟踪
        private Dictionary<string, BoneInfo> lastBoneState;
        private Dictionary<string, WeightInfo> lastWeightState;
        
        private class BoneInfo
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public string parentName;
        }
        
        private class WeightInfo
        {
            public string boneName;
            public float weight;
            public int vertexCount;
        }
        
        void Start()
        {
            try
            {
                lastBoneState = new Dictionary<string, BoneInfo>();
                lastWeightState = new Dictionary<string, WeightInfo>();
                
                InitializeLogger();
                StartCoroutine(MonitorRoutine());
                
                Debug.Log("DAZ Import Logger initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to initialize DAZ Import Logger: " + e.ToString());
            }
        }
        
        private void InitializeLogger()
        {
            logger = new ImportLogger();
        }
        
        private IEnumerator MonitorRoutine()
        {
            while(isEnabled)
            {
                if(enableLogging)
                {
                    if(logMemoryUsage)
                    {
                        long memory = GC.GetTotalMemory(false);
                        logger.LogEvent("Memory", "Current usage: " + (memory / 1024 / 1024) + "MB");
                    }
                    
                    MonitorSceneState();
                }
                
                yield return new WaitForSeconds(monitorInterval);
            }
        }
        
        private void MonitorSceneState()
        {
            try
            {
                var gameObject = this.gameObject;
                if(gameObject != null)
                {
                    logger.LogEvent("Scene", "GameObject: " + gameObject.name);
                    
                    if(logBoneMapping)
                    {
                        MonitorSkeleton(gameObject);
                    }
                    
                    if(logWeightTransfer)
                    {
                        MonitorWeights(gameObject);
                    }
                    
                    MonitorMaterials(gameObject);
                    MonitorMeshes(gameObject);
                }
            }
            catch(Exception e)
            {
                Debug.LogError("Error monitoring scene state: " + e.ToString());
            }
        }
        
        private void MonitorSkeleton(GameObject obj)
        {
            try
            {
                var animator = obj.GetComponent<Animator>();
                if(animator != null)
                {
                    logger.LogEvent("Skeleton", "Found animator in " + obj.name);
                    
                    Transform[] bones = obj.GetComponentsInChildren<Transform>();
                    foreach(var bone in bones)
                    {
                        string boneName = bone.name;
                        BoneInfo currentInfo = new BoneInfo();
                        currentInfo.position = bone.localPosition;
                        currentInfo.rotation = bone.localRotation;
                        currentInfo.scale = bone.localScale;
                        currentInfo.parentName = bone.parent != null ? bone.parent.name : "root";

                        if(!lastBoneState.ContainsKey(boneName) || 
                           !CompareBoneInfo(lastBoneState[boneName], currentInfo))
                        {
                            string boneLog = "Name: " + boneName + "\n" +
                                "  Parent: " + currentInfo.parentName + "\n" +
                                "  Position: " + currentInfo.position + "\n" +
                                "  Rotation: " + currentInfo.rotation.eulerAngles + "\n" +
                                "  Scale: " + currentInfo.scale;
                            
                            logger.LogEvent("Bone", boneLog);
                            lastBoneState[boneName] = currentInfo;
                        }
                    }
                    
                    LogBoneHierarchy(obj.transform, 0);
                }
            }
            catch(Exception e)
            {
                logger.LogEvent("Error", "Failed to monitor skeleton: " + e.ToString());
            }
        }

        private void MonitorWeights(GameObject obj)
        {
            try
            {
                var skinned = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach(var skin in skinned)
                {
                    logger.LogEvent("Skinned", "Analyzing mesh: " + skin.name);
                    
                    var mesh = skin.sharedMesh;
                    if(mesh != null)
                    {
                        var bones = skin.bones;
                        for(int i = 0; i < bones.Length; i++)
                        {
                            var bone = bones[i];
                            if(bone != null)
                            {
                                int affectedVertices = CountAffectedVertices(mesh, i);
                                
                                WeightInfo weightInfo = new WeightInfo();
                                weightInfo.boneName = bone.name;
                                weightInfo.vertexCount = affectedVertices;

                                if(!lastWeightState.ContainsKey(bone.name) || 
                                   lastWeightState[bone.name].vertexCount != affectedVertices)
                                {
                                    string weightLog = "Bone: " + bone.name + "\n" +
                                        "  Affected vertices: " + affectedVertices + "\n" +
                                        "  Mesh: " + mesh.name;
                                    logger.LogEvent("Weight", weightLog);
                                    
                                    lastWeightState[bone.name] = weightInfo;
                                }
                            }
                        }
                        
                        LogBindPose(mesh, skin);
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogEvent("Error", "Failed to monitor weights: " + e.ToString());
            }
        }

        private void MonitorMaterials(GameObject obj)
        {
            try
            {
                var renderers = obj.GetComponentsInChildren<Renderer>();
                foreach(var renderer in renderers)
                {
                    foreach(var material in renderer.materials)
                    {
                        string matLog = "Name: " + material.name + "\n" +
                            "  Shader: " + material.shader.name;
                        logger.LogEvent("Material", matLog);
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogEvent("Error", "Failed to monitor materials: " + e.ToString());
            }
        }

        private void MonitorMeshes(GameObject obj)
        {
            try
            {
                var meshFilters = obj.GetComponentsInChildren<MeshFilter>();
                foreach(var meshFilter in meshFilters)
                {
                    if(meshFilter.sharedMesh != null)
                    {
                        string meshLog = "Name: " + meshFilter.name + "\n" +
                            "  Vertices: " + meshFilter.sharedMesh.vertexCount;
                        logger.LogEvent("Mesh", meshLog);
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogEvent("Error", "Failed to monitor meshes: " + e.ToString());
            }
        }

        private void LogBoneHierarchy(Transform bone, int depth)
        {
            string indent = new string(' ', depth * 2);
            logger.LogEvent("Hierarchy", indent + bone.name);
            
            foreach(Transform child in bone)
            {
                LogBoneHierarchy(child, depth + 1);
            }
        }

        private void LogBindPose(Mesh mesh, SkinnedMeshRenderer skin)
        {
            var bindposes = mesh.bindposes;
            for(int i = 0; i < bindposes.Length; i++)
            {
                var bone = skin.bones[i];
                if(bone != null)
                {
                    Matrix4x4 bindPose = bindposes[i];
                    string bpLog = "Bone: " + bone.name + "\n" +
                        "  Position: " + bindPose.GetColumn(3) + "\n" +
                        "  Rotation: " + Quaternion.LookRotation(
                            bindPose.GetColumn(2),
                            bindPose.GetColumn(1)
                        ).eulerAngles;
                    logger.LogEvent("BindPose", bpLog);
                }
            }
        }

        private int CountAffectedVertices(Mesh mesh, int boneIndex)
        {
            int count = 0;
            var weights = mesh.boneWeights;
            
            foreach(var weight in weights)
            {
                if(weight.boneIndex0 == boneIndex && weight.weight0 > 0) count++;
                if(weight.boneIndex1 == boneIndex && weight.weight1 > 0) count++;
                if(weight.boneIndex2 == boneIndex && weight.weight2 > 0) count++;
                if(weight.boneIndex3 == boneIndex && weight.weight3 > 0) count++;
            }
            
            return count;
        }

        private bool CompareBoneInfo(BoneInfo a, BoneInfo b)
        {
            return a.position == b.position &&
                   a.rotation == b.rotation &&
                   a.scale == b.scale &&
                   a.parentName == b.parentName;
        }

        private void ShowLogInstructions()
        {
            string instructions = 
                "To view the log:\n" +
                "1. Check Unity Console\n" +
                "2. Or check the log file in your project's log directory";
            
            Debug.Log(instructions);
        }

        private void ExportLog()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string header = "\n=== DAZ Import Log (" + timestamp + ") ===\n";
                
                Debug.Log(header);
                if(logger != null)
                {
                    logger.Flush();
                }
                Debug.Log("\n=== End of Log ===\n");
                
                logName = "Last export: " + timestamp;
                Debug.Log("Log exported to Unity Console");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to export log: " + e.ToString());
            }
        }

        private class ImportLogger
        {
            private StringBuilder buffer;
            private readonly object lockObject;
            
            public ImportLogger()
            {
                buffer = new StringBuilder();
                lockObject = new object();
            }
            
            public void LogEvent(string category, string message)
            {
                lock (lockObject)
                {
                    string entry = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] [" + 
                        category + "] " + message;
                    buffer.AppendLine(entry);
                    
                    if (buffer.Length > 1024 * 1024)
                    {
                        Flush();
                    }
                }
            }
            
            public void Flush()
            {
                lock (lockObject)
                {
                    if (buffer.Length > 0)
                    {
                        try
                        {
                            Debug.Log("DAZ Import Log:\n" + buffer.ToString());
                            buffer = new StringBuilder();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Failed to write log: " + e.ToString());
                        }
                    }
                }
            }
        }

        void OnDisable()
        {
            try
            {
                isEnabled = false;
                if(logger != null)
                {
                    logger.Flush();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error during cleanup: " + e.ToString());
            }
        }
    }
} 