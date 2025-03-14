using System.Collections;

namespace System.Net;

internal class WebConnectionGroup
{
	private ServicePoint sPoint;

	private string name;

	private ArrayList connections;

	private Random rnd;

	private Queue queue;

	public string Name => name;

	internal Queue Queue => queue;

	public WebConnectionGroup(ServicePoint sPoint, string name)
	{
		this.sPoint = sPoint;
		this.name = name;
		connections = new ArrayList(1);
		queue = new Queue();
	}

	public void Close()
	{
		lock (connections)
		{
			WeakReference weakReference = null;
			int count = connections.Count;
			ArrayList arrayList = null;
			for (int i = 0; i < count; i++)
			{
				weakReference = (WeakReference)connections[i];
				if (weakReference.Target is System.Net.WebConnection webConnection)
				{
					webConnection.Close(sendNext: false);
				}
			}
			connections.Clear();
		}
	}

	public System.Net.WebConnection GetConnection(HttpWebRequest request)
	{
		System.Net.WebConnection webConnection = null;
		lock (connections)
		{
			WeakReference weakReference = null;
			int count = connections.Count;
			ArrayList arrayList = null;
			for (int i = 0; i < count; i++)
			{
				weakReference = (WeakReference)connections[i];
				webConnection = weakReference.Target as System.Net.WebConnection;
				if (webConnection == null)
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList(1);
					}
					arrayList.Add(i);
				}
			}
			if (arrayList != null)
			{
				for (int num = arrayList.Count - 1; num >= 0; num--)
				{
					connections.RemoveAt((int)arrayList[num]);
				}
			}
			return CreateOrReuseConnection(request);
		}
	}

	private static void PrepareSharingNtlm(System.Net.WebConnection cnc, HttpWebRequest request)
	{
		if (cnc.NtlmAuthenticated)
		{
			bool flag = false;
			NetworkCredential ntlmCredential = cnc.NtlmCredential;
			NetworkCredential credential = request.Credentials.GetCredential(request.RequestUri, "NTLM");
			if (ntlmCredential.Domain != credential.Domain || ntlmCredential.UserName != credential.UserName || ntlmCredential.Password != credential.Password)
			{
				flag = true;
			}
			if (!flag)
			{
				bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
				bool unsafeAuthenticatedConnectionSharing2 = cnc.UnsafeAuthenticatedConnectionSharing;
				flag = !unsafeAuthenticatedConnectionSharing || unsafeAuthenticatedConnectionSharing != unsafeAuthenticatedConnectionSharing2;
			}
			if (flag)
			{
				cnc.Close(sendNext: false);
				cnc.ResetNtlm();
			}
		}
	}

	private System.Net.WebConnection CreateOrReuseConnection(HttpWebRequest request)
	{
		int num = connections.Count;
		for (int i = 0; i < num; i++)
		{
			WeakReference weakReference = connections[i] as WeakReference;
			if (!(weakReference.Target is System.Net.WebConnection webConnection))
			{
				connections.RemoveAt(i);
				num--;
				i--;
			}
			else if (!webConnection.Busy)
			{
				PrepareSharingNtlm(webConnection, request);
				return webConnection;
			}
		}
		System.Net.WebConnection webConnection2;
		if (sPoint.ConnectionLimit > num)
		{
			webConnection2 = new System.Net.WebConnection(this, sPoint);
			connections.Add(new WeakReference(webConnection2));
			return webConnection2;
		}
		if (rnd == null)
		{
			rnd = new Random();
		}
		int index = ((num > 1) ? rnd.Next(0, num - 1) : 0);
		WeakReference weakReference2 = (WeakReference)connections[index];
		webConnection2 = weakReference2.Target as System.Net.WebConnection;
		if (webConnection2 == null)
		{
			webConnection2 = new System.Net.WebConnection(this, sPoint);
			connections.RemoveAt(index);
			connections.Add(new WeakReference(webConnection2));
		}
		return webConnection2;
	}
}
