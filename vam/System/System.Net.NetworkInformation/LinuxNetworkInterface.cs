using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation;

internal class LinuxNetworkInterface : System.Net.NetworkInformation.UnixNetworkInterface
{
	private const int AF_INET = 2;

	private const int AF_INET6 = 10;

	private const int AF_PACKET = 17;

	private NetworkInterfaceType type;

	private string iface_path;

	private string iface_operstate_path;

	private string iface_flags_path;

	internal string IfacePath => iface_path;

	public override OperationalStatus OperationalStatus
	{
		get
		{
			if (!Directory.Exists(iface_path))
			{
				return OperationalStatus.Unknown;
			}
			try
			{
				switch (NetworkInterface.ReadLine(iface_operstate_path))
				{
				case "unknown":
					return OperationalStatus.Unknown;
				case "notpresent":
					return OperationalStatus.NotPresent;
				case "down":
					return OperationalStatus.Down;
				case "lowerlayerdown":
					return OperationalStatus.LowerLayerDown;
				case "testing":
					return OperationalStatus.Testing;
				case "dormant":
					return OperationalStatus.Dormant;
				case "up":
					return OperationalStatus.Up;
				}
			}
			catch
			{
			}
			return OperationalStatus.Unknown;
		}
	}

	public override bool SupportsMulticast
	{
		get
		{
			if (!Directory.Exists(iface_path))
			{
				return false;
			}
			try
			{
				string text = NetworkInterface.ReadLine(iface_flags_path);
				if (text.Length > 2 && text[0] == '0' && text[1] == 'x')
				{
					text = text.Substring(2);
				}
				ulong num = ulong.Parse(text, NumberStyles.HexNumber);
				return (num & 0x1000) == 4096;
			}
			catch
			{
				return false;
			}
		}
	}

	private LinuxNetworkInterface(string name)
		: base(name)
	{
		iface_path = "/sys/class/net/" + name + "/";
		iface_operstate_path = iface_path + "operstate";
		iface_flags_path = iface_path + "flags";
	}

	static LinuxNetworkInterface()
	{
		InitializeInterfaceAddresses();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InitializeInterfaceAddresses();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetInterfaceAddresses(out IntPtr ifap);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FreeInterfaceAddresses(IntPtr ifap);

	public static NetworkInterface[] ImplGetAllNetworkInterfaces()
	{
		Dictionary<string, System.Net.NetworkInformation.LinuxNetworkInterface> dictionary = new Dictionary<string, System.Net.NetworkInformation.LinuxNetworkInterface>();
		if (GetInterfaceAddresses(out var ifap) != 0)
		{
			throw new SystemException("getifaddrs() failed");
		}
		try
		{
			IntPtr intPtr = ifap;
			int num = 0;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.ifaddrs ifaddrs2 = (System.Net.NetworkInformation.ifaddrs)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.ifaddrs));
				IPAddress iPAddress = IPAddress.None;
				string text = ifaddrs2.ifa_name;
				int num2 = -1;
				byte[] array = null;
				NetworkInterfaceType networkInterfaceType = NetworkInterfaceType.Unknown;
				if (ifaddrs2.ifa_addr != IntPtr.Zero)
				{
					System.Net.NetworkInformation.sockaddr_in sockaddr_in7 = (System.Net.NetworkInformation.sockaddr_in)Marshal.PtrToStructure(ifaddrs2.ifa_addr, typeof(System.Net.NetworkInformation.sockaddr_in));
					if (sockaddr_in7.sin_family == 10)
					{
						System.Net.NetworkInformation.sockaddr_in6 sockaddr_in8 = (System.Net.NetworkInformation.sockaddr_in6)Marshal.PtrToStructure(ifaddrs2.ifa_addr, typeof(System.Net.NetworkInformation.sockaddr_in6));
						iPAddress = new IPAddress(sockaddr_in8.sin6_addr.u6_addr8, sockaddr_in8.sin6_scope_id);
					}
					else if (sockaddr_in7.sin_family == 2)
					{
						iPAddress = new IPAddress(sockaddr_in7.sin_addr);
					}
					else if (sockaddr_in7.sin_family == 17)
					{
						System.Net.NetworkInformation.sockaddr_ll sockaddr_ll2 = (System.Net.NetworkInformation.sockaddr_ll)Marshal.PtrToStructure(ifaddrs2.ifa_addr, typeof(System.Net.NetworkInformation.sockaddr_ll));
						if (sockaddr_ll2.sll_halen > sockaddr_ll2.sll_addr.Length)
						{
							Console.Error.WriteLine("Got a bad hardware address length for an AF_PACKET {0} {1}", sockaddr_ll2.sll_halen, sockaddr_ll2.sll_addr.Length);
							intPtr = ifaddrs2.ifa_next;
							continue;
						}
						array = new byte[sockaddr_ll2.sll_halen];
						Array.Copy(sockaddr_ll2.sll_addr, 0, array, 0, array.Length);
						num2 = sockaddr_ll2.sll_ifindex;
						int sll_hatype = sockaddr_ll2.sll_hatype;
						if (Enum.IsDefined(typeof(System.Net.NetworkInformation.LinuxArpHardware), sll_hatype))
						{
							switch ((System.Net.NetworkInformation.LinuxArpHardware)sll_hatype)
							{
							case System.Net.NetworkInformation.LinuxArpHardware.ETHER:
							case System.Net.NetworkInformation.LinuxArpHardware.EETHER:
								networkInterfaceType = NetworkInterfaceType.Ethernet;
								break;
							case System.Net.NetworkInformation.LinuxArpHardware.PRONET:
								networkInterfaceType = NetworkInterfaceType.TokenRing;
								break;
							case System.Net.NetworkInformation.LinuxArpHardware.ATM:
								networkInterfaceType = NetworkInterfaceType.Atm;
								break;
							case System.Net.NetworkInformation.LinuxArpHardware.SLIP:
								networkInterfaceType = NetworkInterfaceType.Slip;
								break;
							case System.Net.NetworkInformation.LinuxArpHardware.PPP:
								networkInterfaceType = NetworkInterfaceType.Ppp;
								break;
							case System.Net.NetworkInformation.LinuxArpHardware.LOOPBACK:
								networkInterfaceType = NetworkInterfaceType.Loopback;
								array = null;
								break;
							case System.Net.NetworkInformation.LinuxArpHardware.FDDI:
								networkInterfaceType = NetworkInterfaceType.Fddi;
								break;
							case System.Net.NetworkInformation.LinuxArpHardware.TUNNEL:
							case System.Net.NetworkInformation.LinuxArpHardware.TUNNEL6:
								networkInterfaceType = NetworkInterfaceType.Tunnel;
								break;
							}
						}
					}
				}
				System.Net.NetworkInformation.LinuxNetworkInterface value = null;
				if (string.IsNullOrEmpty(text))
				{
					int num3 = ++num;
					text = "\0" + num3;
				}
				if (!dictionary.TryGetValue(text, out value))
				{
					value = new System.Net.NetworkInformation.LinuxNetworkInterface(text);
					dictionary.Add(text, value);
				}
				if (!iPAddress.Equals(IPAddress.None))
				{
					value.AddAddress(iPAddress);
				}
				if (array != null || networkInterfaceType == NetworkInterfaceType.Loopback)
				{
					if (networkInterfaceType == NetworkInterfaceType.Ethernet && Directory.Exists(value.IfacePath + "wireless"))
					{
						networkInterfaceType = NetworkInterfaceType.Wireless80211;
					}
					value.SetLinkLayerInfo(num2, array, networkInterfaceType);
				}
				intPtr = ifaddrs2.ifa_next;
			}
		}
		finally
		{
			FreeInterfaceAddresses(ifap);
		}
		NetworkInterface[] array2 = new NetworkInterface[dictionary.Count];
		int num4 = 0;
		foreach (System.Net.NetworkInformation.LinuxNetworkInterface value2 in dictionary.Values)
		{
			array2[num4] = value2;
			num4++;
		}
		return array2;
	}

	public override IPInterfaceProperties GetIPProperties()
	{
		if (ipproperties == null)
		{
			ipproperties = new System.Net.NetworkInformation.LinuxIPInterfaceProperties(this, addresses);
		}
		return ipproperties;
	}

	public override IPv4InterfaceStatistics GetIPv4Statistics()
	{
		if (ipv4stats == null)
		{
			ipv4stats = new System.Net.NetworkInformation.LinuxIPv4InterfaceStatistics(this);
		}
		return ipv4stats;
	}
}
