using UnityEngine;

namespace Leap.Unity.Animation.Internal;

public struct TransformSelector
{
	private class TransformPositionValueInterpolator : Vector3InterpolatorBase<Transform>
	{
		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.position = _a + _b * percent;
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformPositionValueInterpolator>.Recycle(this);
		}
	}

	private class TransformPositionReferenceInterpolator : InterpolatorBase<Transform, Transform>
	{
		public override float length => Vector3.Distance(_a.position, _b.position);

		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.position = Vector3.Lerp(_a.position, _b.position, percent);
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformPositionReferenceInterpolator>.Recycle(this);
		}
	}

	private class TransformLocalPositionValueInterpolator : Vector3InterpolatorBase<Transform>
	{
		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.localPosition = _a + _b * percent;
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformLocalPositionValueInterpolator>.Recycle(this);
		}
	}

	private class TransformLocalPositionReferenceInterpolator : InterpolatorBase<Transform, Transform>
	{
		public override float length => Vector3.Distance(_a.localPosition, _b.localPosition);

		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.localPosition = Vector3.Lerp(_a.localPosition, _b.localPosition, percent);
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformLocalPositionReferenceInterpolator>.Recycle(this);
		}
	}

	private class TransformRotationValueInterpolator : QuaternionInterpolatorBase<Transform>
	{
		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.rotation = Quaternion.Slerp(_a, _b, percent);
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformRotationValueInterpolator>.Recycle(this);
		}
	}

	private class TransformRotationReferenceInterpolator : InterpolatorBase<Transform, Transform>
	{
		public override float length => Quaternion.Angle(_a.rotation, _b.rotation);

		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.rotation = Quaternion.Slerp(_a.rotation, _b.rotation, percent);
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformRotationReferenceInterpolator>.Recycle(this);
		}
	}

	private class TransformLocalRotationValueInterpolator : QuaternionInterpolatorBase<Transform>
	{
		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.localRotation = Quaternion.Slerp(_a, _b, percent);
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformLocalRotationValueInterpolator>.Recycle(this);
		}
	}

	private class TransformLocalRotationReferenceInterpolator : InterpolatorBase<Transform, Transform>
	{
		public override float length => Quaternion.Angle(_a.localRotation, _b.localRotation);

		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.localRotation = Quaternion.Slerp(_a.localRotation, _b.localRotation, percent);
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformLocalRotationReferenceInterpolator>.Recycle(this);
		}
	}

	private class TransformLocalScaleValueInterpolator : Vector3InterpolatorBase<Transform>
	{
		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.localScale = _a + _b * percent;
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformLocalScaleValueInterpolator>.Recycle(this);
		}
	}

	private class TransformLocalScaleReferenceInterpolator : InterpolatorBase<Transform, Transform>
	{
		public override float length => Quaternion.Angle(_a.localRotation, _b.localRotation);

		public override bool isValid => _target != null;

		public override void Interpolate(float percent)
		{
			_target.localScale = Vector3.Lerp(_a.localScale, _b.localScale, percent);
		}

		public override void Dispose()
		{
			_target = null;
			Pool<TransformLocalScaleReferenceInterpolator>.Recycle(this);
		}
	}

	private Transform _target;

	private Tween _tween;

	public TransformSelector(Transform target, Tween tween)
	{
		_target = target;
		_tween = tween;
	}

	public Tween Position(Vector3 a, Vector3 b)
	{
		return _tween.AddInterpolator(Pool<TransformPositionValueInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween ToPosition(Vector3 b)
	{
		return _tween.AddInterpolator(Pool<TransformPositionValueInterpolator>.Spawn().Init(_target.position, b, _target));
	}

	public Tween ByPosition(Vector3 delta)
	{
		return _tween.AddInterpolator(Pool<TransformPositionValueInterpolator>.Spawn().Init(_target.position, _target.position + delta, _target));
	}

	public Tween Position(Transform a, Transform b)
	{
		return _tween.AddInterpolator(Pool<TransformPositionReferenceInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween LocalPosition(Vector3 a, Vector3 b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalPositionValueInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween ToLocalPosition(Vector3 b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalPositionValueInterpolator>.Spawn().Init(_target.localPosition, b, _target));
	}

	public Tween ByLocalPosition(Vector3 delta)
	{
		return _tween.AddInterpolator(Pool<TransformLocalPositionValueInterpolator>.Spawn().Init(_target.localPosition, _target.localPosition + delta, _target));
	}

	public Tween LocalPosition(Transform a, Transform b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalPositionReferenceInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween Rotation(Quaternion a, Quaternion b)
	{
		return _tween.AddInterpolator(Pool<TransformRotationValueInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween ToRotation(Quaternion b)
	{
		return _tween.AddInterpolator(Pool<TransformRotationValueInterpolator>.Spawn().Init(_target.rotation, b, _target));
	}

	public Tween ByRotation(Quaternion delta)
	{
		return _tween.AddInterpolator(Pool<TransformRotationValueInterpolator>.Spawn().Init(_target.rotation, _target.rotation * delta, _target));
	}

	public Tween Rotation(Transform a, Transform b)
	{
		return _tween.AddInterpolator(Pool<TransformRotationReferenceInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween LocalRotation(Quaternion a, Quaternion b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalRotationValueInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween ToLocalRotation(Quaternion b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalRotationValueInterpolator>.Spawn().Init(_target.localRotation, b, _target));
	}

	public Tween ByLocalRotation(Quaternion delta)
	{
		return _tween.AddInterpolator(Pool<TransformLocalRotationValueInterpolator>.Spawn().Init(_target.localRotation, _target.localRotation * delta, _target));
	}

	public Tween LocalRotation(Transform a, Transform b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalRotationReferenceInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween LocalScale(Vector3 a, Vector3 b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalScaleValueInterpolator>.Spawn().Init(a, b, _target));
	}

	public Tween LocalScale(float a, float b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalScaleValueInterpolator>.Spawn().Init(Vector3.one * a, Vector3.one * b, _target));
	}

	public Tween ToLocalScale(Vector3 b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalScaleValueInterpolator>.Spawn().Init(_target.localScale, b, _target));
	}

	public Tween ToLocalScale(float b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalScaleValueInterpolator>.Spawn().Init(_target.localScale, Vector3.one * b, _target));
	}

	public Tween ByLocalScale(float b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalScaleValueInterpolator>.Spawn().Init(_target.localScale, _target.localScale * b, _target));
	}

	public Tween LocalScale(Transform a, Transform b)
	{
		return _tween.AddInterpolator(Pool<TransformLocalScaleReferenceInterpolator>.Spawn().Init(a, b, _target));
	}
}
