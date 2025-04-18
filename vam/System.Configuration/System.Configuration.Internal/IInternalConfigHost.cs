using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Configuration.Internal;

[ComVisible(false)]
public interface IInternalConfigHost
{
	bool IsRemote { get; }

	bool SupportsChangeNotifications { get; }

	bool SupportsLocation { get; }

	bool SupportsPath { get; }

	bool SupportsRefresh { get; }

	object CreateConfigurationContext(string configPath, string locationSubPath);

	object CreateDeprecatedConfigContext(string configPath);

	string DecryptSection(string encryptedXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedSection);

	void DeleteStream(string streamName);

	string EncryptSection(string encryptedXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedSection);

	string GetConfigPathFromLocationSubPath(string configPath, string locatinSubPath);

	Type GetConfigType(string typeName, bool throwOnError);

	string GetConfigTypeName(Type t);

	void GetRestrictedPermissions(IInternalConfigRecord configRecord, out PermissionSet permissionSet, out bool isHostReady);

	string GetStreamName(string configPath);

	string GetStreamNameForConfigSource(string streamName, string configSource);

	object GetStreamVersion(string streamName);

	IDisposable Impersonate();

	void Init(IInternalConfigRoot root, params object[] hostInitParams);

	void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot root, params object[] hostInitConfigurationParams);

	bool IsAboveApplication(string configPath);

	bool IsConfigRecordRequired(string configPath);

	bool IsDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition);

	bool IsFile(string streamName);

	bool IsFullTrustSectionWithoutAptcaAllowed(IInternalConfigRecord configRecord);

	bool IsInitDelayed(IInternalConfigRecord configRecord);

	bool IsLocationApplicable(string configPath);

	bool IsSecondaryRoot(string configPath);

	bool IsTrustedConfigPath(string configPath);

	Stream OpenStreamForRead(string streamName);

	Stream OpenStreamForRead(string streamName, bool assertPermissions);

	Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext);

	Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext, bool assertPermissions);

	bool PrefetchAll(string configPath, string streamName);

	bool PrefetchSection(string sectionGroupName, string sectionName);

	void RequireCompleteInit(IInternalConfigRecord configRecord);

	object StartMonitoringStreamForChanges(string streamName, StreamChangeCallback callback);

	void StopMonitoringStreamForChanges(string streamName, StreamChangeCallback callback);

	void VerifyDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition, IConfigErrorInfo errorInfo);

	void WriteCompleted(string streamName, bool success, object writeContext);

	void WriteCompleted(string streamName, bool success, object writeContext, bool assertPermissions);
}
