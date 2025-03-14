using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Net.FtpClient;

public interface IFtpClient : IDisposable
{
	bool IsDisposed { get; }

	FtpIpVersion InternetProtocolVersions { get; set; }

	int SocketPollInterval { get; set; }

	bool StaleDataCheck { get; set; }

	bool IsConnected { get; }

	bool EnableThreadSafeDataConnections { get; set; }

	Encoding Encoding { get; set; }

	string Host { get; set; }

	int Port { get; set; }

	NetworkCredential Credentials { get; set; }

	int MaximumDereferenceCount { get; set; }

	X509CertificateCollection ClientCertificates { get; }

	FtpDataConnectionType DataConnectionType { get; set; }

	bool UngracefullDisconnection { get; set; }

	int ConnectTimeout { get; set; }

	int ReadTimeout { get; set; }

	int DataConnectionConnectTimeout { get; set; }

	int DataConnectionReadTimeout { get; set; }

	bool SocketKeepAlive { get; set; }

	FtpCapability Capabilities { get; }

	FtpHashAlgorithm HashAlgorithms { get; }

	FtpEncryptionMode EncryptionMode { get; set; }

	bool DataConnectionEncryption { get; set; }

	string SystemType { get; }

	event FtpSslValidation ValidateCertificate;

	bool HasFeature(FtpCapability cap);

	FtpReply Execute(string command, params object[] args);

	FtpReply Execute(string command);

	IAsyncResult BeginExecute(string command, AsyncCallback callback, object state);

	FtpReply EndExecute(IAsyncResult ar);

	void Connect();

	IAsyncResult BeginConnect(AsyncCallback callback, object state);

	void EndConnect(IAsyncResult ar);

	void Disconnect();

	IAsyncResult BeginDisconnect(AsyncCallback callback, object state);

	void EndDisconnect(IAsyncResult ar);

	Stream OpenRead(string path);

	Stream OpenRead(string path, FtpDataType type);

	Stream OpenRead(string path, long restart);

	Stream OpenRead(string path, FtpDataType type, long restart);

	IAsyncResult BeginOpenRead(string path, AsyncCallback callback, object state);

	IAsyncResult BeginOpenRead(string path, FtpDataType type, AsyncCallback callback, object state);

	IAsyncResult BeginOpenRead(string path, long restart, AsyncCallback callback, object state);

	IAsyncResult BeginOpenRead(string path, FtpDataType type, long restart, AsyncCallback callback, object state);

	Stream EndOpenRead(IAsyncResult ar);

	Stream OpenWrite(string path);

	Stream OpenWrite(string path, FtpDataType type);

	IAsyncResult BeginOpenWrite(string path, AsyncCallback callback, object state);

	IAsyncResult BeginOpenWrite(string path, FtpDataType type, AsyncCallback callback, object state);

	Stream EndOpenWrite(IAsyncResult ar);

	Stream OpenAppend(string path);

	Stream OpenAppend(string path, FtpDataType type);

	IAsyncResult BeginOpenAppend(string path, AsyncCallback callback, object state);

	IAsyncResult BeginOpenAppend(string path, FtpDataType type, AsyncCallback callback, object state);

	Stream EndOpenAppend(IAsyncResult ar);

	FtpListItem DereferenceLink(FtpListItem item);

	FtpListItem DereferenceLink(FtpListItem item, int recMax);

	IAsyncResult BeginDereferenceLink(FtpListItem item, int recMax, AsyncCallback callback, object state);

	IAsyncResult BeginDereferenceLink(FtpListItem item, AsyncCallback callback, object state);

	FtpListItem EndDereferenceLink(IAsyncResult ar);

	FtpListItem GetObjectInfo(string path);

	IAsyncResult BeginGetObjectInfo(string path, AsyncCallback callback, object state);

	FtpListItem EndGetObjectInfo(IAsyncResult ar);

	FtpListItem[] GetListing();

	FtpListItem[] GetListing(string path);

	FtpListItem[] GetListing(string path, FtpListOption options);

	IAsyncResult BeginGetListing(AsyncCallback callback, object state);

	IAsyncResult BeginGetListing(string path, AsyncCallback callback, object state);

	IAsyncResult BeginGetListing(string path, FtpListOption options, AsyncCallback callback, object state);

	FtpListItem[] EndGetListing(IAsyncResult ar);

	string[] GetNameListing();

	string[] GetNameListing(string path);

	IAsyncResult BeginGetNameListing(string path, AsyncCallback callback, object state);

	IAsyncResult BeginGetNameListing(AsyncCallback callback, object state);

	string[] EndGetNameListing(IAsyncResult ar);

	void SetWorkingDirectory(string path);

	IAsyncResult BeginSetWorkingDirectory(string path, AsyncCallback callback, object state);

	void EndSetWorkingDirectory(IAsyncResult ar);

	string GetWorkingDirectory();

	IAsyncResult BeginGetWorkingDirectory(AsyncCallback callback, object state);

	string EndGetWorkingDirectory(IAsyncResult ar);

	long GetFileSize(string path);

	IAsyncResult BeginGetFileSize(string path, AsyncCallback callback, object state);

	long EndGetFileSize(IAsyncResult ar);

	DateTime GetModifiedTime(string path);

	IAsyncResult BeginGetModifiedTime(string path, AsyncCallback callback, object state);

	DateTime EndGetModifiedTime(IAsyncResult ar);

	void DeleteFile(string path);

	IAsyncResult BeginDeleteFile(string path, AsyncCallback callback, object state);

	void EndDeleteFile(IAsyncResult ar);

	void DeleteDirectory(string path);

	void DeleteDirectory(string path, bool force);

	void DeleteDirectory(string path, bool force, FtpListOption options);

	IAsyncResult BeginDeleteDirectory(string path, AsyncCallback callback, object state);

	IAsyncResult BeginDeleteDirectory(string path, bool force, AsyncCallback callback, object state);

	IAsyncResult BeginDeleteDirectory(string path, bool force, FtpListOption options, AsyncCallback callback, object state);

	void EndDeleteDirectory(IAsyncResult ar);

	bool DirectoryExists(string path);

	IAsyncResult BeginDirectoryExists(string path, AsyncCallback callback, object state);

	bool EndDirectoryExists(IAsyncResult ar);

	bool FileExists(string path);

	bool FileExists(string path, FtpListOption options);

	IAsyncResult BeginFileExists(string path, AsyncCallback callback, object state);

	IAsyncResult BeginFileExists(string path, FtpListOption options, AsyncCallback callback, object state);

	bool EndFileExists(IAsyncResult ar);

	void CreateDirectory(string path);

	void CreateDirectory(string path, bool force);

	IAsyncResult BeginCreateDirectory(string path, AsyncCallback callback, object state);

	IAsyncResult BeginCreateDirectory(string path, bool force, AsyncCallback callback, object state);

	void EndCreateDirectory(IAsyncResult ar);

	void Rename(string path, string dest);

	IAsyncResult BeginRename(string path, string dest, AsyncCallback callback, object state);

	void EndRename(IAsyncResult ar);

	FtpHashAlgorithm GetHashAlgorithm();

	IAsyncResult BeginGetHashAlgorithm(AsyncCallback callback, object state);

	FtpHashAlgorithm EndGetHashAlgorithm(IAsyncResult ar);

	void SetHashAlgorithm(FtpHashAlgorithm type);

	IAsyncResult BeginSetHashAlgorithm(FtpHashAlgorithm type, AsyncCallback callback, object state);

	void EndSetHashAlgorithm(IAsyncResult ar);

	FtpHash GetHash(string path);

	IAsyncResult BeginGetHash(string path, AsyncCallback callback, object state);

	void EndGetHash(IAsyncResult ar);

	void DisableUTF8();
}
