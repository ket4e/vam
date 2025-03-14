using System;
using ProtoBuf;

namespace Battlehub.RTSaveLoad.PersistentObjects.UI;

[Serializable]
[ProtoContract(AsReferenceDefault = true, ImplicitFields = ImplicitFields.AllFields)]
public class PersistentPositionAsUV1 : PersistentBaseMeshEffect
{
}
