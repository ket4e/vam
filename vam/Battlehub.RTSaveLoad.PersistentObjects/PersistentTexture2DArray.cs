using System;
using ProtoBuf;

namespace Battlehub.RTSaveLoad.PersistentObjects;

[Serializable]
[ProtoContract(AsReferenceDefault = true, ImplicitFields = ImplicitFields.AllFields)]
public class PersistentTexture2DArray : PersistentTexture
{
}
