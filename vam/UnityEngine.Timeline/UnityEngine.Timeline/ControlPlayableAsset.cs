using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

[Serializable]
[NotKeyable]
public class ControlPlayableAsset : PlayableAsset, IPropertyPreview, ITimelineClipAsset
{
	private static readonly int k_MaxRandInt = 10000;

	[SerializeField]
	public ExposedReference<GameObject> sourceGameObject;

	[SerializeField]
	public GameObject prefabGameObject;

	[SerializeField]
	public bool updateParticle = true;

	[SerializeField]
	public uint particleRandomSeed;

	[SerializeField]
	public bool updateDirector = true;

	[SerializeField]
	public bool updateITimeControl = true;

	[SerializeField]
	public bool searchHierarchy = true;

	[SerializeField]
	public bool active = true;

	[SerializeField]
	public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

	private PlayableAsset m_ControlDirectorAsset;

	private double m_Duration = PlayableBinding.DefaultDuration;

	private bool m_SupportLoop;

	private static HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();

	private static HashSet<GameObject> s_CreatedPrefabs = new HashSet<GameObject>();

	public override double duration => m_Duration;

	public ClipCaps clipCaps => ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | (m_SupportLoop ? ClipCaps.Looping : ClipCaps.None);

	public void OnEnable()
	{
		if (particleRandomSeed == 0)
		{
			particleRandomSeed = (uint)Random.Range(1, k_MaxRandInt);
		}
	}

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		if (prefabGameObject != null)
		{
			if (s_CreatedPrefabs.Contains(prefabGameObject))
			{
				Debug.LogWarningFormat("Control Track Clip ({0}) is causing a prefab to instantiate itself recursively. Aborting further instances.", base.name);
				return Playable.Create(graph);
			}
			s_CreatedPrefabs.Add(prefabGameObject);
		}
		Playable playable = Playable.Null;
		List<Playable> list = new List<Playable>();
		GameObject gameObject = sourceGameObject.Resolve(graph.GetResolver());
		if (prefabGameObject != null)
		{
			Transform parentTransform = ((!(gameObject != null)) ? null : gameObject.transform);
			ScriptPlayable<PrefabControlPlayable> scriptPlayable = PrefabControlPlayable.Create(graph, prefabGameObject, parentTransform);
			gameObject = scriptPlayable.GetBehaviour().prefabInstance;
			list.Add(scriptPlayable);
		}
		m_Duration = PlayableBinding.DefaultDuration;
		m_SupportLoop = false;
		if (gameObject != null)
		{
			IList<PlayableDirector> component = GetComponent<PlayableDirector>(gameObject);
			IList<ParticleSystem> particleSystemRoots = GetParticleSystemRoots(gameObject);
			UpdateDurationAndLoopFlag(component, particleSystemRoots);
			PlayableDirector component2 = go.GetComponent<PlayableDirector>();
			if (component2 != null)
			{
				m_ControlDirectorAsset = component2.playableAsset;
			}
			if (go == gameObject && prefabGameObject == null)
			{
				Debug.LogWarning("Control Playable (" + base.name + ") is referencing the same PlayableDirector component than the one in which it is playing.");
				active = false;
				if (!searchHierarchy)
				{
					updateDirector = false;
				}
			}
			if (active)
			{
				CreateActivationPlayable(gameObject, graph, list);
			}
			if (updateDirector)
			{
				SearchHierarchyAndConnectDirector(component, graph, list, prefabGameObject != null);
			}
			if (updateParticle)
			{
				SearchHiearchyAndConnectParticleSystem(particleSystemRoots, graph, list);
			}
			if (updateITimeControl)
			{
				SearchHierarchyAndConnectControlableScripts(GetControlableScripts(gameObject), graph, list);
			}
			playable = ConnectPlayablesToMixer(graph, list);
		}
		if (prefabGameObject != null)
		{
			s_CreatedPrefabs.Remove(prefabGameObject);
		}
		if (!playable.IsValid())
		{
			playable = Playable.Create(graph);
		}
		return playable;
	}

	private static Playable ConnectPlayablesToMixer(PlayableGraph graph, List<Playable> playables)
	{
		Playable playable = Playable.Create(graph, playables.Count);
		for (int i = 0; i != playables.Count; i++)
		{
			ConnectMixerAndPlayable(graph, playable, playables[i], i);
		}
		playable.SetPropagateSetTime(value: true);
		return playable;
	}

	private void CreateActivationPlayable(GameObject root, PlayableGraph graph, List<Playable> outplayables)
	{
		ScriptPlayable<ActivationControlPlayable> scriptPlayable = ActivationControlPlayable.Create(graph, root, postPlayback);
		if (scriptPlayable.IsValid())
		{
			outplayables.Add(scriptPlayable);
		}
	}

	private void SearchHiearchyAndConnectParticleSystem(IEnumerable<ParticleSystem> particleSystems, PlayableGraph graph, List<Playable> outplayables)
	{
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (particleSystem != null)
			{
				outplayables.Add(ParticleControlPlayable.Create(graph, particleSystem, particleRandomSeed));
			}
		}
	}

	private void SearchHierarchyAndConnectDirector(IEnumerable<PlayableDirector> directors, PlayableGraph graph, List<Playable> outplayables, bool disableSelfReferences)
	{
		foreach (PlayableDirector director in directors)
		{
			if (director != null)
			{
				if (director.playableAsset != m_ControlDirectorAsset)
				{
					outplayables.Add(DirectorControlPlayable.Create(graph, director));
				}
				else if (disableSelfReferences)
				{
					director.enabled = false;
				}
			}
		}
	}

	private static void SearchHierarchyAndConnectControlableScripts(IEnumerable<MonoBehaviour> controlableScripts, PlayableGraph graph, List<Playable> outplayables)
	{
		foreach (MonoBehaviour controlableScript in controlableScripts)
		{
			outplayables.Add(TimeControlPlayable.Create(graph, (ITimeControl)controlableScript));
		}
	}

	private static void ConnectMixerAndPlayable(PlayableGraph graph, Playable mixer, Playable playable, int portIndex)
	{
		graph.Connect(playable, 0, mixer, portIndex);
		mixer.SetInputWeight(playable, 1f);
	}

	internal IList<T> GetComponent<T>(GameObject gameObject)
	{
		List<T> list = new List<T>();
		if (gameObject != null)
		{
			if (searchHierarchy)
			{
				gameObject.GetComponentsInChildren(includeInactive: true, list);
			}
			else
			{
				gameObject.GetComponents(list);
			}
		}
		return list;
	}

	private static IEnumerable<MonoBehaviour> GetControlableScripts(GameObject root)
	{
		if (root == null)
		{
			yield break;
		}
		MonoBehaviour[] componentsInChildren = root.GetComponentsInChildren<MonoBehaviour>();
		foreach (MonoBehaviour script in componentsInChildren)
		{
			if (script is ITimeControl)
			{
				yield return script;
			}
		}
	}

	private void UpdateDurationAndLoopFlag(IList<PlayableDirector> directors, IList<ParticleSystem> particleSystems)
	{
		if (directors.Count == 1 && particleSystems.Count == 0)
		{
			PlayableDirector playableDirector = directors[0];
			if (playableDirector.playableAsset != null)
			{
				m_Duration = playableDirector.playableAsset.duration;
				m_SupportLoop = playableDirector.extrapolationMode == DirectorWrapMode.Loop;
			}
		}
		else if (particleSystems.Count == 1 && directors.Count == 0)
		{
			ParticleSystem particleSystem = particleSystems[0];
			m_Duration = particleSystem.main.duration;
			m_SupportLoop = particleSystem.main.loop;
		}
	}

	private IList<ParticleSystem> GetParticleSystemRoots(GameObject go)
	{
		if (searchHierarchy)
		{
			List<ParticleSystem> list = new List<ParticleSystem>();
			GetParticleSystemRoots(go.transform, list);
			return list;
		}
		return GetComponent<ParticleSystem>(go);
	}

	private static void GetParticleSystemRoots(Transform t, ICollection<ParticleSystem> roots)
	{
		ParticleSystem component = t.GetComponent<ParticleSystem>();
		if (component != null)
		{
			roots.Add(component);
			return;
		}
		for (int i = 0; i < t.childCount; i++)
		{
			GetParticleSystemRoots(t.GetChild(i), roots);
		}
	}

	public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		if (director == null || s_ProcessedDirectors.Contains(director))
		{
			return;
		}
		s_ProcessedDirectors.Add(director);
		GameObject gameObject = sourceGameObject.Resolve(director);
		if (gameObject != null)
		{
			if (updateParticle)
			{
				foreach (ParticleSystem particleSystemRoot in GetParticleSystemRoots(gameObject))
				{
					driver.AddFromName<ParticleSystem>(particleSystemRoot.gameObject, "randomSeed");
					driver.AddFromName<ParticleSystem>(particleSystemRoot.gameObject, "autoRandomSeed");
				}
			}
			if (active)
			{
				driver.AddFromName(gameObject, "m_IsActive");
			}
			if (updateITimeControl)
			{
				foreach (MonoBehaviour controlableScript in GetControlableScripts(gameObject))
				{
					if (controlableScript is IPropertyPreview propertyPreview)
					{
						propertyPreview.GatherProperties(director, driver);
					}
					else
					{
						driver.AddFromComponent(controlableScript.gameObject, controlableScript);
					}
				}
			}
			if (updateDirector)
			{
				foreach (PlayableDirector item in GetComponent<PlayableDirector>(gameObject))
				{
					if (!(item == null))
					{
						TimelineAsset timelineAsset = item.playableAsset as TimelineAsset;
						if (!(timelineAsset == null))
						{
							timelineAsset.GatherProperties(item, driver);
						}
					}
				}
			}
		}
		s_ProcessedDirectors.Remove(director);
	}
}
