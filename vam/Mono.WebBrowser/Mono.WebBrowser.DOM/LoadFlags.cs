using System;

namespace Mono.WebBrowser.DOM;

[Flags]
public enum LoadFlags : uint
{
	None = 0u,
	AsMetaRefresh = 0x10u,
	AsLinkClick = 0x20u,
	BypassHistory = 0x40u,
	ReplaceHistory = 0x80u,
	BypassLocalCache = 0x100u,
	BypassProxy = 0x200u,
	CharsetChange = 0x400u
}
