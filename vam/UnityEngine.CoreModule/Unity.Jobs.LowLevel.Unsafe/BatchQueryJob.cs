using Unity.Collections;

namespace Unity.Jobs.LowLevel.Unsafe;

/// <summary>
///   <para>Struct used to implement batch query jobs.</para>
/// </summary>
public struct BatchQueryJob<CommandT, ResultT> where CommandT : struct where ResultT : struct
{
	[ReadOnly]
	internal NativeArray<CommandT> commands;

	internal NativeArray<ResultT> results;

	public BatchQueryJob(NativeArray<CommandT> commands, NativeArray<ResultT> results)
	{
		this.commands = commands;
		this.results = results;
	}
}
