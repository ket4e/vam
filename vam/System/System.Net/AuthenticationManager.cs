using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Configuration;

namespace System.Net;

public class AuthenticationManager
{
	private static ArrayList modules;

	private static object locker = new object();

	private static ICredentialPolicy credential_policy = null;

	public static ICredentialPolicy CredentialPolicy
	{
		get
		{
			return credential_policy;
		}
		set
		{
			credential_policy = value;
		}
	}

	[System.MonoTODO]
	public static StringDictionary CustomTargetNameDictionary
	{
		get
		{
			throw GetMustImplement();
		}
	}

	public static IEnumerator RegisteredModules
	{
		get
		{
			EnsureModules();
			return modules.GetEnumerator();
		}
	}

	private AuthenticationManager()
	{
	}

	private static void EnsureModules()
	{
		lock (locker)
		{
			if (modules != null)
			{
				return;
			}
			modules = new ArrayList();
			object section = ConfigurationManager.GetSection("system.net/authenticationModules");
			if (!(section is AuthenticationModulesSection authenticationModulesSection))
			{
				return;
			}
			foreach (AuthenticationModuleElement authenticationModule in authenticationModulesSection.AuthenticationModules)
			{
				IAuthenticationModule value = null;
				try
				{
					Type type = Type.GetType(authenticationModule.Type, throwOnError: true);
					value = (IAuthenticationModule)Activator.CreateInstance(type);
				}
				catch
				{
				}
				modules.Add(value);
			}
		}
	}

	private static Exception GetMustImplement()
	{
		return new NotImplementedException();
	}

	internal static void Clear()
	{
		EnsureModules();
		lock (modules)
		{
			modules.Clear();
		}
	}

	public static Authorization Authenticate(string challenge, WebRequest request, ICredentials credentials)
	{
		if (request == null)
		{
			throw new ArgumentNullException("request");
		}
		if (credentials == null)
		{
			throw new ArgumentNullException("credentials");
		}
		if (challenge == null)
		{
			throw new ArgumentNullException("challenge");
		}
		return DoAuthenticate(challenge, request, credentials);
	}

	private static Authorization DoAuthenticate(string challenge, WebRequest request, ICredentials credentials)
	{
		EnsureModules();
		lock (modules)
		{
			foreach (IAuthenticationModule module in modules)
			{
				Authorization authorization = module.Authenticate(challenge, request, credentials);
				if (authorization == null)
				{
					continue;
				}
				authorization.Module = module;
				return authorization;
			}
		}
		return null;
	}

	public static Authorization PreAuthenticate(WebRequest request, ICredentials credentials)
	{
		if (request == null)
		{
			throw new ArgumentNullException("request");
		}
		if (credentials == null)
		{
			return null;
		}
		EnsureModules();
		lock (modules)
		{
			foreach (IAuthenticationModule module in modules)
			{
				Authorization authorization = module.PreAuthenticate(request, credentials);
				if (authorization == null)
				{
					continue;
				}
				authorization.Module = module;
				return authorization;
			}
		}
		return null;
	}

	public static void Register(IAuthenticationModule authenticationModule)
	{
		if (authenticationModule == null)
		{
			throw new ArgumentNullException("authenticationModule");
		}
		DoUnregister(authenticationModule.AuthenticationType, throwEx: false);
		lock (modules)
		{
			modules.Add(authenticationModule);
		}
	}

	public static void Unregister(IAuthenticationModule authenticationModule)
	{
		if (authenticationModule == null)
		{
			throw new ArgumentNullException("authenticationModule");
		}
		DoUnregister(authenticationModule.AuthenticationType, throwEx: true);
	}

	public static void Unregister(string authenticationScheme)
	{
		if (authenticationScheme == null)
		{
			throw new ArgumentNullException("authenticationScheme");
		}
		DoUnregister(authenticationScheme, throwEx: true);
	}

	private static void DoUnregister(string authenticationScheme, bool throwEx)
	{
		EnsureModules();
		lock (modules)
		{
			IAuthenticationModule authenticationModule = null;
			foreach (IAuthenticationModule module in modules)
			{
				string authenticationType = module.AuthenticationType;
				if (string.Compare(authenticationType, authenticationScheme, ignoreCase: true) == 0)
				{
					authenticationModule = module;
					break;
				}
			}
			if (authenticationModule == null)
			{
				if (throwEx)
				{
					throw new InvalidOperationException("Scheme not registered.");
				}
			}
			else
			{
				modules.Remove(authenticationModule);
			}
		}
	}
}
