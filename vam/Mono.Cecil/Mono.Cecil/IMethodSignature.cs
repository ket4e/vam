using Mono.Collections.Generic;

namespace Mono.Cecil;

public interface IMethodSignature : IMetadataTokenProvider
{
	bool HasThis { get; set; }

	bool ExplicitThis { get; set; }

	MethodCallingConvention CallingConvention { get; set; }

	bool HasParameters { get; }

	Collection<ParameterDefinition> Parameters { get; }

	TypeReference ReturnType { get; set; }

	MethodReturnType MethodReturnType { get; }
}
