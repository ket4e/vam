using System.Collections;

namespace System.ComponentModel.Design.Serialization;

public abstract class MemberRelationshipService
{
	private class MemberRelationshipWeakEntry
	{
		private WeakReference _ownerWeakRef;

		private MemberDescriptor _member;

		public object Owner
		{
			get
			{
				if (_ownerWeakRef.IsAlive)
				{
					return _ownerWeakRef.Target;
				}
				return null;
			}
		}

		public MemberDescriptor Member => _member;

		public MemberRelationshipWeakEntry(MemberRelationship relation)
		{
			_ownerWeakRef = new WeakReference(relation.Owner);
			_member = relation.Member;
		}

		public override int GetHashCode()
		{
			if (Owner != null && _member != null)
			{
				return _member.GetHashCode() ^ _ownerWeakRef.Target.GetHashCode();
			}
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			if (o is MemberRelationshipWeakEntry)
			{
				return (MemberRelationshipWeakEntry)o == this;
			}
			return false;
		}

		public static bool operator ==(MemberRelationshipWeakEntry left, MemberRelationshipWeakEntry right)
		{
			if (left.Owner == right.Owner && left.Member == right.Member)
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(MemberRelationshipWeakEntry left, MemberRelationshipWeakEntry right)
		{
			return !(left == right);
		}
	}

	private Hashtable _relations;

	public MemberRelationship this[object owner, MemberDescriptor member]
	{
		get
		{
			return GetRelationship(new MemberRelationship(owner, member));
		}
		set
		{
			SetRelationship(new MemberRelationship(owner, member), value);
		}
	}

	public MemberRelationship this[MemberRelationship source]
	{
		get
		{
			return GetRelationship(source);
		}
		set
		{
			SetRelationship(source, value);
		}
	}

	protected MemberRelationshipService()
	{
		_relations = new Hashtable();
	}

	public abstract bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship);

	protected virtual MemberRelationship GetRelationship(MemberRelationship source)
	{
		if (source.IsEmpty)
		{
			throw new ArgumentNullException("source");
		}
		MemberRelationshipWeakEntry memberRelationshipWeakEntry = _relations[new MemberRelationshipWeakEntry(source)] as MemberRelationshipWeakEntry;
		if (memberRelationshipWeakEntry != null)
		{
			return new MemberRelationship(memberRelationshipWeakEntry.Owner, memberRelationshipWeakEntry.Member);
		}
		return MemberRelationship.Empty;
	}

	protected virtual void SetRelationship(MemberRelationship source, MemberRelationship relationship)
	{
		if (source.IsEmpty)
		{
			throw new ArgumentNullException("source");
		}
		if (!relationship.IsEmpty && !SupportsRelationship(source, relationship))
		{
			throw new ArgumentException("Relationship not supported.");
		}
		_relations[new MemberRelationshipWeakEntry(source)] = new MemberRelationshipWeakEntry(relationship);
	}
}
