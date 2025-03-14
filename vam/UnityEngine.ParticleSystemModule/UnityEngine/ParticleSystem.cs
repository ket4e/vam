using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Script interface for particle systems (Shuriken).</para>
/// </summary>
[RequireComponent(typeof(Transform))]
[UsedByNativeCode]
public sealed class ParticleSystem : Component
{
	/// <summary>
	///   <para>Script interface for a Burst.</para>
	/// </summary>
	public struct Burst
	{
		private float m_Time;

		private MinMaxCurve m_Count;

		private int m_RepeatCount;

		private float m_RepeatInterval;

		/// <summary>
		///   <para>The time that each burst occurs.</para>
		/// </summary>
		public float time
		{
			get
			{
				return m_Time;
			}
			set
			{
				m_Time = value;
			}
		}

		/// <summary>
		///   <para>Number of particles to be emitted.</para>
		/// </summary>
		public MinMaxCurve count
		{
			get
			{
				return m_Count;
			}
			set
			{
				m_Count = value;
			}
		}

		/// <summary>
		///   <para>Minimum number of particles to be emitted.</para>
		/// </summary>
		public short minCount
		{
			get
			{
				return (short)m_Count.constantMin;
			}
			set
			{
				m_Count.constantMin = value;
			}
		}

		/// <summary>
		///   <para>Maximum number of particles to be emitted.</para>
		/// </summary>
		public short maxCount
		{
			get
			{
				return (short)m_Count.constantMax;
			}
			set
			{
				m_Count.constantMax = value;
			}
		}

		/// <summary>
		///   <para>How many times to play the burst. (0 means infinitely).</para>
		/// </summary>
		public int cycleCount
		{
			get
			{
				return m_RepeatCount + 1;
			}
			set
			{
				m_RepeatCount = value - 1;
			}
		}

		/// <summary>
		///   <para>How often to repeat the burst, in seconds.</para>
		/// </summary>
		public float repeatInterval
		{
			get
			{
				return m_RepeatInterval;
			}
			set
			{
				m_RepeatInterval = value;
			}
		}

		/// <summary>
		///   <para>Construct a new Burst with a time and count.</para>
		/// </summary>
		/// <param name="_time">Time to emit the burst.</param>
		/// <param name="_minCount">Minimum number of particles to emit.</param>
		/// <param name="_maxCount">Maximum number of particles to emit.</param>
		/// <param name="_count">Number of particles to emit.</param>
		/// <param name="_cycleCount">Number of times to play the burst. (0 means indefinitely).</param>
		/// <param name="_repeatInterval">How often to repeat the burst, in seconds.</param>
		public Burst(float _time, short _count)
		{
			m_Time = _time;
			m_Count = _count;
			m_RepeatCount = 0;
			m_RepeatInterval = 0f;
		}

		/// <summary>
		///   <para>Construct a new Burst with a time and count.</para>
		/// </summary>
		/// <param name="_time">Time to emit the burst.</param>
		/// <param name="_minCount">Minimum number of particles to emit.</param>
		/// <param name="_maxCount">Maximum number of particles to emit.</param>
		/// <param name="_count">Number of particles to emit.</param>
		/// <param name="_cycleCount">Number of times to play the burst. (0 means indefinitely).</param>
		/// <param name="_repeatInterval">How often to repeat the burst, in seconds.</param>
		public Burst(float _time, short _minCount, short _maxCount)
		{
			m_Time = _time;
			m_Count = new MinMaxCurve(_minCount, _maxCount);
			m_RepeatCount = 0;
			m_RepeatInterval = 0f;
		}

		/// <summary>
		///   <para>Construct a new Burst with a time and count.</para>
		/// </summary>
		/// <param name="_time">Time to emit the burst.</param>
		/// <param name="_minCount">Minimum number of particles to emit.</param>
		/// <param name="_maxCount">Maximum number of particles to emit.</param>
		/// <param name="_count">Number of particles to emit.</param>
		/// <param name="_cycleCount">Number of times to play the burst. (0 means indefinitely).</param>
		/// <param name="_repeatInterval">How often to repeat the burst, in seconds.</param>
		public Burst(float _time, short _minCount, short _maxCount, int _cycleCount, float _repeatInterval)
		{
			m_Time = _time;
			m_Count = new MinMaxCurve(_minCount, _maxCount);
			m_RepeatCount = _cycleCount - 1;
			m_RepeatInterval = _repeatInterval;
		}

		public Burst(float _time, MinMaxCurve _count)
		{
			m_Time = _time;
			m_Count = _count;
			m_RepeatCount = 0;
			m_RepeatInterval = 0f;
		}

		public Burst(float _time, MinMaxCurve _count, int _cycleCount, float _repeatInterval)
		{
			m_Time = _time;
			m_Count = _count;
			m_RepeatCount = _cycleCount - 1;
			m_RepeatInterval = _repeatInterval;
		}
	}

	/// <summary>
	///   <para>Script interface for a Min-Max Curve.</para>
	/// </summary>
	public struct MinMaxCurve
	{
		private ParticleSystemCurveMode m_Mode;

		private float m_CurveMultiplier;

		private AnimationCurve m_CurveMin;

		private AnimationCurve m_CurveMax;

		private float m_ConstantMin;

		private float m_ConstantMax;

		/// <summary>
		///   <para>Set the mode that the min-max curve will use to evaluate values.</para>
		/// </summary>
		public ParticleSystemCurveMode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
			}
		}

		/// <summary>
		///   <para>Set a multiplier to be applied to the curves.</para>
		/// </summary>
		public float curveMultiplier
		{
			get
			{
				return m_CurveMultiplier;
			}
			set
			{
				m_CurveMultiplier = value;
			}
		}

		/// <summary>
		///   <para>Set a curve for the upper bound.</para>
		/// </summary>
		public AnimationCurve curveMax
		{
			get
			{
				return m_CurveMax;
			}
			set
			{
				m_CurveMax = value;
			}
		}

		/// <summary>
		///   <para>Set a curve for the lower bound.</para>
		/// </summary>
		public AnimationCurve curveMin
		{
			get
			{
				return m_CurveMin;
			}
			set
			{
				m_CurveMin = value;
			}
		}

		/// <summary>
		///   <para>Set a constant for the upper bound.</para>
		/// </summary>
		public float constantMax
		{
			get
			{
				return m_ConstantMax;
			}
			set
			{
				m_ConstantMax = value;
			}
		}

		/// <summary>
		///   <para>Set a constant for the lower bound.</para>
		/// </summary>
		public float constantMin
		{
			get
			{
				return m_ConstantMin;
			}
			set
			{
				m_ConstantMin = value;
			}
		}

		/// <summary>
		///   <para>Set the constant value.</para>
		/// </summary>
		public float constant
		{
			get
			{
				return m_ConstantMax;
			}
			set
			{
				m_ConstantMax = value;
			}
		}

		/// <summary>
		///   <para>Set the curve.</para>
		/// </summary>
		public AnimationCurve curve
		{
			get
			{
				return m_CurveMax;
			}
			set
			{
				m_CurveMax = value;
			}
		}

		/// <summary>
		///   <para>A single constant value for the entire curve.</para>
		/// </summary>
		/// <param name="constant">Constant value.</param>
		public MinMaxCurve(float constant)
		{
			m_Mode = ParticleSystemCurveMode.Constant;
			m_CurveMultiplier = 0f;
			m_CurveMin = null;
			m_CurveMax = null;
			m_ConstantMin = 0f;
			m_ConstantMax = constant;
		}

		/// <summary>
		///   <para>Use one curve when evaluating numbers along this Min-Max curve.</para>
		/// </summary>
		/// <param name="scalar">A multiplier to be applied to the curve.</param>
		/// <param name="curve">A single curve for evaluating against.</param>
		/// <param name="multiplier"></param>
		public MinMaxCurve(float multiplier, AnimationCurve curve)
		{
			m_Mode = ParticleSystemCurveMode.Curve;
			m_CurveMultiplier = multiplier;
			m_CurveMin = null;
			m_CurveMax = curve;
			m_ConstantMin = 0f;
			m_ConstantMax = 0f;
		}

		/// <summary>
		///   <para>Randomly select values based on the interval between the minimum and maximum curves.</para>
		/// </summary>
		/// <param name="scalar">A multiplier to be applied to the curves.</param>
		/// <param name="min">The curve describing the minimum values to be evaluated.</param>
		/// <param name="max">The curve describing the maximum values to be evaluated.</param>
		/// <param name="multiplier"></param>
		public MinMaxCurve(float multiplier, AnimationCurve min, AnimationCurve max)
		{
			m_Mode = ParticleSystemCurveMode.TwoCurves;
			m_CurveMultiplier = multiplier;
			m_CurveMin = min;
			m_CurveMax = max;
			m_ConstantMin = 0f;
			m_ConstantMax = 0f;
		}

		/// <summary>
		///   <para>Randomly select values based on the interval between the minimum and maximum constants.</para>
		/// </summary>
		/// <param name="min">The constant describing the minimum values to be evaluated.</param>
		/// <param name="max">The constant describing the maximum values to be evaluated.</param>
		public MinMaxCurve(float min, float max)
		{
			m_Mode = ParticleSystemCurveMode.TwoConstants;
			m_CurveMultiplier = 0f;
			m_CurveMin = null;
			m_CurveMax = null;
			m_ConstantMin = min;
			m_ConstantMax = max;
		}

		/// <summary>
		///   <para>Manually query the curve to calculate values based on what mode it is in.</para>
		/// </summary>
		/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the curve. This is valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.Curve or ParticleSystemCurveMode.TwoCurves.</param>
		/// <param name="lerpFactor">Blend between the 2 curves/constants (Valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.TwoConstants or ParticleSystemCurveMode.TwoCurves).</param>
		/// <returns>
		///   <para>Calculated curve/constant value.</para>
		/// </returns>
		public float Evaluate(float time)
		{
			return Evaluate(time, 1f);
		}

		/// <summary>
		///   <para>Manually query the curve to calculate values based on what mode it is in.</para>
		/// </summary>
		/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the curve. This is valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.Curve or ParticleSystemCurveMode.TwoCurves.</param>
		/// <param name="lerpFactor">Blend between the 2 curves/constants (Valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.TwoConstants or ParticleSystemCurveMode.TwoCurves).</param>
		/// <returns>
		///   <para>Calculated curve/constant value.</para>
		/// </returns>
		public float Evaluate(float time, float lerpFactor)
		{
			time = Mathf.Clamp(time, 0f, 1f);
			lerpFactor = Mathf.Clamp(lerpFactor, 0f, 1f);
			if (m_Mode == ParticleSystemCurveMode.Constant)
			{
				return m_ConstantMax;
			}
			if (m_Mode == ParticleSystemCurveMode.TwoConstants)
			{
				return Mathf.Lerp(m_ConstantMin, m_ConstantMax, lerpFactor);
			}
			float num = m_CurveMax.Evaluate(time) * m_CurveMultiplier;
			if (m_Mode == ParticleSystemCurveMode.TwoCurves)
			{
				return Mathf.Lerp(m_CurveMin.Evaluate(time) * m_CurveMultiplier, num, lerpFactor);
			}
			return num;
		}

		public static implicit operator MinMaxCurve(float constant)
		{
			return new MinMaxCurve(constant);
		}
	}

	/// <summary>
	///   <para>MinMaxGradient contains two Gradients, and returns a Color based on ParticleSystem.MinMaxGradient.mode. Depending on the mode, the Color returned may be randomized.
	/// Gradients are edited via the ParticleSystem Inspector once a ParticleSystemGradientMode requiring them has been selected. Some modes do not require gradients, only colors.</para>
	/// </summary>
	public struct MinMaxGradient
	{
		private ParticleSystemGradientMode m_Mode;

		private Gradient m_GradientMin;

		private Gradient m_GradientMax;

		private Color m_ColorMin;

		private Color m_ColorMax;

		/// <summary>
		///   <para>Set the mode that the min-max gradient will use to evaluate colors.</para>
		/// </summary>
		public ParticleSystemGradientMode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
			}
		}

		/// <summary>
		///   <para>Set a gradient for the upper bound.</para>
		/// </summary>
		public Gradient gradientMax
		{
			get
			{
				return m_GradientMax;
			}
			set
			{
				m_GradientMax = value;
			}
		}

		/// <summary>
		///   <para>Set a gradient for the lower bound.</para>
		/// </summary>
		public Gradient gradientMin
		{
			get
			{
				return m_GradientMin;
			}
			set
			{
				m_GradientMin = value;
			}
		}

		/// <summary>
		///   <para>Set a constant color for the upper bound.</para>
		/// </summary>
		public Color colorMax
		{
			get
			{
				return m_ColorMax;
			}
			set
			{
				m_ColorMax = value;
			}
		}

		/// <summary>
		///   <para>Set a constant color for the lower bound.</para>
		/// </summary>
		public Color colorMin
		{
			get
			{
				return m_ColorMin;
			}
			set
			{
				m_ColorMin = value;
			}
		}

		/// <summary>
		///   <para>Set a constant color.</para>
		/// </summary>
		public Color color
		{
			get
			{
				return m_ColorMax;
			}
			set
			{
				m_ColorMax = value;
			}
		}

		/// <summary>
		///   <para>Set the gradient.</para>
		/// </summary>
		public Gradient gradient
		{
			get
			{
				return m_GradientMax;
			}
			set
			{
				m_GradientMax = value;
			}
		}

		/// <summary>
		///   <para>A single constant color for the entire gradient.</para>
		/// </summary>
		/// <param name="color">Constant color.</param>
		public MinMaxGradient(Color color)
		{
			m_Mode = ParticleSystemGradientMode.Color;
			m_GradientMin = null;
			m_GradientMax = null;
			m_ColorMin = Color.black;
			m_ColorMax = color;
		}

		/// <summary>
		///   <para>Use one gradient when evaluating numbers along this Min-Max gradient.</para>
		/// </summary>
		/// <param name="gradient">A single gradient for evaluating against.</param>
		public MinMaxGradient(Gradient gradient)
		{
			m_Mode = ParticleSystemGradientMode.Gradient;
			m_GradientMin = null;
			m_GradientMax = gradient;
			m_ColorMin = Color.black;
			m_ColorMax = Color.black;
		}

		/// <summary>
		///   <para>Randomly select colors based on the interval between the minimum and maximum constants.</para>
		/// </summary>
		/// <param name="min">The constant color describing the minimum colors to be evaluated.</param>
		/// <param name="max">The constant color describing the maximum colors to be evaluated.</param>
		public MinMaxGradient(Color min, Color max)
		{
			m_Mode = ParticleSystemGradientMode.TwoColors;
			m_GradientMin = null;
			m_GradientMax = null;
			m_ColorMin = min;
			m_ColorMax = max;
		}

		/// <summary>
		///   <para>Randomly select colors based on the interval between the minimum and maximum gradients.</para>
		/// </summary>
		/// <param name="min">The gradient describing the minimum colors to be evaluated.</param>
		/// <param name="max">The gradient describing the maximum colors to be evaluated.</param>
		public MinMaxGradient(Gradient min, Gradient max)
		{
			m_Mode = ParticleSystemGradientMode.TwoGradients;
			m_GradientMin = min;
			m_GradientMax = max;
			m_ColorMin = Color.black;
			m_ColorMax = Color.black;
		}

		/// <summary>
		///   <para>Manually query the gradient to calculate colors based on what mode it is in.</para>
		/// </summary>
		/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the gradient. This is valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.Gradient or ParticleSystemGradientMode.TwoGradients.</param>
		/// <param name="lerpFactor">Blend between the 2 gradients/colors (Valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.TwoColors or ParticleSystemGradientMode.TwoGradients).</param>
		/// <returns>
		///   <para>Calculated gradient/color value.</para>
		/// </returns>
		public Color Evaluate(float time)
		{
			return Evaluate(time, 1f);
		}

		/// <summary>
		///   <para>Manually query the gradient to calculate colors based on what mode it is in.</para>
		/// </summary>
		/// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the gradient. This is valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.Gradient or ParticleSystemGradientMode.TwoGradients.</param>
		/// <param name="lerpFactor">Blend between the 2 gradients/colors (Valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.TwoColors or ParticleSystemGradientMode.TwoGradients).</param>
		/// <returns>
		///   <para>Calculated gradient/color value.</para>
		/// </returns>
		public Color Evaluate(float time, float lerpFactor)
		{
			time = Mathf.Clamp(time, 0f, 1f);
			lerpFactor = Mathf.Clamp(lerpFactor, 0f, 1f);
			if (m_Mode == ParticleSystemGradientMode.Color)
			{
				return m_ColorMax;
			}
			if (m_Mode == ParticleSystemGradientMode.TwoColors)
			{
				return Color.Lerp(m_ColorMin, m_ColorMax, lerpFactor);
			}
			Color color = m_GradientMax.Evaluate(time);
			if (m_Mode == ParticleSystemGradientMode.TwoGradients)
			{
				return Color.Lerp(m_GradientMin.Evaluate(time), color, lerpFactor);
			}
			return color;
		}

		public static implicit operator MinMaxGradient(Color color)
		{
			return new MinMaxGradient(color);
		}

		public static implicit operator MinMaxGradient(Gradient gradient)
		{
			return new MinMaxGradient(gradient);
		}
	}

	/// <summary>
	///   <para>Script interface for the main module.</para>
	/// </summary>
	public struct MainModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>The duration of the particle system in seconds.</para>
		/// </summary>
		public float duration
		{
			get
			{
				return GetDuration(m_ParticleSystem);
			}
			set
			{
				SetDuration(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Is the particle system looping?</para>
		/// </summary>
		public bool loop
		{
			get
			{
				return GetLoop(m_ParticleSystem);
			}
			set
			{
				SetLoop(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When looping is enabled, this controls whether this particle system will look like it has already simulated for one loop when first becoming visible.</para>
		/// </summary>
		public bool prewarm
		{
			get
			{
				return GetPrewarm(m_ParticleSystem);
			}
			set
			{
				SetPrewarm(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Start delay in seconds.</para>
		/// </summary>
		public MinMaxCurve startDelay
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartDelay(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartDelay(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start delay multiplier in seconds.</para>
		/// </summary>
		public float startDelayMultiplier
		{
			get
			{
				return GetStartDelayMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartDelayMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The total lifetime in seconds that each new particle will have.</para>
		/// </summary>
		public MinMaxCurve startLifetime
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartLifetime(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartLifetime(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start lifetime multiplier.</para>
		/// </summary>
		public float startLifetimeMultiplier
		{
			get
			{
				return GetStartLifetimeMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartLifetimeMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial speed of particles when emitted.</para>
		/// </summary>
		public MinMaxCurve startSpeed
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartSpeed(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartSpeed(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>A multiplier of the initial speed of particles when emitted.</para>
		/// </summary>
		public float startSpeedMultiplier
		{
			get
			{
				return GetStartSpeedMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartSpeedMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>A flag to enable specifying particle size individually for each axis.</para>
		/// </summary>
		public bool startSize3D
		{
			get
			{
				return GetStartSize3D(m_ParticleSystem);
			}
			set
			{
				SetStartSize3D(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial size of particles when emitted.</para>
		/// </summary>
		public MinMaxCurve startSize
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartSizeX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartSizeX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start size multiplier.</para>
		/// </summary>
		public float startSizeMultiplier
		{
			get
			{
				return GetStartSizeXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartSizeXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial size of particles along the X axis when emitted.</para>
		/// </summary>
		public MinMaxCurve startSizeX
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartSizeX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartSizeX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start rotation multiplier along the X axis.</para>
		/// </summary>
		public float startSizeXMultiplier
		{
			get
			{
				return GetStartSizeXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartSizeXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial size of particles along the Y axis when emitted.</para>
		/// </summary>
		public MinMaxCurve startSizeY
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartSizeY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartSizeY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start rotation multiplier along the Y axis.</para>
		/// </summary>
		public float startSizeYMultiplier
		{
			get
			{
				return GetStartSizeYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartSizeYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial size of particles along the Z axis when emitted.</para>
		/// </summary>
		public MinMaxCurve startSizeZ
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartSizeZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartSizeZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start rotation multiplier along the Z axis.</para>
		/// </summary>
		public float startSizeZMultiplier
		{
			get
			{
				return GetStartSizeZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartSizeZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>A flag to enable 3D particle rotation.</para>
		/// </summary>
		public bool startRotation3D
		{
			get
			{
				return GetStartRotation3D(m_ParticleSystem);
			}
			set
			{
				SetStartRotation3D(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial rotation of particles when emitted.</para>
		/// </summary>
		public MinMaxCurve startRotation
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartRotationZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartRotationZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start rotation multiplier.</para>
		/// </summary>
		public float startRotationMultiplier
		{
			get
			{
				return GetStartRotationZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartRotationZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial rotation of particles around the X axis when emitted.</para>
		/// </summary>
		public MinMaxCurve startRotationX
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartRotationX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartRotationX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start rotation multiplier around the X axis.</para>
		/// </summary>
		public float startRotationXMultiplier
		{
			get
			{
				return GetStartRotationXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartRotationXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial rotation of particles around the Y axis when emitted.</para>
		/// </summary>
		public MinMaxCurve startRotationY
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartRotationY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartRotationY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start rotation multiplier around the Y axis.</para>
		/// </summary>
		public float startRotationYMultiplier
		{
			get
			{
				return GetStartRotationYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartRotationYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial rotation of particles around the Z axis when emitted.</para>
		/// </summary>
		public MinMaxCurve startRotationZ
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartRotationZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartRotationZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Start rotation multiplier around the Z axis.</para>
		/// </summary>
		public float startRotationZMultiplier
		{
			get
			{
				return GetStartRotationZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartRotationZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Cause some particles to spin in  the opposite direction.</para>
		/// </summary>
		public float flipRotation
		{
			get
			{
				return GetFlipRotation(m_ParticleSystem);
			}
			set
			{
				SetFlipRotation(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The initial color of particles when emitted.</para>
		/// </summary>
		public MinMaxGradient startColor
		{
			get
			{
				MinMaxGradient gradient = default(MinMaxGradient);
				GetStartColor(m_ParticleSystem, ref gradient);
				return gradient;
			}
			set
			{
				SetStartColor(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Scale applied to the gravity, defined by Physics.gravity.</para>
		/// </summary>
		public MinMaxCurve gravityModifier
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetGravityModifier(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetGravityModifier(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the gravity mulutiplier.</para>
		/// </summary>
		public float gravityModifierMultiplier
		{
			get
			{
				return GetGravityModifierMultiplier(m_ParticleSystem);
			}
			set
			{
				SetGravityModifierMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>This selects the space in which to simulate particles. It can be either world or local space.</para>
		/// </summary>
		public ParticleSystemSimulationSpace simulationSpace
		{
			get
			{
				return GetSimulationSpace(m_ParticleSystem);
			}
			set
			{
				SetSimulationSpace(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Simulate particles relative to a custom transform component.</para>
		/// </summary>
		public Transform customSimulationSpace
		{
			get
			{
				return GetCustomSimulationSpace(m_ParticleSystem);
			}
			set
			{
				SetCustomSimulationSpace(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Override the default playback speed of the Particle System.</para>
		/// </summary>
		public float simulationSpeed
		{
			get
			{
				return GetSimulationSpeed(m_ParticleSystem);
			}
			set
			{
				SetSimulationSpeed(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When true, use the unscaled delta time to simulate the Particle System. Otherwise, use the scaled delta time.</para>
		/// </summary>
		public bool useUnscaledTime
		{
			get
			{
				return GetUseUnscaledTime(m_ParticleSystem);
			}
			set
			{
				SetUseUnscaledTime(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Control how the particle system's Transform Component is applied to the particle system.</para>
		/// </summary>
		public ParticleSystemScalingMode scalingMode
		{
			get
			{
				return GetScalingMode(m_ParticleSystem);
			}
			set
			{
				SetScalingMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>If set to true, the particle system will automatically start playing on startup.</para>
		/// </summary>
		public bool playOnAwake
		{
			get
			{
				return GetPlayOnAwake(m_ParticleSystem);
			}
			set
			{
				SetPlayOnAwake(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The maximum number of particles to emit.</para>
		/// </summary>
		public int maxParticles
		{
			get
			{
				return GetMaxParticles(m_ParticleSystem);
			}
			set
			{
				SetMaxParticles(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Control how the Particle System calculates its velocity, when moving in the world.</para>
		/// </summary>
		public ParticleSystemEmitterVelocityMode emitterVelocityMode
		{
			get
			{
				return GetUseRigidbodyForVelocity(m_ParticleSystem) ? ParticleSystemEmitterVelocityMode.Rigidbody : ParticleSystemEmitterVelocityMode.Transform;
			}
			set
			{
				SetUseRigidbodyForVelocity(m_ParticleSystem, value == ParticleSystemEmitterVelocityMode.Rigidbody);
			}
		}

		/// <summary>
		///   <para>Configure whether the GameObject will automatically disable or destroy itself, when the Particle System is stopped and all particles have died.</para>
		/// </summary>
		public ParticleSystemStopAction stopAction
		{
			get
			{
				return GetStopAction(m_ParticleSystem);
			}
			set
			{
				SetStopAction(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Cause some particles to spin in  the opposite direction.</para>
		/// </summary>
		[Obsolete("Please use flipRotation instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/MainModule.flipRotation", false)]
		public float randomizeRotationDirection
		{
			get
			{
				return flipRotation;
			}
			set
			{
				flipRotation = value;
			}
		}

		internal MainModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDuration(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetDuration(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetLoop(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetLoop(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetPrewarm(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetPrewarm(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartDelay(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartDelay(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartDelayMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartDelayMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartLifetime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartLifetime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartLifetimeMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartLifetimeMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSpeedMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartSpeedMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSize3D(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetStartSize3D(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSizeX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartSizeX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSizeXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartSizeXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSizeY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartSizeY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSizeYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartSizeYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSizeZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartSizeZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSizeZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartSizeZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotation3D(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetStartRotation3D(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotationX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartRotationX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotationXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartRotationXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotationY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartRotationY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotationYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartRotationYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotationZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartRotationZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotationZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartRotationZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetFlipRotation(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetFlipRotation(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartColor(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartColor(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetGravityModifier(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetGravityModifier(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetGravityModifierMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetGravityModifierMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSimulationSpace(ParticleSystem system, ParticleSystemSimulationSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemSimulationSpace GetSimulationSpace(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetCustomSimulationSpace(ParticleSystem system, Transform value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern Transform GetCustomSimulationSpace(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSimulationSpeed(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetSimulationSpeed(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUseUnscaledTime(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUseUnscaledTime(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetScalingMode(ParticleSystem system, ParticleSystemScalingMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemScalingMode GetScalingMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetPlayOnAwake(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetPlayOnAwake(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMaxParticles(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetMaxParticles(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUseRigidbodyForVelocity(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUseRigidbodyForVelocity(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStopAction(ParticleSystem system, ParticleSystemStopAction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemStopAction GetStopAction(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Emission module.</para>
	/// </summary>
	public struct EmissionModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Emission module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The rate at which new particles are spawned, over time.</para>
		/// </summary>
		public MinMaxCurve rateOverTime
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRateOverTime(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRateOverTime(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the rate over time multiplier.</para>
		/// </summary>
		public float rateOverTimeMultiplier
		{
			get
			{
				return GetRateOverTimeMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRateOverTimeMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The rate at which new particles are spawned, over distance.</para>
		/// </summary>
		public MinMaxCurve rateOverDistance
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRateOverDistance(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRateOverDistance(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the rate over distance multiplier.</para>
		/// </summary>
		public float rateOverDistanceMultiplier
		{
			get
			{
				return GetRateOverDistanceMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRateOverDistanceMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The current number of bursts.</para>
		/// </summary>
		public int burstCount
		{
			get
			{
				return GetBurstCount(m_ParticleSystem);
			}
			set
			{
				SetBurstCount(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The emission type.</para>
		/// </summary>
		[Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.", false)]
		public ParticleSystemEmissionType type
		{
			get
			{
				return ParticleSystemEmissionType.Time;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>The rate at which new particles are spawned.</para>
		/// </summary>
		[Obsolete("rate property is deprecated. Use rateOverTime or rateOverDistance instead.", false)]
		public MinMaxCurve rate
		{
			get
			{
				return rateOverTime;
			}
			set
			{
				rateOverTime = value;
			}
		}

		/// <summary>
		///   <para>Change the rate multiplier.</para>
		/// </summary>
		[Obsolete("rateMultiplier property is deprecated. Use rateOverTimeMultiplier or rateOverDistanceMultiplier instead.", false)]
		public float rateMultiplier
		{
			get
			{
				return rateOverTimeMultiplier;
			}
			set
			{
				rateOverTimeMultiplier = value;
			}
		}

		internal EmissionModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		public void SetBursts(Burst[] bursts)
		{
			SetBursts(bursts, bursts.Length);
		}

		public void SetBursts(Burst[] bursts, int size)
		{
			burstCount = size;
			for (int i = 0; i < size; i++)
			{
				SetBurst(m_ParticleSystem, i, bursts[i]);
			}
		}

		public int GetBursts(Burst[] bursts)
		{
			int num = burstCount;
			for (int i = 0; i < num; i++)
			{
				ref Burst reference = ref bursts[i];
				reference = GetBurst(m_ParticleSystem, i);
			}
			return num;
		}

		public void SetBurst(int index, Burst burst)
		{
			SetBurst(m_ParticleSystem, index, burst);
		}

		/// <summary>
		///   <para>Get a single burst from the array of bursts.</para>
		/// </summary>
		/// <param name="index">The index of the burst to retrieve.</param>
		/// <returns>
		///   <para>The burst data at the given index.</para>
		/// </returns>
		public Burst GetBurst(int index)
		{
			return GetBurst(m_ParticleSystem, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetBurstCount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRateOverTime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRateOverTime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRateOverTimeMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRateOverTimeMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRateOverDistance(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRateOverDistance(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRateOverDistanceMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRateOverDistanceMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetBurstCount(ParticleSystem system, int value);

		private static void SetBurst(ParticleSystem system, int index, Burst burst)
		{
			INTERNAL_CALL_SetBurst(system, index, ref burst);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetBurst(ParticleSystem system, int index, ref Burst burst);

		private static Burst GetBurst(ParticleSystem system, int index)
		{
			INTERNAL_CALL_GetBurst(system, index, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetBurst(ParticleSystem system, int index, out Burst value);
	}

	/// <summary>
	///   <para>Script interface for the Shape module.</para>
	/// </summary>
	public struct ShapeModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Shape module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Type of shape to emit particles from.</para>
		/// </summary>
		public ParticleSystemShapeType shapeType
		{
			get
			{
				return GetShapeType(m_ParticleSystem);
			}
			set
			{
				SetShapeType(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Randomizes the starting direction of particles.</para>
		/// </summary>
		public float randomDirectionAmount
		{
			get
			{
				return GetRandomDirectionAmount(m_ParticleSystem);
			}
			set
			{
				SetRandomDirectionAmount(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Spherizes the starting direction of particles.</para>
		/// </summary>
		public float sphericalDirectionAmount
		{
			get
			{
				return GetSphericalDirectionAmount(m_ParticleSystem);
			}
			set
			{
				SetSphericalDirectionAmount(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Randomizes the starting position of particles.</para>
		/// </summary>
		public float randomPositionAmount
		{
			get
			{
				return GetRandomPositionAmount(m_ParticleSystem);
			}
			set
			{
				SetRandomPositionAmount(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Align particles based on their initial direction of travel.</para>
		/// </summary>
		public bool alignToDirection
		{
			get
			{
				return GetAlignToDirection(m_ParticleSystem);
			}
			set
			{
				SetAlignToDirection(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Radius of the shape.</para>
		/// </summary>
		public float radius
		{
			get
			{
				return GetRadius(m_ParticleSystem);
			}
			set
			{
				SetRadius(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The mode used for generating particles along the radius.</para>
		/// </summary>
		public ParticleSystemShapeMultiModeValue radiusMode
		{
			get
			{
				return GetRadiusMode(m_ParticleSystem);
			}
			set
			{
				SetRadiusMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Control the gap between emission points along the radius.</para>
		/// </summary>
		public float radiusSpread
		{
			get
			{
				return GetRadiusSpread(m_ParticleSystem);
			}
			set
			{
				SetRadiusSpread(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When using one of the animated modes, how quickly to move the emission position along the radius.</para>
		/// </summary>
		public MinMaxCurve radiusSpeed
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRadiusSpeed(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRadiusSpeed(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>A multiplier of the radius speed of the emission shape.</para>
		/// </summary>
		public float radiusSpeedMultiplier
		{
			get
			{
				return GetRadiusSpeedMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRadiusSpeedMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Thickness of the radius.</para>
		/// </summary>
		public float radiusThickness
		{
			get
			{
				return GetRadiusThickness(m_ParticleSystem);
			}
			set
			{
				SetRadiusThickness(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Angle of the cone.</para>
		/// </summary>
		public float angle
		{
			get
			{
				return GetAngle(m_ParticleSystem);
			}
			set
			{
				SetAngle(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Length of the cone.</para>
		/// </summary>
		public float length
		{
			get
			{
				return GetLength(m_ParticleSystem);
			}
			set
			{
				SetLength(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Thickness of the box.</para>
		/// </summary>
		public Vector3 boxThickness
		{
			get
			{
				return GetBoxThickness(m_ParticleSystem);
			}
			set
			{
				SetBoxThickness(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Where on the mesh to emit particles from.</para>
		/// </summary>
		public ParticleSystemMeshShapeType meshShapeType
		{
			get
			{
				return GetMeshShapeType(m_ParticleSystem);
			}
			set
			{
				SetMeshShapeType(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Mesh to emit particles from.</para>
		/// </summary>
		public Mesh mesh
		{
			get
			{
				return GetMesh(m_ParticleSystem);
			}
			set
			{
				SetMesh(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>MeshRenderer to emit particles from.</para>
		/// </summary>
		public MeshRenderer meshRenderer
		{
			get
			{
				return GetMeshRenderer(m_ParticleSystem);
			}
			set
			{
				SetMeshRenderer(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>SkinnedMeshRenderer to emit particles from.</para>
		/// </summary>
		public SkinnedMeshRenderer skinnedMeshRenderer
		{
			get
			{
				return GetSkinnedMeshRenderer(m_ParticleSystem);
			}
			set
			{
				SetSkinnedMeshRenderer(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Emit from a single material, or the whole mesh.</para>
		/// </summary>
		public bool useMeshMaterialIndex
		{
			get
			{
				return GetUseMeshMaterialIndex(m_ParticleSystem);
			}
			set
			{
				SetUseMeshMaterialIndex(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Emit particles from a single material of a mesh.</para>
		/// </summary>
		public int meshMaterialIndex
		{
			get
			{
				return GetMeshMaterialIndex(m_ParticleSystem);
			}
			set
			{
				SetMeshMaterialIndex(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Modulate the particle colors with the vertex colors, or the material color if no vertex colors exist.</para>
		/// </summary>
		public bool useMeshColors
		{
			get
			{
				return GetUseMeshColors(m_ParticleSystem);
			}
			set
			{
				SetUseMeshColors(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Move particles away from the surface of the source mesh.</para>
		/// </summary>
		public float normalOffset
		{
			get
			{
				return GetNormalOffset(m_ParticleSystem);
			}
			set
			{
				SetNormalOffset(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Circle arc angle.</para>
		/// </summary>
		public float arc
		{
			get
			{
				return GetArc(m_ParticleSystem);
			}
			set
			{
				SetArc(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The mode used for generating particles around the arc.</para>
		/// </summary>
		public ParticleSystemShapeMultiModeValue arcMode
		{
			get
			{
				return GetArcMode(m_ParticleSystem);
			}
			set
			{
				SetArcMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Control the gap between emission points around the arc.</para>
		/// </summary>
		public float arcSpread
		{
			get
			{
				return GetArcSpread(m_ParticleSystem);
			}
			set
			{
				SetArcSpread(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When using one of the animated modes, how quickly to move the emission position around the arc.</para>
		/// </summary>
		public MinMaxCurve arcSpeed
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetArcSpeed(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetArcSpeed(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>A multiplier of the arc speed of the emission shape.</para>
		/// </summary>
		public float arcSpeedMultiplier
		{
			get
			{
				return GetArcSpeedMultiplier(m_ParticleSystem);
			}
			set
			{
				SetArcSpeedMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The radius of the Donut shape.</para>
		/// </summary>
		public float donutRadius
		{
			get
			{
				return GetDonutRadius(m_ParticleSystem);
			}
			set
			{
				SetDonutRadius(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Apply an offset to the position from which particles are emitted.</para>
		/// </summary>
		public Vector3 position
		{
			get
			{
				return GetPosition(m_ParticleSystem);
			}
			set
			{
				SetPosition(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Apply a rotation to the shape from which particles are emitted.</para>
		/// </summary>
		public Vector3 rotation
		{
			get
			{
				return GetRotation(m_ParticleSystem);
			}
			set
			{
				SetRotation(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Apply scale to the shape from which particles are emitted.</para>
		/// </summary>
		public Vector3 scale
		{
			get
			{
				return GetScale(m_ParticleSystem);
			}
			set
			{
				SetScale(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Selects a texture to be used for tinting particle start colors.</para>
		/// </summary>
		public Texture2D texture
		{
			get
			{
				return GetTexture(m_ParticleSystem);
			}
			set
			{
				SetTexture(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Selected which channel of the texture to be used for dicarding particles.</para>
		/// </summary>
		public ParticleSystemShapeTextureChannel textureClipChannel
		{
			get
			{
				return (ParticleSystemShapeTextureChannel)GetTextureClipChannel(m_ParticleSystem);
			}
			set
			{
				SetTextureClipChannel(m_ParticleSystem, (int)value);
			}
		}

		/// <summary>
		///   <para>Discards particles when they are spawned on an area of the texture with a value lower than this threshold.</para>
		/// </summary>
		public float textureClipThreshold
		{
			get
			{
				return GetTextureClipThreshold(m_ParticleSystem);
			}
			set
			{
				SetTextureClipThreshold(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When enabled, the RGB channels of the texture are applied to the particle color when spawned.</para>
		/// </summary>
		public bool textureColorAffectsParticles
		{
			get
			{
				return GetTextureColorAffectsParticles(m_ParticleSystem);
			}
			set
			{
				SetTextureColorAffectsParticles(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When enabled, the alpha channel of the texture is applied to the particle alpha when spawned.</para>
		/// </summary>
		public bool textureAlphaAffectsParticles
		{
			get
			{
				return GetTextureAlphaAffectsParticles(m_ParticleSystem);
			}
			set
			{
				SetTextureAlphaAffectsParticles(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When enabled, 4 neighboring samples are taken from the texture, and combined to give the final particle value.</para>
		/// </summary>
		public bool textureBilinearFiltering
		{
			get
			{
				return GetTextureBilinearFiltering(m_ParticleSystem);
			}
			set
			{
				SetTextureBilinearFiltering(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When using a Mesh as a source shape type, this option controls which UV channel on the Mesh is used for reading the source texture.</para>
		/// </summary>
		public int textureUVChannel
		{
			get
			{
				return GetTextureUVChannel(m_ParticleSystem);
			}
			set
			{
				SetTextureUVChannel(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Scale of the box.</para>
		/// </summary>
		[Obsolete("Please use scale instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/ShapeModule.scale", false)]
		public Vector3 box
		{
			get
			{
				return GetScale(m_ParticleSystem);
			}
			set
			{
				SetScale(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Apply a scaling factor to the mesh used for generating source positions.</para>
		/// </summary>
		[Obsolete("meshScale property is deprecated.Please use scale instead.", false)]
		public float meshScale
		{
			get
			{
				return scale.x;
			}
			set
			{
				scale = new Vector3(value, value, value);
			}
		}

		/// <summary>
		///   <para>Randomizes the starting direction of particles.</para>
		/// </summary>
		[Obsolete("randomDirection property is deprecated.Use randomDirectionAmount instead.", false)]
		public bool randomDirection
		{
			get
			{
				return randomDirectionAmount >= 0.5f;
			}
			set
			{
				randomDirectionAmount = ((!value) ? 0f : 1f);
			}
		}

		internal ShapeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetShapeType(ParticleSystem system, ParticleSystemShapeType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemShapeType GetShapeType(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRandomDirectionAmount(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRandomDirectionAmount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSphericalDirectionAmount(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetSphericalDirectionAmount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRandomPositionAmount(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRandomPositionAmount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetAlignToDirection(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetAlignToDirection(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadius(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRadius(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadiusMode(ParticleSystem system, ParticleSystemShapeMultiModeValue value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemShapeMultiModeValue GetRadiusMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadiusSpread(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRadiusSpread(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadiusSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRadiusSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadiusSpeedMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRadiusSpeedMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadiusThickness(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRadiusThickness(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetAngle(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetAngle(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetLength(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetLength(ParticleSystem system);

		private static void SetBoxThickness(ParticleSystem system, Vector3 value)
		{
			INTERNAL_CALL_SetBoxThickness(system, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetBoxThickness(ParticleSystem system, ref Vector3 value);

		private static Vector3 GetBoxThickness(ParticleSystem system)
		{
			INTERNAL_CALL_GetBoxThickness(system, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetBoxThickness(ParticleSystem system, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMeshShapeType(ParticleSystem system, ParticleSystemMeshShapeType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemMeshShapeType GetMeshShapeType(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMesh(ParticleSystem system, Mesh value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern Mesh GetMesh(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMeshRenderer(ParticleSystem system, MeshRenderer value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern MeshRenderer GetMeshRenderer(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSkinnedMeshRenderer(ParticleSystem system, SkinnedMeshRenderer value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern SkinnedMeshRenderer GetSkinnedMeshRenderer(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUseMeshMaterialIndex(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUseMeshMaterialIndex(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMeshMaterialIndex(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetMeshMaterialIndex(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUseMeshColors(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUseMeshColors(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetNormalOffset(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetNormalOffset(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetArc(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetArc(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetArcMode(ParticleSystem system, ParticleSystemShapeMultiModeValue value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemShapeMultiModeValue GetArcMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetArcSpread(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetArcSpread(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetArcSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetArcSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetArcSpeedMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetArcSpeedMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDonutRadius(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetDonutRadius(ParticleSystem system);

		private static void SetPosition(ParticleSystem system, Vector3 value)
		{
			INTERNAL_CALL_SetPosition(system, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetPosition(ParticleSystem system, ref Vector3 value);

		private static Vector3 GetPosition(ParticleSystem system)
		{
			INTERNAL_CALL_GetPosition(system, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetPosition(ParticleSystem system, out Vector3 value);

		private static void SetRotation(ParticleSystem system, Vector3 value)
		{
			INTERNAL_CALL_SetRotation(system, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetRotation(ParticleSystem system, ref Vector3 value);

		private static Vector3 GetRotation(ParticleSystem system)
		{
			INTERNAL_CALL_GetRotation(system, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetRotation(ParticleSystem system, out Vector3 value);

		private static void SetScale(ParticleSystem system, Vector3 value)
		{
			INTERNAL_CALL_SetScale(system, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetScale(ParticleSystem system, ref Vector3 value);

		private static Vector3 GetScale(ParticleSystem system)
		{
			INTERNAL_CALL_GetScale(system, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetScale(ParticleSystem system, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTexture(ParticleSystem system, Texture2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern Texture2D GetTexture(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTextureClipChannel(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetTextureClipChannel(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTextureClipThreshold(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetTextureClipThreshold(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTextureColorAffectsParticles(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetTextureColorAffectsParticles(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTextureAlphaAffectsParticles(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetTextureAlphaAffectsParticles(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTextureBilinearFiltering(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetTextureBilinearFiltering(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTextureUVChannel(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetTextureUVChannel(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Velocity Over Lifetime module.</para>
	/// </summary>
	public struct VelocityOverLifetimeModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Velocity Over Lifetime module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, on the X axis.</para>
		/// </summary>
		public MinMaxCurve x
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, on the Y axis.</para>
		/// </summary>
		public MinMaxCurve y
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, on the Z axis.</para>
		/// </summary>
		public MinMaxCurve z
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>X axis speed multiplier.</para>
		/// </summary>
		public float xMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Y axis speed multiplier.</para>
		/// </summary>
		public float yMultiplier
		{
			get
			{
				return GetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Z axis speed multiplier.</para>
		/// </summary>
		public float zMultiplier
		{
			get
			{
				return GetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, around the X axis.</para>
		/// </summary>
		public MinMaxCurve orbitalX
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetOrbitalX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetOrbitalX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, around the Y axis.</para>
		/// </summary>
		public MinMaxCurve orbitalY
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetOrbitalY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetOrbitalY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, around the Z axis.</para>
		/// </summary>
		public MinMaxCurve orbitalZ
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetOrbitalZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetOrbitalZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>X axis speed multiplier.</para>
		/// </summary>
		public float orbitalXMultiplier
		{
			get
			{
				return GetOrbitalXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetOrbitalXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Y axis speed multiplier.</para>
		/// </summary>
		public float orbitalYMultiplier
		{
			get
			{
				return GetOrbitalYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetOrbitalYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Z axis speed multiplier.</para>
		/// </summary>
		public float orbitalZMultiplier
		{
			get
			{
				return GetOrbitalZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetOrbitalZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Specify a custom center of rotation for the orbital and radial velocities.</para>
		/// </summary>
		public MinMaxCurve orbitalOffsetX
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetOrbitalOffsetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetOrbitalOffsetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Specify a custom center of rotation for the orbital and radial velocities.</para>
		/// </summary>
		public MinMaxCurve orbitalOffsetY
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetOrbitalOffsetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetOrbitalOffsetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Specify a custom center of rotation for the orbital and radial velocities.</para>
		/// </summary>
		public MinMaxCurve orbitalOffsetZ
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetOrbitalOffsetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetOrbitalOffsetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>This method is more efficient than accessing the whole curve, if you only want to change the overall offset multiplier.</para>
		/// </summary>
		public float orbitalOffsetXMultiplier
		{
			get
			{
				return GetOrbitalOffsetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetOrbitalOffsetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>This method is more efficient than accessing the whole curve, if you only want to change the overall offset multiplier.</para>
		/// </summary>
		public float orbitalOffsetYMultiplier
		{
			get
			{
				return GetOrbitalOffsetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetOrbitalOffsetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>This method is more efficient than accessing the whole curve, if you only want to change the overall offset multiplier.</para>
		/// </summary>
		public float orbitalOffsetZMultiplier
		{
			get
			{
				return GetOrbitalOffsetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetOrbitalOffsetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, away from a cetner position.</para>
		/// </summary>
		public MinMaxCurve radial
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRadial(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRadial(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Radial speed multiplier.</para>
		/// </summary>
		public float radialMultiplier
		{
			get
			{
				return GetRadialMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRadialMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle speed based on lifetime, without affecting the direction of the particles.</para>
		/// </summary>
		public MinMaxCurve speedModifier
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetSpeedModifier(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetSpeedModifier(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Speed multiplier.</para>
		/// </summary>
		public float speedModifierMultiplier
		{
			get
			{
				return GetSpeedModifierMultiplier(m_ParticleSystem);
			}
			set
			{
				SetSpeedModifierMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Specifies if the velocities are in local space (rotated with the transform) or world space.</para>
		/// </summary>
		public ParticleSystemSimulationSpace space
		{
			get
			{
				return GetWorldSpace(m_ParticleSystem) ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
			}
			set
			{
				SetWorldSpace(m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
			}
		}

		internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetOrbitalX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetOrbitalY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetOrbitalZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOrbitalXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOrbitalYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOrbitalZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalOffsetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetOrbitalOffsetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalOffsetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetOrbitalOffsetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalOffsetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetOrbitalOffsetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalOffsetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOrbitalOffsetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalOffsetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOrbitalOffsetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOrbitalOffsetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOrbitalOffsetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadial(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRadial(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadialMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRadialMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSpeedModifier(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetSpeedModifier(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSpeedModifierMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetSpeedModifierMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetWorldSpace(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetWorldSpace(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Limit Velocity Over Lifetime module.</para>
	/// </summary>
	public struct LimitVelocityOverLifetimeModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Limit Force Over Lifetime module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Maximum velocity curve for the X axis.</para>
		/// </summary>
		public MinMaxCurve limitX
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the limit multiplier on the X axis.</para>
		/// </summary>
		public float limitXMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Maximum velocity curve for the Y axis.</para>
		/// </summary>
		public MinMaxCurve limitY
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the limit multiplier on the Y axis.</para>
		/// </summary>
		public float limitYMultiplier
		{
			get
			{
				return GetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Maximum velocity curve for the Z axis.</para>
		/// </summary>
		public MinMaxCurve limitZ
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the limit multiplier on the Z axis.</para>
		/// </summary>
		public float limitZMultiplier
		{
			get
			{
				return GetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Maximum velocity curve, when not using one curve per axis.</para>
		/// </summary>
		public MinMaxCurve limit
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetMagnitude(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetMagnitude(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the limit multiplier.</para>
		/// </summary>
		public float limitMultiplier
		{
			get
			{
				return GetMagnitudeMultiplier(m_ParticleSystem);
			}
			set
			{
				SetMagnitudeMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Controls how much the velocity that exceeds the velocity limit should be dampened.</para>
		/// </summary>
		public float dampen
		{
			get
			{
				return GetDampen(m_ParticleSystem);
			}
			set
			{
				SetDampen(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set the velocity limit on each axis separately.</para>
		/// </summary>
		public bool separateAxes
		{
			get
			{
				return GetSeparateAxes(m_ParticleSystem);
			}
			set
			{
				SetSeparateAxes(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Specifies if the velocity limits are in local space (rotated with the transform) or world space.</para>
		/// </summary>
		public ParticleSystemSimulationSpace space
		{
			get
			{
				return GetWorldSpace(m_ParticleSystem) ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
			}
			set
			{
				SetWorldSpace(m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
			}
		}

		/// <summary>
		///   <para>Controls the amount of drag applied to the particle velocities.</para>
		/// </summary>
		public MinMaxCurve drag
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetDrag(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetDrag(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the drag multiplier.</para>
		/// </summary>
		public float dragMultiplier
		{
			get
			{
				return GetDragMultiplier(m_ParticleSystem);
			}
			set
			{
				SetDragMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Adjust the amount of drag applied to particles, based on their sizes.</para>
		/// </summary>
		public bool multiplyDragByParticleSize
		{
			get
			{
				return GetMultiplyDragByParticleSize(m_ParticleSystem);
			}
			set
			{
				SetMultiplyDragByParticleSize(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Adjust the amount of drag applied to particles, based on their speeds.</para>
		/// </summary>
		public bool multiplyDragByParticleVelocity
		{
			get
			{
				return GetMultiplyDragByParticleVelocity(m_ParticleSystem);
			}
			set
			{
				SetMultiplyDragByParticleVelocity(m_ParticleSystem, value);
			}
		}

		internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMagnitude(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetMagnitude(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMagnitudeMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetMagnitudeMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDampen(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetDampen(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSeparateAxes(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSeparateAxes(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetWorldSpace(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetWorldSpace(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDrag(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetDrag(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDragMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetDragMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMultiplyDragByParticleSize(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetMultiplyDragByParticleSize(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMultiplyDragByParticleVelocity(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetMultiplyDragByParticleVelocity(ParticleSystem system);
	}

	/// <summary>
	///   <para>The Inherit Velocity Module controls how the velocity of the emitter is transferred to the particles as they are emitted.</para>
	/// </summary>
	public struct InheritVelocityModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the InheritVelocity module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>How to apply emitter velocity to particles.</para>
		/// </summary>
		public ParticleSystemInheritVelocityMode mode
		{
			get
			{
				return GetMode(m_ParticleSystem);
			}
			set
			{
				SetMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to define how much emitter velocity is applied during the lifetime of a particle.</para>
		/// </summary>
		public MinMaxCurve curve
		{
			get
			{
				MinMaxCurve result = default(MinMaxCurve);
				GetCurve(m_ParticleSystem, ref result);
				return result;
			}
			set
			{
				SetCurve(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the curve multiplier.</para>
		/// </summary>
		public float curveMultiplier
		{
			get
			{
				return GetCurveMultiplier(m_ParticleSystem);
			}
			set
			{
				SetCurveMultiplier(m_ParticleSystem, value);
			}
		}

		internal InheritVelocityModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMode(ParticleSystem system, ParticleSystemInheritVelocityMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemInheritVelocityMode GetMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetCurve(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetCurve(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetCurveMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetCurveMultiplier(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Force Over Lifetime module.</para>
	/// </summary>
	public struct ForceOverLifetimeModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Force Over Lifetime module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The curve defining particle forces in the X axis.</para>
		/// </summary>
		public MinMaxCurve x
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>The curve defining particle forces in the Y axis.</para>
		/// </summary>
		public MinMaxCurve y
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>The curve defining particle forces in the Z axis.</para>
		/// </summary>
		public MinMaxCurve z
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the X axis mulutiplier.</para>
		/// </summary>
		public float xMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Change the Y axis multiplier.</para>
		/// </summary>
		public float yMultiplier
		{
			get
			{
				return GetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Change the Z axis multiplier.</para>
		/// </summary>
		public float zMultiplier
		{
			get
			{
				return GetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Are the forces being applied in local or world space?</para>
		/// </summary>
		public ParticleSystemSimulationSpace space
		{
			get
			{
				return GetWorldSpace(m_ParticleSystem) ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
			}
			set
			{
				SetWorldSpace(m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
			}
		}

		/// <summary>
		///   <para>When randomly selecting values between two curves or constants, this flag will cause a new random force to be chosen on each frame.</para>
		/// </summary>
		public bool randomized
		{
			get
			{
				return GetRandomized(m_ParticleSystem);
			}
			set
			{
				SetRandomized(m_ParticleSystem, value);
			}
		}

		internal ForceOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetWorldSpace(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetWorldSpace(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRandomized(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetRandomized(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Color Over Lifetime module.</para>
	/// </summary>
	public struct ColorOverLifetimeModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Color Over Lifetime module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The gradient controlling the particle colors.</para>
		/// </summary>
		public MinMaxGradient color
		{
			get
			{
				MinMaxGradient gradient = default(MinMaxGradient);
				GetColor(m_ParticleSystem, ref gradient);
				return gradient;
			}
			set
			{
				SetColor(m_ParticleSystem, ref value);
			}
		}

		internal ColorOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetColor(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetColor(ParticleSystem system, ref MinMaxGradient gradient);
	}

	/// <summary>
	///   <para>Script interface for the Color By Speed module.</para>
	/// </summary>
	public struct ColorBySpeedModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Color By Speed module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The gradient controlling the particle colors.</para>
		/// </summary>
		public MinMaxGradient color
		{
			get
			{
				MinMaxGradient gradient = default(MinMaxGradient);
				GetColor(m_ParticleSystem, ref gradient);
				return gradient;
			}
			set
			{
				SetColor(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Apply the color gradient between these minimum and maximum speeds.</para>
		/// </summary>
		public Vector2 range
		{
			get
			{
				return GetRange(m_ParticleSystem);
			}
			set
			{
				SetRange(m_ParticleSystem, value);
			}
		}

		internal ColorBySpeedModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetColor(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetColor(ParticleSystem system, ref MinMaxGradient gradient);

		private static void SetRange(ParticleSystem system, Vector2 value)
		{
			INTERNAL_CALL_SetRange(system, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

		private static Vector2 GetRange(ParticleSystem system)
		{
			INTERNAL_CALL_GetRange(system, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
	}

	/// <summary>
	///   <para>Script interface for the Size Over Lifetime module.</para>
	/// </summary>
	public struct SizeOverLifetimeModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Size Over Lifetime module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle size based on lifetime.</para>
		/// </summary>
		public MinMaxCurve size
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Size multiplier.</para>
		/// </summary>
		public float sizeMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Size over lifetime curve for the X axis.</para>
		/// </summary>
		public MinMaxCurve x
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>X axis size multiplier.</para>
		/// </summary>
		public float xMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Size over lifetime curve for the Y axis.</para>
		/// </summary>
		public MinMaxCurve y
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Y axis size multiplier.</para>
		/// </summary>
		public float yMultiplier
		{
			get
			{
				return GetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Size over lifetime curve for the Z axis.</para>
		/// </summary>
		public MinMaxCurve z
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Z axis size multiplier.</para>
		/// </summary>
		public float zMultiplier
		{
			get
			{
				return GetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set the size over lifetime on each axis separately.</para>
		/// </summary>
		public bool separateAxes
		{
			get
			{
				return GetSeparateAxes(m_ParticleSystem);
			}
			set
			{
				SetSeparateAxes(m_ParticleSystem, value);
			}
		}

		internal SizeOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSeparateAxes(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSeparateAxes(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Size By Speed module.</para>
	/// </summary>
	public struct SizeBySpeedModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Size By Speed module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to control particle size based on speed.</para>
		/// </summary>
		public MinMaxCurve size
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Size multiplier.</para>
		/// </summary>
		public float sizeMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Size by speed curve for the X axis.</para>
		/// </summary>
		public MinMaxCurve x
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>X axis size multiplier.</para>
		/// </summary>
		public float xMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Size by speed curve for the Y axis.</para>
		/// </summary>
		public MinMaxCurve y
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Y axis size multiplier.</para>
		/// </summary>
		public float yMultiplier
		{
			get
			{
				return GetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Size by speed curve for the Z axis.</para>
		/// </summary>
		public MinMaxCurve z
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Z axis size multiplier.</para>
		/// </summary>
		public float zMultiplier
		{
			get
			{
				return GetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set the size by speed on each axis separately.</para>
		/// </summary>
		public bool separateAxes
		{
			get
			{
				return GetSeparateAxes(m_ParticleSystem);
			}
			set
			{
				SetSeparateAxes(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Apply the size curve between these minimum and maximum speeds.</para>
		/// </summary>
		public Vector2 range
		{
			get
			{
				return GetRange(m_ParticleSystem);
			}
			set
			{
				SetRange(m_ParticleSystem, value);
			}
		}

		internal SizeBySpeedModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSeparateAxes(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSeparateAxes(ParticleSystem system);

		private static void SetRange(ParticleSystem system, Vector2 value)
		{
			INTERNAL_CALL_SetRange(system, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

		private static Vector2 GetRange(ParticleSystem system)
		{
			INTERNAL_CALL_GetRange(system, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
	}

	/// <summary>
	///   <para>Script interface for the Rotation Over Lifetime module.</para>
	/// </summary>
	public struct RotationOverLifetimeModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Rotation Over Lifetime module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Rotation over lifetime curve for the X axis.</para>
		/// </summary>
		public MinMaxCurve x
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Rotation multiplier around the X axis.</para>
		/// </summary>
		public float xMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Rotation over lifetime curve for the Y axis.</para>
		/// </summary>
		public MinMaxCurve y
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Rotation multiplier around the Y axis.</para>
		/// </summary>
		public float yMultiplier
		{
			get
			{
				return GetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Rotation over lifetime curve for the Z axis.</para>
		/// </summary>
		public MinMaxCurve z
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Rotation multiplier around the Z axis.</para>
		/// </summary>
		public float zMultiplier
		{
			get
			{
				return GetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set the rotation over lifetime on each axis separately.</para>
		/// </summary>
		public bool separateAxes
		{
			get
			{
				return GetSeparateAxes(m_ParticleSystem);
			}
			set
			{
				SetSeparateAxes(m_ParticleSystem, value);
			}
		}

		internal RotationOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSeparateAxes(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSeparateAxes(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Rotation By Speed module.</para>
	/// </summary>
	public struct RotationBySpeedModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Rotation By Speed module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Rotation by speed curve for the X axis.</para>
		/// </summary>
		public MinMaxCurve x
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Speed multiplier along the X axis.</para>
		/// </summary>
		public float xMultiplier
		{
			get
			{
				return GetXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Rotation by speed curve for the Y axis.</para>
		/// </summary>
		public MinMaxCurve y
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Speed multiplier along the Y axis.</para>
		/// </summary>
		public float yMultiplier
		{
			get
			{
				return GetYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Rotation by speed curve for the Z axis.</para>
		/// </summary>
		public MinMaxCurve z
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Speed multiplier along the Z axis.</para>
		/// </summary>
		public float zMultiplier
		{
			get
			{
				return GetZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set the rotation by speed on each axis separately.</para>
		/// </summary>
		public bool separateAxes
		{
			get
			{
				return GetSeparateAxes(m_ParticleSystem);
			}
			set
			{
				SetSeparateAxes(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Apply the rotation curve between these minimum and maximum speeds.</para>
		/// </summary>
		public Vector2 range
		{
			get
			{
				return GetRange(m_ParticleSystem);
			}
			set
			{
				SetRange(m_ParticleSystem, value);
			}
		}

		internal RotationBySpeedModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSeparateAxes(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSeparateAxes(ParticleSystem system);

		private static void SetRange(ParticleSystem system, Vector2 value)
		{
			INTERNAL_CALL_SetRange(system, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

		private static Vector2 GetRange(ParticleSystem system)
		{
			INTERNAL_CALL_GetRange(system, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
	}

	/// <summary>
	///   <para>Script interface for the External Forces module.</para>
	/// </summary>
	public struct ExternalForcesModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the External Forces module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Multiplies the magnitude of applied external forces.</para>
		/// </summary>
		public float multiplier
		{
			get
			{
				return GetMultiplier(m_ParticleSystem);
			}
			set
			{
				SetMultiplier(m_ParticleSystem, value);
			}
		}

		internal ExternalForcesModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetMultiplier(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Noise Module.
	///
	/// The Noise Module allows you to apply turbulence to the movement of your particles. Use the low quality settings to create computationally efficient Noise, or simulate smoother, richer Noise with the higher quality settings. You can also choose to define the behavior of the Noise individually for each axis.</para>
	/// </summary>
	public struct NoiseModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Noise module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Control the noise separately for each axis.</para>
		/// </summary>
		public bool separateAxes
		{
			get
			{
				return GetSeparateAxes(m_ParticleSystem);
			}
			set
			{
				SetSeparateAxes(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>How strong the overall noise effect is.</para>
		/// </summary>
		public MinMaxCurve strength
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStrengthX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStrengthX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Strength multiplier.</para>
		/// </summary>
		public float strengthMultiplier
		{
			get
			{
				return GetStrengthXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStrengthXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define the strength of the effect on the X axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
		/// </summary>
		public MinMaxCurve strengthX
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStrengthX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStrengthX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>X axis strength multiplier.</para>
		/// </summary>
		public float strengthXMultiplier
		{
			get
			{
				return GetStrengthXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStrengthXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define the strength of the effect on the Y axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
		/// </summary>
		public MinMaxCurve strengthY
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStrengthY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStrengthY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Y axis strength multiplier.</para>
		/// </summary>
		public float strengthYMultiplier
		{
			get
			{
				return GetStrengthYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStrengthYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define the strength of the effect on the Z axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
		/// </summary>
		public MinMaxCurve strengthZ
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStrengthZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStrengthZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Z axis strength multiplier.</para>
		/// </summary>
		public float strengthZMultiplier
		{
			get
			{
				return GetStrengthZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStrengthZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Low values create soft, smooth noise, and high values create rapidly changing noise.</para>
		/// </summary>
		public float frequency
		{
			get
			{
				return GetFrequency(m_ParticleSystem);
			}
			set
			{
				SetFrequency(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Higher frequency noise will reduce the strength by a proportional amount, if enabled.</para>
		/// </summary>
		public bool damping
		{
			get
			{
				return GetDamping(m_ParticleSystem);
			}
			set
			{
				SetDamping(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Layers of noise that combine to produce final noise.</para>
		/// </summary>
		public int octaveCount
		{
			get
			{
				return GetOctaveCount(m_ParticleSystem);
			}
			set
			{
				SetOctaveCount(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When combining each octave, scale the intensity by this amount.</para>
		/// </summary>
		public float octaveMultiplier
		{
			get
			{
				return GetOctaveMultiplier(m_ParticleSystem);
			}
			set
			{
				SetOctaveMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>When combining each octave, zoom in by this amount.</para>
		/// </summary>
		public float octaveScale
		{
			get
			{
				return GetOctaveScale(m_ParticleSystem);
			}
			set
			{
				SetOctaveScale(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Generate 1D, 2D or 3D noise.</para>
		/// </summary>
		public ParticleSystemNoiseQuality quality
		{
			get
			{
				return (ParticleSystemNoiseQuality)GetQuality(m_ParticleSystem);
			}
			set
			{
				SetQuality(m_ParticleSystem, (int)value);
			}
		}

		/// <summary>
		///   <para>Scroll the noise map over the particle system.</para>
		/// </summary>
		public MinMaxCurve scrollSpeed
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetScrollSpeed(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetScrollSpeed(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Scroll speed multiplier.</para>
		/// </summary>
		public float scrollSpeedMultiplier
		{
			get
			{
				return GetScrollSpeedMultiplier(m_ParticleSystem);
			}
			set
			{
				SetScrollSpeedMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Enable remapping of the final noise values, allowing for noise values to be translated into different values.</para>
		/// </summary>
		public bool remapEnabled
		{
			get
			{
				return GetRemapEnabled(m_ParticleSystem);
			}
			set
			{
				SetRemapEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define how the noise values are remapped.</para>
		/// </summary>
		public MinMaxCurve remap
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRemapX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRemapX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Remap multiplier.</para>
		/// </summary>
		public float remapMultiplier
		{
			get
			{
				return GetRemapXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRemapXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define how the noise values are remapped on the X axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
		/// </summary>
		public MinMaxCurve remapX
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRemapX(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRemapX(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>X axis remap multiplier.</para>
		/// </summary>
		public float remapXMultiplier
		{
			get
			{
				return GetRemapXMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRemapXMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define how the noise values are remapped on the Y axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
		/// </summary>
		public MinMaxCurve remapY
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRemapY(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRemapY(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Y axis remap multiplier.</para>
		/// </summary>
		public float remapYMultiplier
		{
			get
			{
				return GetRemapYMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRemapYMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define how the noise values are remapped on the Z axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
		/// </summary>
		public MinMaxCurve remapZ
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRemapZ(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRemapZ(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Z axis remap multiplier.</para>
		/// </summary>
		public float remapZMultiplier
		{
			get
			{
				return GetRemapZMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRemapZMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>How much the noise affects the particle positions.</para>
		/// </summary>
		public MinMaxCurve positionAmount
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetPositionAmount(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetPositionAmount(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>How much the noise affects the particle rotation, in degrees per second.</para>
		/// </summary>
		public MinMaxCurve rotationAmount
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRotationAmount(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRotationAmount(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>How much the noise affects the particle sizes, applied as a multiplier on the size of each particle.</para>
		/// </summary>
		public MinMaxCurve sizeAmount
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetSizeAmount(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetSizeAmount(m_ParticleSystem, ref value);
			}
		}

		internal NoiseModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSeparateAxes(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSeparateAxes(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStrengthX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStrengthX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStrengthY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStrengthY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStrengthZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStrengthZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStrengthXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStrengthXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStrengthYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStrengthYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStrengthZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStrengthZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetFrequency(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetFrequency(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDamping(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetDamping(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOctaveCount(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetOctaveCount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOctaveMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOctaveMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOctaveScale(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetOctaveScale(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetQuality(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetQuality(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetScrollSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetScrollSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetScrollSpeedMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetScrollSpeedMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRemapEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetRemapEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRemapX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRemapX(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRemapY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRemapY(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRemapZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRemapZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRemapXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRemapXMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRemapYMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRemapYMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRemapZMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRemapZMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetPositionAmount(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetPositionAmount(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRotationAmount(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRotationAmount(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSizeAmount(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetSizeAmount(ParticleSystem system, ref MinMaxCurve curve);
	}

	/// <summary>
	///   <para>Script interface for the Collision module.</para>
	/// </summary>
	public struct CollisionModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Collision module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The type of particle collision to perform.</para>
		/// </summary>
		public ParticleSystemCollisionType type
		{
			get
			{
				return GetType(m_ParticleSystem);
			}
			set
			{
				SetType(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose between 2D and 3D world collisions.</para>
		/// </summary>
		public ParticleSystemCollisionMode mode
		{
			get
			{
				return GetMode(m_ParticleSystem);
			}
			set
			{
				SetMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>How much speed is lost from each particle after a collision.</para>
		/// </summary>
		public MinMaxCurve dampen
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetDampen(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetDampen(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the dampen multiplier.</para>
		/// </summary>
		public float dampenMultiplier
		{
			get
			{
				return GetDampenMultiplier(m_ParticleSystem);
			}
			set
			{
				SetDampenMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>How much force is applied to each particle after a collision.</para>
		/// </summary>
		public MinMaxCurve bounce
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetBounce(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetBounce(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the bounce multiplier.</para>
		/// </summary>
		public float bounceMultiplier
		{
			get
			{
				return GetBounceMultiplier(m_ParticleSystem);
			}
			set
			{
				SetBounceMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>How much a particle's lifetime is reduced after a collision.</para>
		/// </summary>
		public MinMaxCurve lifetimeLoss
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetLifetimeLoss(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetLifetimeLoss(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the lifetime loss multiplier.</para>
		/// </summary>
		public float lifetimeLossMultiplier
		{
			get
			{
				return GetLifetimeLossMultiplier(m_ParticleSystem);
			}
			set
			{
				SetLifetimeLossMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Kill particles whose speed falls below this threshold, after a collision.</para>
		/// </summary>
		public float minKillSpeed
		{
			get
			{
				return GetMinKillSpeed(m_ParticleSystem);
			}
			set
			{
				SetMinKillSpeed(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Kill particles whose speed goes above this threshold, after a collision.</para>
		/// </summary>
		public float maxKillSpeed
		{
			get
			{
				return GetMaxKillSpeed(m_ParticleSystem);
			}
			set
			{
				SetMaxKillSpeed(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Control which layers this particle system collides with.</para>
		/// </summary>
		public LayerMask collidesWith
		{
			get
			{
				return GetCollidesWith(m_ParticleSystem);
			}
			set
			{
				SetCollidesWith(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Allow particles to collide with dynamic colliders when using world collision mode.</para>
		/// </summary>
		public bool enableDynamicColliders
		{
			get
			{
				return GetEnableDynamicColliders(m_ParticleSystem);
			}
			set
			{
				SetEnableDynamicColliders(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The maximum number of collision shapes that will be considered for particle collisions. Excess shapes will be ignored. Terrains take priority.</para>
		/// </summary>
		public int maxCollisionShapes
		{
			get
			{
				return GetMaxCollisionShapes(m_ParticleSystem);
			}
			set
			{
				SetMaxCollisionShapes(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Specifies the accuracy of particle collisions against colliders in the scene.</para>
		/// </summary>
		public ParticleSystemCollisionQuality quality
		{
			get
			{
				return (ParticleSystemCollisionQuality)GetQuality(m_ParticleSystem);
			}
			set
			{
				SetQuality(m_ParticleSystem, (int)value);
			}
		}

		/// <summary>
		///   <para>Size of voxels in the collision cache.</para>
		/// </summary>
		public float voxelSize
		{
			get
			{
				return GetVoxelSize(m_ParticleSystem);
			}
			set
			{
				SetVoxelSize(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>A multiplier applied to the size of each particle before collisions are processed.</para>
		/// </summary>
		public float radiusScale
		{
			get
			{
				return GetRadiusScale(m_ParticleSystem);
			}
			set
			{
				SetRadiusScale(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Send collision callback messages.</para>
		/// </summary>
		public bool sendCollisionMessages
		{
			get
			{
				return GetUsesCollisionMessages(m_ParticleSystem);
			}
			set
			{
				SetUsesCollisionMessages(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>How much force is applied to a Collider when hit by particles from this Particle System.</para>
		/// </summary>
		public float colliderForce
		{
			get
			{
				return GetColliderForce(m_ParticleSystem);
			}
			set
			{
				SetColliderForce(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>If true, the collision angle is considered when applying forces from particles to Colliders.</para>
		/// </summary>
		public bool multiplyColliderForceByCollisionAngle
		{
			get
			{
				return GetMultiplyColliderForceByCollisionAngle(m_ParticleSystem);
			}
			set
			{
				SetMultiplyColliderForceByCollisionAngle(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>If true, particle speeds are considered when applying forces to Colliders.</para>
		/// </summary>
		public bool multiplyColliderForceByParticleSpeed
		{
			get
			{
				return GetMultiplyColliderForceByParticleSpeed(m_ParticleSystem);
			}
			set
			{
				SetMultiplyColliderForceByParticleSpeed(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>If true, particle sizes are considered when applying forces to Colliders.</para>
		/// </summary>
		public bool multiplyColliderForceByParticleSize
		{
			get
			{
				return GetMultiplyColliderForceByParticleSize(m_ParticleSystem);
			}
			set
			{
				SetMultiplyColliderForceByParticleSize(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The maximum number of planes it is possible to set as colliders.</para>
		/// </summary>
		public int maxPlaneCount => GetMaxPlaneCount(m_ParticleSystem);

		/// <summary>
		///   <para>Allow particles to collide when inside colliders.</para>
		/// </summary>
		[Obsolete("enableInteriorCollisions property is deprecated and is no longer required and has no effect on the particle system.", false)]
		public bool enableInteriorCollisions
		{
			get
			{
				return GetEnableInteriorCollisions(m_ParticleSystem);
			}
			set
			{
				SetEnableInteriorCollisions(m_ParticleSystem, value);
			}
		}

		internal CollisionModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		/// <summary>
		///   <para>Set a collision plane to be used with this particle system.</para>
		/// </summary>
		/// <param name="index">Specifies which plane to set.</param>
		/// <param name="transform">The plane to set.</param>
		public void SetPlane(int index, Transform transform)
		{
			SetPlane(m_ParticleSystem, index, transform);
		}

		/// <summary>
		///   <para>Get a collision plane associated with this particle system.</para>
		/// </summary>
		/// <param name="index">Specifies which plane to access.</param>
		/// <returns>
		///   <para>The plane.</para>
		/// </returns>
		public Transform GetPlane(int index)
		{
			return GetPlane(m_ParticleSystem, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetType(ParticleSystem system, ParticleSystemCollisionType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemCollisionType GetType(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMode(ParticleSystem system, ParticleSystemCollisionMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemCollisionMode GetMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDampen(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetDampen(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDampenMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetDampenMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetBounce(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetBounce(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetBounceMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetBounceMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetLifetimeLoss(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetLifetimeLoss(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetLifetimeLossMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetLifetimeLossMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMinKillSpeed(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetMinKillSpeed(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMaxKillSpeed(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetMaxKillSpeed(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetCollidesWith(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetCollidesWith(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnableDynamicColliders(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnableDynamicColliders(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnableInteriorCollisions(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnableInteriorCollisions(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMaxCollisionShapes(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetMaxCollisionShapes(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetQuality(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetQuality(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetVoxelSize(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetVoxelSize(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadiusScale(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRadiusScale(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUsesCollisionMessages(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUsesCollisionMessages(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetColliderForce(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetColliderForce(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMultiplyColliderForceByCollisionAngle(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetMultiplyColliderForceByCollisionAngle(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMultiplyColliderForceByParticleSpeed(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetMultiplyColliderForceByParticleSpeed(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMultiplyColliderForceByParticleSize(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetMultiplyColliderForceByParticleSize(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetPlane(ParticleSystem system, int index, Transform transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern Transform GetPlane(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetMaxPlaneCount(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Trigger module.</para>
	/// </summary>
	public struct TriggerModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Trigger module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose what action to perform when particles are inside the trigger volume.</para>
		/// </summary>
		public ParticleSystemOverlapAction inside
		{
			get
			{
				return GetInside(m_ParticleSystem);
			}
			set
			{
				SetInside(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose what action to perform when particles are outside the trigger volume.</para>
		/// </summary>
		public ParticleSystemOverlapAction outside
		{
			get
			{
				return GetOutside(m_ParticleSystem);
			}
			set
			{
				SetOutside(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose what action to perform when particles enter the trigger volume.</para>
		/// </summary>
		public ParticleSystemOverlapAction enter
		{
			get
			{
				return GetEnter(m_ParticleSystem);
			}
			set
			{
				SetEnter(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose what action to perform when particles leave the trigger volume.</para>
		/// </summary>
		public ParticleSystemOverlapAction exit
		{
			get
			{
				return GetExit(m_ParticleSystem);
			}
			set
			{
				SetExit(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>A multiplier applied to the size of each particle before overlaps are processed.</para>
		/// </summary>
		public float radiusScale
		{
			get
			{
				return GetRadiusScale(m_ParticleSystem);
			}
			set
			{
				SetRadiusScale(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The maximum number of collision shapes that can be attached to this particle system trigger.</para>
		/// </summary>
		public int maxColliderCount => GetMaxColliderCount(m_ParticleSystem);

		internal TriggerModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		/// <summary>
		///   <para>Set a collision shape associated with this particle system trigger.</para>
		/// </summary>
		/// <param name="index">Which collider to set.</param>
		/// <param name="collider">The collider to associate with this trigger.</param>
		public void SetCollider(int index, Component collider)
		{
			SetCollider(m_ParticleSystem, index, collider);
		}

		/// <summary>
		///   <para>Get a collision shape associated with this particle system trigger.</para>
		/// </summary>
		/// <param name="index">Which collider to return.</param>
		/// <returns>
		///   <para>The collider at the given index.</para>
		/// </returns>
		public Component GetCollider(int index)
		{
			return GetCollider(m_ParticleSystem, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetInside(ParticleSystem system, ParticleSystemOverlapAction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemOverlapAction GetInside(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetOutside(ParticleSystem system, ParticleSystemOverlapAction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemOverlapAction GetOutside(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnter(ParticleSystem system, ParticleSystemOverlapAction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemOverlapAction GetEnter(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetExit(ParticleSystem system, ParticleSystemOverlapAction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemOverlapAction GetExit(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRadiusScale(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRadiusScale(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetCollider(ParticleSystem system, int index, Component collider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern Component GetCollider(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetMaxColliderCount(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Sub Emitters module.</para>
	/// </summary>
	public struct SubEmittersModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Sub Emitters module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The total number of sub-emitters.</para>
		/// </summary>
		public int subEmittersCount => GetSubEmittersCount(m_ParticleSystem);

		/// <summary>
		///   <para>Sub particle system which spawns at the locations of the birth of the particles from the parent system.</para>
		/// </summary>
		[Obsolete("birth0 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem birth0
		{
			get
			{
				return GetBirth(m_ParticleSystem, 0);
			}
			set
			{
				SetBirth(m_ParticleSystem, 0, value);
			}
		}

		/// <summary>
		///   <para>Sub particle system which spawns at the locations of the birth of the particles from the parent system.</para>
		/// </summary>
		[Obsolete("birth1 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem birth1
		{
			get
			{
				return GetBirth(m_ParticleSystem, 1);
			}
			set
			{
				SetBirth(m_ParticleSystem, 1, value);
			}
		}

		/// <summary>
		///   <para>Sub particle system which spawns at the locations of the collision of the particles from the parent system.</para>
		/// </summary>
		[Obsolete("collision0 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem collision0
		{
			get
			{
				return GetCollision(m_ParticleSystem, 0);
			}
			set
			{
				SetCollision(m_ParticleSystem, 0, value);
			}
		}

		/// <summary>
		///   <para>Sub particle system which spawns at the locations of the collision of the particles from the parent system.</para>
		/// </summary>
		[Obsolete("collision1 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem collision1
		{
			get
			{
				return GetCollision(m_ParticleSystem, 1);
			}
			set
			{
				SetCollision(m_ParticleSystem, 1, value);
			}
		}

		/// <summary>
		///   <para>Sub particle system which spawns at the locations of the death of the particles from the parent system.</para>
		/// </summary>
		[Obsolete("death0 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem death0
		{
			get
			{
				return GetDeath(m_ParticleSystem, 0);
			}
			set
			{
				SetDeath(m_ParticleSystem, 0, value);
			}
		}

		/// <summary>
		///   <para>Sub particle system to spawn on death of the parent system's particles.</para>
		/// </summary>
		[Obsolete("death1 property is deprecated.Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem death1
		{
			get
			{
				return GetDeath(m_ParticleSystem, 1);
			}
			set
			{
				SetDeath(m_ParticleSystem, 1, value);
			}
		}

		internal SubEmittersModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		/// <summary>
		///   <para>Add a new sub-emitter.</para>
		/// </summary>
		/// <param name="subEmitter">The sub-emitter to be added.</param>
		/// <param name="type">The event that creates new particles.</param>
		/// <param name="properties">The properties of the new particles.</param>
		public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties)
		{
			AddSubEmitter(m_ParticleSystem, subEmitter, (int)type, (int)properties);
		}

		/// <summary>
		///   <para>Remove a sub-emitter from the given index in the array.</para>
		/// </summary>
		/// <param name="index">The index from which to remove a sub-emitter.</param>
		public void RemoveSubEmitter(int index)
		{
			RemoveSubEmitter(m_ParticleSystem, index);
		}

		/// <summary>
		///   <para>Set the Particle System to use as the sub-emitter at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the sub-emitter being modified.</param>
		/// <param name="subEmitter">The Particle System being used as this sub-emitter.</param>
		public void SetSubEmitterSystem(int index, ParticleSystem subEmitter)
		{
			SetSubEmitterSystem(m_ParticleSystem, index, subEmitter);
		}

		/// <summary>
		///   <para>Set the type of the sub-emitter at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the sub-emitter being modified.</param>
		/// <param name="type">The new spawning type to assign to this sub-emitter.</param>
		public void SetSubEmitterType(int index, ParticleSystemSubEmitterType type)
		{
			SetSubEmitterType(m_ParticleSystem, index, (int)type);
		}

		/// <summary>
		///   <para>Set the properties of the sub-emitter at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the sub-emitter being modified.</param>
		/// <param name="properties">The new properties to assign to this sub-emitter.</param>
		public void SetSubEmitterProperties(int index, ParticleSystemSubEmitterProperties properties)
		{
			SetSubEmitterProperties(m_ParticleSystem, index, (int)properties);
		}

		/// <summary>
		///   <para>Get the sub-emitter Particle System at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the desired sub-emitter.</param>
		/// <returns>
		///   <para>The sub-emitter being requested.</para>
		/// </returns>
		public ParticleSystem GetSubEmitterSystem(int index)
		{
			return GetSubEmitterSystem(m_ParticleSystem, index);
		}

		/// <summary>
		///   <para>Get the type of the sub-emitter at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the desired sub-emitter.</param>
		/// <returns>
		///   <para>The type of the requested sub-emitter.</para>
		/// </returns>
		public ParticleSystemSubEmitterType GetSubEmitterType(int index)
		{
			return (ParticleSystemSubEmitterType)GetSubEmitterType(m_ParticleSystem, index);
		}

		/// <summary>
		///   <para>Get the properties of the sub-emitter at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the desired sub-emitter.</param>
		/// <returns>
		///   <para>The properties of the requested sub-emitter.</para>
		/// </returns>
		public ParticleSystemSubEmitterProperties GetSubEmitterProperties(int index)
		{
			return (ParticleSystemSubEmitterProperties)GetSubEmitterProperties(m_ParticleSystem, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetSubEmittersCount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetBirth(ParticleSystem system, int index, ParticleSystem value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystem GetBirth(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetCollision(ParticleSystem system, int index, ParticleSystem value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystem GetCollision(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDeath(ParticleSystem system, int index, ParticleSystem value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystem GetDeath(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void AddSubEmitter(ParticleSystem system, ParticleSystem subEmitter, int type, int properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void RemoveSubEmitter(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSubEmitterSystem(ParticleSystem system, int index, ParticleSystem subEmitter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSubEmitterType(ParticleSystem system, int index, int type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSubEmitterProperties(ParticleSystem system, int index, int properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystem GetSubEmitterSystem(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetSubEmitterType(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetSubEmitterProperties(ParticleSystem system, int index);
	}

	/// <summary>
	///   <para>Script interface for the Texture Sheet Animation module.</para>
	/// </summary>
	public struct TextureSheetAnimationModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Texture Sheet Animation module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Select whether the animated texture information comes from a grid of frames on a single texture, or from a list of Sprite objects.</para>
		/// </summary>
		public ParticleSystemAnimationMode mode
		{
			get
			{
				return GetMode(m_ParticleSystem);
			}
			set
			{
				SetMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Defines the tiling of the texture in the X axis.</para>
		/// </summary>
		public int numTilesX
		{
			get
			{
				return GetNumTilesX(m_ParticleSystem);
			}
			set
			{
				SetNumTilesX(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Defines the tiling of the texture in the Y axis.</para>
		/// </summary>
		public int numTilesY
		{
			get
			{
				return GetNumTilesY(m_ParticleSystem);
			}
			set
			{
				SetNumTilesY(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Specifies the animation type.</para>
		/// </summary>
		public ParticleSystemAnimationType animation
		{
			get
			{
				return GetAnimationType(m_ParticleSystem);
			}
			set
			{
				SetAnimationType(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Use a random row of the texture sheet for each particle emitted.</para>
		/// </summary>
		public bool useRandomRow
		{
			get
			{
				return GetUseRandomRow(m_ParticleSystem);
			}
			set
			{
				SetUseRandomRow(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Curve to control which frame of the texture sheet animation to play.</para>
		/// </summary>
		public MinMaxCurve frameOverTime
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetFrameOverTime(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetFrameOverTime(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Frame over time mutiplier.</para>
		/// </summary>
		public float frameOverTimeMultiplier
		{
			get
			{
				return GetFrameOverTimeMultiplier(m_ParticleSystem);
			}
			set
			{
				SetFrameOverTimeMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define a random starting frame for the texture sheet animation.</para>
		/// </summary>
		public MinMaxCurve startFrame
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetStartFrame(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetStartFrame(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Starting frame multiplier.</para>
		/// </summary>
		public float startFrameMultiplier
		{
			get
			{
				return GetStartFrameMultiplier(m_ParticleSystem);
			}
			set
			{
				SetStartFrameMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Specifies how many times the animation will loop during the lifetime of the particle.</para>
		/// </summary>
		public int cycleCount
		{
			get
			{
				return GetCycleCount(m_ParticleSystem);
			}
			set
			{
				SetCycleCount(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Explicitly select which row of the texture sheet is used, when ParticleSystem.TextureSheetAnimationModule.useRandomRow is set to false.</para>
		/// </summary>
		public int rowIndex
		{
			get
			{
				return GetRowIndex(m_ParticleSystem);
			}
			set
			{
				SetRowIndex(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose which UV channels will receive texture animation.</para>
		/// </summary>
		public UVChannelFlags uvChannelMask
		{
			get
			{
				return (UVChannelFlags)GetUVChannelMask(m_ParticleSystem);
			}
			set
			{
				SetUVChannelMask(m_ParticleSystem, (int)value);
			}
		}

		/// <summary>
		///   <para>Flip the U coordinate on particles, causing them to appear mirrored horizontally.</para>
		/// </summary>
		public float flipU
		{
			get
			{
				return GetFlipU(m_ParticleSystem);
			}
			set
			{
				SetFlipU(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Flip the V coordinate on particles, causing them to appear mirrored vertically.</para>
		/// </summary>
		public float flipV
		{
			get
			{
				return GetFlipV(m_ParticleSystem);
			}
			set
			{
				SetFlipV(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The total number of sprites.</para>
		/// </summary>
		public int spriteCount => GetSpriteCount(m_ParticleSystem);

		internal TextureSheetAnimationModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		/// <summary>
		///   <para>Add a new Sprite.</para>
		/// </summary>
		/// <param name="sprite">The Sprite to be added.</param>
		public void AddSprite(Sprite sprite)
		{
			AddSprite(m_ParticleSystem, sprite);
		}

		/// <summary>
		///   <para>Remove a Sprite from the given index in the array.</para>
		/// </summary>
		/// <param name="index">The index from which to remove a Sprite.</param>
		public void RemoveSprite(int index)
		{
			RemoveSprite(m_ParticleSystem, index);
		}

		/// <summary>
		///   <para>Set the Sprite at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the Sprite being modified.</param>
		/// <param name="sprite">The Sprite being assigned.</param>
		public void SetSprite(int index, Sprite sprite)
		{
			SetSprite(m_ParticleSystem, index, sprite);
		}

		/// <summary>
		///   <para>Get the Sprite at the given index.</para>
		/// </summary>
		/// <param name="index">The index of the desired Sprite.</param>
		/// <returns>
		///   <para>The Sprite being requested.</para>
		/// </returns>
		public Sprite GetSprite(int index)
		{
			return GetSprite(m_ParticleSystem, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMode(ParticleSystem system, ParticleSystemAnimationMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemAnimationMode GetMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetNumTilesX(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetNumTilesX(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetNumTilesY(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetNumTilesY(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetAnimationType(ParticleSystem system, ParticleSystemAnimationType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemAnimationType GetAnimationType(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUseRandomRow(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUseRandomRow(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetFrameOverTime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetFrameOverTime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetFrameOverTimeMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetFrameOverTimeMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartFrame(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetStartFrame(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartFrameMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetStartFrameMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetCycleCount(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetCycleCount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRowIndex(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetRowIndex(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUVChannelMask(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetUVChannelMask(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetFlipU(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetFlipU(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetFlipV(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetFlipV(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetSpriteCount(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void AddSprite(ParticleSystem system, Object sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void RemoveSprite(ParticleSystem system, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSprite(ParticleSystem system, int index, Object sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern Sprite GetSprite(ParticleSystem system, int index);
	}

	/// <summary>
	///   <para>Access the ParticleSystem Lights Module.</para>
	/// </summary>
	public struct LightsModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Lights module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose what proportion of particles will receive a dynamic light.</para>
		/// </summary>
		public float ratio
		{
			get
			{
				return GetRatio(m_ParticleSystem);
			}
			set
			{
				SetRatio(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Randomly assign lights to new particles based on ParticleSystem.LightsModule.ratio.</para>
		/// </summary>
		public bool useRandomDistribution
		{
			get
			{
				return GetUseRandomDistribution(m_ParticleSystem);
			}
			set
			{
				SetUseRandomDistribution(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Select what Light prefab you want to base your particle lights on.</para>
		/// </summary>
		public Light light
		{
			get
			{
				return GetLightPrefab(m_ParticleSystem);
			}
			set
			{
				SetLightPrefab(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Toggle whether the particle lights will have their color multiplied by the particle color.</para>
		/// </summary>
		public bool useParticleColor
		{
			get
			{
				return GetUseParticleColor(m_ParticleSystem);
			}
			set
			{
				SetUseParticleColor(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Toggle where the particle size will be multiplied by the light range, to determine the final light range.</para>
		/// </summary>
		public bool sizeAffectsRange
		{
			get
			{
				return GetSizeAffectsRange(m_ParticleSystem);
			}
			set
			{
				SetSizeAffectsRange(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Toggle whether the particle alpha gets multiplied by the light intensity, when computing the final light intensity.</para>
		/// </summary>
		public bool alphaAffectsIntensity
		{
			get
			{
				return GetAlphaAffectsIntensity(m_ParticleSystem);
			}
			set
			{
				SetAlphaAffectsIntensity(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define a curve to apply custom range scaling to particle lights.</para>
		/// </summary>
		public MinMaxCurve range
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetRange(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetRange(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Range multiplier.</para>
		/// </summary>
		public float rangeMultiplier
		{
			get
			{
				return GetRangeMultiplier(m_ParticleSystem);
			}
			set
			{
				SetRangeMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Define a curve to apply custom intensity scaling to particle lights.</para>
		/// </summary>
		public MinMaxCurve intensity
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetIntensity(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetIntensity(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Intensity multiplier.</para>
		/// </summary>
		public float intensityMultiplier
		{
			get
			{
				return GetIntensityMultiplier(m_ParticleSystem);
			}
			set
			{
				SetIntensityMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set a limit on how many lights this Module can create.</para>
		/// </summary>
		public int maxLights
		{
			get
			{
				return GetMaxLights(m_ParticleSystem);
			}
			set
			{
				SetMaxLights(m_ParticleSystem, value);
			}
		}

		internal LightsModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRatio(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRatio(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUseRandomDistribution(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUseRandomDistribution(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetLightPrefab(ParticleSystem system, Light value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern Light GetLightPrefab(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetUseParticleColor(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetUseParticleColor(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSizeAffectsRange(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSizeAffectsRange(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetAlphaAffectsIntensity(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetAlphaAffectsIntensity(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRange(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetRange(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRangeMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRangeMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetIntensity(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetIntensity(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetIntensityMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetIntensityMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMaxLights(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetMaxLights(ParticleSystem system);
	}

	/// <summary>
	///   <para>Access the particle system trails module.</para>
	/// </summary>
	public struct TrailModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Trail module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose how particle trails are generated.</para>
		/// </summary>
		public ParticleSystemTrailMode mode
		{
			get
			{
				return GetMode(m_ParticleSystem);
			}
			set
			{
				SetMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose what proportion of particles will receive a trail.</para>
		/// </summary>
		public float ratio
		{
			get
			{
				return GetRatio(m_ParticleSystem);
			}
			set
			{
				SetRatio(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The curve describing the trail lifetime, throughout the lifetime of the particle.</para>
		/// </summary>
		public MinMaxCurve lifetime
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetLifetime(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetLifetime(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the lifetime multiplier.</para>
		/// </summary>
		public float lifetimeMultiplier
		{
			get
			{
				return GetLifetimeMultiplier(m_ParticleSystem);
			}
			set
			{
				SetLifetimeMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set the minimum distance each trail can travel before a new vertex is added to it.</para>
		/// </summary>
		public float minVertexDistance
		{
			get
			{
				return GetMinVertexDistance(m_ParticleSystem);
			}
			set
			{
				SetMinVertexDistance(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Choose whether the U coordinate of the trail texture is tiled or stretched.</para>
		/// </summary>
		public ParticleSystemTrailTextureMode textureMode
		{
			get
			{
				return GetTextureMode(m_ParticleSystem);
			}
			set
			{
				SetTextureMode(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Drop new trail points in world space, regardless of Particle System Simulation Space.</para>
		/// </summary>
		public bool worldSpace
		{
			get
			{
				return GetWorldSpace(m_ParticleSystem);
			}
			set
			{
				SetWorldSpace(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>If enabled, Trails will disappear immediately when their owning particle dies. Otherwise, the trail will persist until all its points have naturally expired, based on its lifetime.</para>
		/// </summary>
		public bool dieWithParticles
		{
			get
			{
				return GetDieWithParticles(m_ParticleSystem);
			}
			set
			{
				SetDieWithParticles(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set whether the particle size will act as a multiplier on top of the trail width.</para>
		/// </summary>
		public bool sizeAffectsWidth
		{
			get
			{
				return GetSizeAffectsWidth(m_ParticleSystem);
			}
			set
			{
				SetSizeAffectsWidth(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Set whether the particle size will act as a multiplier on top of the trail lifetime.</para>
		/// </summary>
		public bool sizeAffectsLifetime
		{
			get
			{
				return GetSizeAffectsLifetime(m_ParticleSystem);
			}
			set
			{
				SetSizeAffectsLifetime(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Toggle whether the trail will inherit the particle color as its starting color.</para>
		/// </summary>
		public bool inheritParticleColor
		{
			get
			{
				return GetInheritParticleColor(m_ParticleSystem);
			}
			set
			{
				SetInheritParticleColor(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The gradient controlling the trail colors during the lifetime of the attached particle.</para>
		/// </summary>
		public MinMaxGradient colorOverLifetime
		{
			get
			{
				MinMaxGradient gradient = default(MinMaxGradient);
				GetColorOverLifetime(m_ParticleSystem, ref gradient);
				return gradient;
			}
			set
			{
				SetColorOverLifetime(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>The curve describing the width, of each trail point.</para>
		/// </summary>
		public MinMaxCurve widthOverTrail
		{
			get
			{
				MinMaxCurve curve = default(MinMaxCurve);
				GetWidthOverTrail(m_ParticleSystem, ref curve);
				return curve;
			}
			set
			{
				SetWidthOverTrail(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Change the width multiplier.</para>
		/// </summary>
		public float widthOverTrailMultiplier
		{
			get
			{
				return GetWidthOverTrailMultiplier(m_ParticleSystem);
			}
			set
			{
				SetWidthOverTrailMultiplier(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>The gradient controlling the trail colors over the length of the trail.</para>
		/// </summary>
		public MinMaxGradient colorOverTrail
		{
			get
			{
				MinMaxGradient gradient = default(MinMaxGradient);
				GetColorOverTrail(m_ParticleSystem, ref gradient);
				return gradient;
			}
			set
			{
				SetColorOverTrail(m_ParticleSystem, ref value);
			}
		}

		/// <summary>
		///   <para>Configures the trails to generate Normals and Tangents. With this data, Scene lighting can affect the trails via Normal Maps and the Unity Standard Shader, or your own custom-built Shaders.</para>
		/// </summary>
		public bool generateLightingData
		{
			get
			{
				return GetGenerateLightingData(m_ParticleSystem);
			}
			set
			{
				SetGenerateLightingData(m_ParticleSystem, value);
			}
		}

		/// <summary>
		///   <para>Select how many lines to create through the Particle System.</para>
		/// </summary>
		public int ribbonCount
		{
			get
			{
				return GetRibbonCount(m_ParticleSystem);
			}
			set
			{
				SetRibbonCount(m_ParticleSystem, value);
			}
		}

		internal TrailModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMode(ParticleSystem system, ParticleSystemTrailMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemTrailMode GetMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRatio(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetRatio(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetLifetime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetLifetime(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetLifetimeMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetLifetimeMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMinVertexDistance(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetMinVertexDistance(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetTextureMode(ParticleSystem system, ParticleSystemTrailTextureMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemTrailTextureMode GetTextureMode(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetWorldSpace(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetWorldSpace(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetDieWithParticles(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetDieWithParticles(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSizeAffectsWidth(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSizeAffectsWidth(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSizeAffectsLifetime(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetSizeAffectsLifetime(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetInheritParticleColor(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetInheritParticleColor(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetColorOverLifetime(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetColorOverLifetime(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetWidthOverTrail(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetWidthOverTrail(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetWidthOverTrailMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetWidthOverTrailMultiplier(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetColorOverTrail(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetColorOverTrail(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetGenerateLightingData(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetGenerateLightingData(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRibbonCount(ParticleSystem system, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetRibbonCount(ParticleSystem system);
	}

	/// <summary>
	///   <para>Script interface for the Custom Data module.</para>
	/// </summary>
	public struct CustomDataModule
	{
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		///   <para>Enable/disable the Custom Data module.</para>
		/// </summary>
		public bool enabled
		{
			get
			{
				return GetEnabled(m_ParticleSystem);
			}
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		internal CustomDataModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		/// <summary>
		///   <para>Choose the type of custom data to generate for the chosen data stream.</para>
		/// </summary>
		/// <param name="stream">The name of the custom data stream to enable data generation on.</param>
		/// <param name="mode">The type of data to generate.</param>
		public void SetMode(ParticleSystemCustomData stream, ParticleSystemCustomDataMode mode)
		{
			SetMode(m_ParticleSystem, (int)stream, mode);
		}

		/// <summary>
		///   <para>Find out the type of custom data that is being generated for the chosen data stream.</para>
		/// </summary>
		/// <param name="stream">The name of the custom data stream to query.</param>
		/// <returns>
		///   <para>The type of data being generated for the requested stream.</para>
		/// </returns>
		public ParticleSystemCustomDataMode GetMode(ParticleSystemCustomData stream)
		{
			return GetMode(m_ParticleSystem, (int)stream);
		}

		/// <summary>
		///   <para>Specify how many curves are used to generate custom data for this stream.</para>
		/// </summary>
		/// <param name="stream">The name of the custom data stream to apply the curve to.</param>
		/// <param name="curveCount">The number of curves to generate data for.</param>
		/// <param name="count"></param>
		public void SetVectorComponentCount(ParticleSystemCustomData stream, int count)
		{
			SetVectorComponentCount(m_ParticleSystem, (int)stream, count);
		}

		/// <summary>
		///   <para>Query how many ParticleSystem.MinMaxCurve elements are being used to generate this stream of custom data.</para>
		/// </summary>
		/// <param name="stream">The name of the custom data stream to retrieve the curve from.</param>
		/// <returns>
		///   <para>The number of curves.</para>
		/// </returns>
		public int GetVectorComponentCount(ParticleSystemCustomData stream)
		{
			return GetVectorComponentCount(m_ParticleSystem, (int)stream);
		}

		public void SetVector(ParticleSystemCustomData stream, int component, MinMaxCurve curve)
		{
			SetVector(m_ParticleSystem, (int)stream, component, ref curve);
		}

		/// <summary>
		///   <para>Get a ParticleSystem.MinMaxCurve, that is being used to generate custom data.</para>
		/// </summary>
		/// <param name="stream">The name of the custom data stream to retrieve the curve from.</param>
		/// <param name="component">The component index to retrieve the curve for (0-3, mapping to the xyzw components of a Vector4 or float4).</param>
		/// <returns>
		///   <para>The curve being used to generate custom data.</para>
		/// </returns>
		public MinMaxCurve GetVector(ParticleSystemCustomData stream, int component)
		{
			MinMaxCurve curve = default(MinMaxCurve);
			GetVector(m_ParticleSystem, (int)stream, component, ref curve);
			return curve;
		}

		public void SetColor(ParticleSystemCustomData stream, MinMaxGradient gradient)
		{
			SetColor(m_ParticleSystem, (int)stream, ref gradient);
		}

		/// <summary>
		///   <para>Get a ParticleSystem.MinMaxGradient, that is being used to generate custom HDR color data.</para>
		/// </summary>
		/// <param name="stream">The name of the custom data stream to retrieve the gradient from.</param>
		/// <returns>
		///   <para>The color gradient being used to generate custom color data.</para>
		/// </returns>
		public MinMaxGradient GetColor(ParticleSystemCustomData stream)
		{
			MinMaxGradient gradient = default(MinMaxGradient);
			GetColor(m_ParticleSystem, (int)stream, ref gradient);
			return gradient;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern bool GetEnabled(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetMode(ParticleSystem system, int stream, ParticleSystemCustomDataMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetVectorComponentCount(ParticleSystem system, int stream, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetVector(ParticleSystem system, int stream, int component, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetColor(ParticleSystem system, int stream, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern ParticleSystemCustomDataMode GetMode(ParticleSystem system, int stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern int GetVectorComponentCount(ParticleSystem system, int stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetVector(ParticleSystem system, int stream, int component, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void GetColor(ParticleSystem system, int stream, ref MinMaxGradient gradient);
	}

	/// <summary>
	///   <para>Script interface for a Particle.</para>
	/// </summary>
	[RequiredByNativeCode("particleSystemParticle", Optional = true)]
	public struct Particle
	{
		private Vector3 m_Position;

		private Vector3 m_Velocity;

		private Vector3 m_AnimatedVelocity;

		private Vector3 m_InitialVelocity;

		private Vector3 m_AxisOfRotation;

		private Vector3 m_Rotation;

		private Vector3 m_AngularVelocity;

		private Vector3 m_StartSize;

		private Color32 m_StartColor;

		private uint m_RandomSeed;

		private float m_Lifetime;

		private float m_StartLifetime;

		private float m_EmitAccumulator0;

		private float m_EmitAccumulator1;

		/// <summary>
		///   <para>The position of the particle.</para>
		/// </summary>
		public Vector3 position
		{
			get
			{
				return m_Position;
			}
			set
			{
				m_Position = value;
			}
		}

		/// <summary>
		///   <para>The velocity of the particle.</para>
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				return m_Velocity;
			}
			set
			{
				m_Velocity = value;
			}
		}

		/// <summary>
		///   <para>The animated velocity of the particle.</para>
		/// </summary>
		public Vector3 animatedVelocity => m_AnimatedVelocity;

		/// <summary>
		///   <para>The total velocity of the particle.</para>
		/// </summary>
		public Vector3 totalVelocity => m_Velocity + m_AnimatedVelocity;

		/// <summary>
		///   <para>The remaining lifetime of the particle.</para>
		/// </summary>
		public float remainingLifetime
		{
			get
			{
				return m_Lifetime;
			}
			set
			{
				m_Lifetime = value;
			}
		}

		/// <summary>
		///   <para>The starting lifetime of the particle.</para>
		/// </summary>
		public float startLifetime
		{
			get
			{
				return m_StartLifetime;
			}
			set
			{
				m_StartLifetime = value;
			}
		}

		/// <summary>
		///   <para>The initial size of the particle. The current size of the particle is calculated procedurally based on this value and the active size modules.</para>
		/// </summary>
		public float startSize
		{
			get
			{
				return m_StartSize.x;
			}
			set
			{
				m_StartSize = new Vector3(value, value, value);
			}
		}

		/// <summary>
		///   <para>The initial 3D size of the particle. The current size of the particle is calculated procedurally based on this value and the active size modules.</para>
		/// </summary>
		public Vector3 startSize3D
		{
			get
			{
				return m_StartSize;
			}
			set
			{
				m_StartSize = value;
			}
		}

		public Vector3 axisOfRotation
		{
			get
			{
				return m_AxisOfRotation;
			}
			set
			{
				m_AxisOfRotation = value;
			}
		}

		/// <summary>
		///   <para>The rotation of the particle.</para>
		/// </summary>
		public float rotation
		{
			get
			{
				return m_Rotation.z * 57.29578f;
			}
			set
			{
				m_Rotation = new Vector3(0f, 0f, value * ((float)Math.PI / 180f));
			}
		}

		/// <summary>
		///   <para>The 3D rotation of the particle.</para>
		/// </summary>
		public Vector3 rotation3D
		{
			get
			{
				return m_Rotation * 57.29578f;
			}
			set
			{
				m_Rotation = value * ((float)Math.PI / 180f);
			}
		}

		/// <summary>
		///   <para>The angular velocity of the particle.</para>
		/// </summary>
		public float angularVelocity
		{
			get
			{
				return m_AngularVelocity.z * 57.29578f;
			}
			set
			{
				m_AngularVelocity.z = value * ((float)Math.PI / 180f);
			}
		}

		/// <summary>
		///   <para>The 3D angular velocity of the particle.</para>
		/// </summary>
		public Vector3 angularVelocity3D
		{
			get
			{
				return m_AngularVelocity * 57.29578f;
			}
			set
			{
				m_AngularVelocity = value * ((float)Math.PI / 180f);
			}
		}

		/// <summary>
		///   <para>The initial color of the particle. The current color of the particle is calculated procedurally based on this value and the active color modules.</para>
		/// </summary>
		public Color32 startColor
		{
			get
			{
				return m_StartColor;
			}
			set
			{
				m_StartColor = value;
			}
		}

		/// <summary>
		///   <para>The random seed of the particle.</para>
		/// </summary>
		public uint randomSeed
		{
			get
			{
				return m_RandomSeed;
			}
			set
			{
				m_RandomSeed = value;
			}
		}

		/// <summary>
		///   <para>The lifetime of the particle.</para>
		/// </summary>
		[Obsolete("Please use Particle.remainingLifetime instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/Particle.remainingLifetime", false)]
		public float lifetime
		{
			get
			{
				return remainingLifetime;
			}
			set
			{
				remainingLifetime = value;
			}
		}

		/// <summary>
		///   <para>The random value of the particle.</para>
		/// </summary>
		[Obsolete("randomValue property is deprecated.Use randomSeed instead to control random behavior of particles.", false)]
		public float randomValue
		{
			get
			{
				return BitConverter.ToSingle(BitConverter.GetBytes(m_RandomSeed), 0);
			}
			set
			{
				m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
			}
		}

		[Obsolete("size property is deprecated.Use startSize or GetCurrentSize() instead.", false)]
		public float size
		{
			get
			{
				return startSize;
			}
			set
			{
				startSize = value;
			}
		}

		[Obsolete("color property is deprecated.Use startColor or GetCurrentColor() instead.", false)]
		public Color32 color
		{
			get
			{
				return startColor;
			}
			set
			{
				startColor = value;
			}
		}

		/// <summary>
		///   <para>Calculate the current size of the particle by applying the relevant curves to its startSize property.</para>
		/// </summary>
		/// <param name="system">The particle system from which this particle was emitted.</param>
		/// <returns>
		///   <para>Current size.</para>
		/// </returns>
		public float GetCurrentSize(ParticleSystem system)
		{
			return GetCurrentSize(system, ref this);
		}

		/// <summary>
		///   <para>Calculate the current 3D size of the particle by applying the relevant curves to its startSize3D property.</para>
		/// </summary>
		/// <param name="system">The particle system from which this particle was emitted.</param>
		/// <returns>
		///   <para>Current size.</para>
		/// </returns>
		public Vector3 GetCurrentSize3D(ParticleSystem system)
		{
			return GetCurrentSize3D(system, ref this);
		}

		/// <summary>
		///   <para>Calculate the current color of the particle by applying the relevant curves to its startColor property.</para>
		/// </summary>
		/// <param name="system">The particle system from which this particle was emitted.</param>
		/// <returns>
		///   <para>Current color.</para>
		/// </returns>
		public Color32 GetCurrentColor(ParticleSystem system)
		{
			return GetCurrentColor(system, ref this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetCurrentSize(ParticleSystem system, ref Particle particle);

		private static Vector3 GetCurrentSize3D(ParticleSystem system, ref Particle particle)
		{
			INTERNAL_CALL_GetCurrentSize3D(system, ref particle, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetCurrentSize3D(ParticleSystem system, ref Particle particle, out Vector3 value);

		private static Color32 GetCurrentColor(ParticleSystem system, ref Particle particle)
		{
			INTERNAL_CALL_GetCurrentColor(system, ref particle, out var value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void INTERNAL_CALL_GetCurrentColor(ParticleSystem system, ref Particle particle, out Color32 value);
	}

	/// <summary>
	///   <para>Script interface for particle emission parameters.</para>
	/// </summary>
	public struct EmitParams
	{
		internal Particle m_Particle;

		internal bool m_PositionSet;

		internal bool m_VelocitySet;

		internal bool m_AxisOfRotationSet;

		internal bool m_RotationSet;

		internal bool m_AngularVelocitySet;

		internal bool m_StartSizeSet;

		internal bool m_StartColorSet;

		internal bool m_RandomSeedSet;

		internal bool m_StartLifetimeSet;

		internal bool m_ApplyShapeToPosition;

		/// <summary>
		///   <para>Override the position of emitted particles.</para>
		/// </summary>
		public Vector3 position
		{
			get
			{
				return m_Particle.position;
			}
			set
			{
				m_Particle.position = value;
				m_PositionSet = true;
			}
		}

		/// <summary>
		///   <para>When overriding the position of particles, setting this flag to true allows you to retain the influence of the shape module.</para>
		/// </summary>
		public bool applyShapeToPosition
		{
			get
			{
				return m_ApplyShapeToPosition;
			}
			set
			{
				m_ApplyShapeToPosition = value;
			}
		}

		/// <summary>
		///   <para>Override the velocity of emitted particles.</para>
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				return m_Particle.velocity;
			}
			set
			{
				m_Particle.velocity = value;
				m_VelocitySet = true;
			}
		}

		/// <summary>
		///   <para>Override the lifetime of emitted particles.</para>
		/// </summary>
		public float startLifetime
		{
			get
			{
				return m_Particle.startLifetime;
			}
			set
			{
				m_Particle.startLifetime = value;
				m_StartLifetimeSet = true;
			}
		}

		/// <summary>
		///   <para>Override the initial size of emitted particles.</para>
		/// </summary>
		public float startSize
		{
			get
			{
				return m_Particle.startSize;
			}
			set
			{
				m_Particle.startSize = value;
				m_StartSizeSet = true;
			}
		}

		/// <summary>
		///   <para>Override the initial 3D size of emitted particles.</para>
		/// </summary>
		public Vector3 startSize3D
		{
			get
			{
				return m_Particle.startSize3D;
			}
			set
			{
				m_Particle.startSize3D = value;
				m_StartSizeSet = true;
			}
		}

		/// <summary>
		///   <para>Override the axis of rotation of emitted particles.</para>
		/// </summary>
		public Vector3 axisOfRotation
		{
			get
			{
				return m_Particle.axisOfRotation;
			}
			set
			{
				m_Particle.axisOfRotation = value;
				m_AxisOfRotationSet = true;
			}
		}

		/// <summary>
		///   <para>Override the rotation of emitted particles.</para>
		/// </summary>
		public float rotation
		{
			get
			{
				return m_Particle.rotation;
			}
			set
			{
				m_Particle.rotation = value;
				m_RotationSet = true;
			}
		}

		/// <summary>
		///   <para>Override the 3D rotation of emitted particles.</para>
		/// </summary>
		public Vector3 rotation3D
		{
			get
			{
				return m_Particle.rotation3D;
			}
			set
			{
				m_Particle.rotation3D = value;
				m_RotationSet = true;
			}
		}

		/// <summary>
		///   <para>Override the angular velocity of emitted particles.</para>
		/// </summary>
		public float angularVelocity
		{
			get
			{
				return m_Particle.angularVelocity;
			}
			set
			{
				m_Particle.angularVelocity = value;
				m_AngularVelocitySet = true;
			}
		}

		/// <summary>
		///   <para>Override the 3D angular velocity of emitted particles.</para>
		/// </summary>
		public Vector3 angularVelocity3D
		{
			get
			{
				return m_Particle.angularVelocity3D;
			}
			set
			{
				m_Particle.angularVelocity3D = value;
				m_AngularVelocitySet = true;
			}
		}

		/// <summary>
		///   <para>Override the initial color of emitted particles.</para>
		/// </summary>
		public Color32 startColor
		{
			get
			{
				return m_Particle.startColor;
			}
			set
			{
				m_Particle.startColor = value;
				m_StartColorSet = true;
			}
		}

		/// <summary>
		///   <para>Override the random seed of emitted particles.</para>
		/// </summary>
		public uint randomSeed
		{
			get
			{
				return m_Particle.randomSeed;
			}
			set
			{
				m_Particle.randomSeed = value;
				m_RandomSeedSet = true;
			}
		}

		/// <summary>
		///   <para>Revert the position back to the value specified in the inspector.</para>
		/// </summary>
		public void ResetPosition()
		{
			m_PositionSet = false;
		}

		/// <summary>
		///   <para>Revert the velocity back to the value specified in the inspector.</para>
		/// </summary>
		public void ResetVelocity()
		{
			m_VelocitySet = false;
		}

		/// <summary>
		///   <para>Revert the axis of rotation back to the value specified in the inspector.</para>
		/// </summary>
		public void ResetAxisOfRotation()
		{
			m_AxisOfRotationSet = false;
		}

		/// <summary>
		///   <para>Reverts rotation and rotation3D back to the values specified in the inspector.</para>
		/// </summary>
		public void ResetRotation()
		{
			m_RotationSet = false;
		}

		/// <summary>
		///   <para>Reverts angularVelocity and angularVelocity3D back to the values specified in the inspector.</para>
		/// </summary>
		public void ResetAngularVelocity()
		{
			m_AngularVelocitySet = false;
		}

		/// <summary>
		///   <para>Revert the initial size back to the value specified in the inspector.</para>
		/// </summary>
		public void ResetStartSize()
		{
			m_StartSizeSet = false;
		}

		/// <summary>
		///   <para>Revert the initial color back to the value specified in the inspector.</para>
		/// </summary>
		public void ResetStartColor()
		{
			m_StartColorSet = false;
		}

		/// <summary>
		///   <para>Revert the random seed back to the value specified in the inspector.</para>
		/// </summary>
		public void ResetRandomSeed()
		{
			m_RandomSeedSet = false;
		}

		/// <summary>
		///   <para>Revert the lifetime back to the value specified in the inspector.</para>
		/// </summary>
		public void ResetStartLifetime()
		{
			m_StartLifetimeSet = false;
		}
	}

	/// <summary>
	///   <para>Is the Particle System playing right now?</para>
	/// </summary>
	public extern bool isPlaying
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Is the Particle System currently emitting particles? A Particle System may stop emitting when its emission module has finished, it has been paused or if the system has been stopped using ParticleSystem.Stop|Stop with the ParticleSystemStopBehavior.StopEmitting|StopEmitting flag. Resume emitting by calling ParticleSystem.Play|Play.</para>
	/// </summary>
	public extern bool isEmitting
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Is the Particle System stopped right now?</para>
	/// </summary>
	public extern bool isStopped
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Is the Particle System paused right now?</para>
	/// </summary>
	public extern bool isPaused
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Playback position in seconds.</para>
	/// </summary>
	public extern float time
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The current number of particles (Read Only).</para>
	/// </summary>
	public extern int particleCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Override the random seed used for the particle system emission.</para>
	/// </summary>
	public extern uint randomSeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Controls whether the Particle System uses an automatically-generated random number to seed the random number generator.</para>
	/// </summary>
	public extern bool useAutoRandomSeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Does this system support Automatic Culling?</para>
	/// </summary>
	public extern bool automaticCullingEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Access the main particle system settings.</para>
	/// </summary>
	public MainModule main => new MainModule(this);

	/// <summary>
	///   <para>Access the particle system emission module.</para>
	/// </summary>
	public EmissionModule emission => new EmissionModule(this);

	/// <summary>
	///   <para>Access the particle system shape module.</para>
	/// </summary>
	public ShapeModule shape => new ShapeModule(this);

	/// <summary>
	///   <para>Access the particle system velocity over lifetime module.</para>
	/// </summary>
	public VelocityOverLifetimeModule velocityOverLifetime => new VelocityOverLifetimeModule(this);

	/// <summary>
	///   <para>Access the particle system limit velocity over lifetime module.</para>
	/// </summary>
	public LimitVelocityOverLifetimeModule limitVelocityOverLifetime => new LimitVelocityOverLifetimeModule(this);

	/// <summary>
	///   <para>Access the particle system velocity inheritance module.</para>
	/// </summary>
	public InheritVelocityModule inheritVelocity => new InheritVelocityModule(this);

	/// <summary>
	///   <para>Access the particle system force over lifetime module.</para>
	/// </summary>
	public ForceOverLifetimeModule forceOverLifetime => new ForceOverLifetimeModule(this);

	/// <summary>
	///   <para>Access the particle system color over lifetime module.</para>
	/// </summary>
	public ColorOverLifetimeModule colorOverLifetime => new ColorOverLifetimeModule(this);

	/// <summary>
	///   <para>Access the particle system color by lifetime module.</para>
	/// </summary>
	public ColorBySpeedModule colorBySpeed => new ColorBySpeedModule(this);

	/// <summary>
	///   <para>Access the particle system size over lifetime module.</para>
	/// </summary>
	public SizeOverLifetimeModule sizeOverLifetime => new SizeOverLifetimeModule(this);

	/// <summary>
	///   <para>Access the particle system size by speed module.</para>
	/// </summary>
	public SizeBySpeedModule sizeBySpeed => new SizeBySpeedModule(this);

	/// <summary>
	///   <para>Access the particle system rotation over lifetime module.</para>
	/// </summary>
	public RotationOverLifetimeModule rotationOverLifetime => new RotationOverLifetimeModule(this);

	/// <summary>
	///   <para>Access the particle system rotation by speed  module.</para>
	/// </summary>
	public RotationBySpeedModule rotationBySpeed => new RotationBySpeedModule(this);

	/// <summary>
	///   <para>Access the particle system external forces module.</para>
	/// </summary>
	public ExternalForcesModule externalForces => new ExternalForcesModule(this);

	/// <summary>
	///   <para>Access the particle system noise module.</para>
	/// </summary>
	public NoiseModule noise => new NoiseModule(this);

	/// <summary>
	///   <para>Access the particle system collision module.</para>
	/// </summary>
	public CollisionModule collision => new CollisionModule(this);

	/// <summary>
	///   <para>Access the particle system trigger module.</para>
	/// </summary>
	public TriggerModule trigger => new TriggerModule(this);

	/// <summary>
	///   <para>Access the particle system sub emitters module.</para>
	/// </summary>
	public SubEmittersModule subEmitters => new SubEmittersModule(this);

	/// <summary>
	///   <para>Access the particle system texture sheet animation module.</para>
	/// </summary>
	public TextureSheetAnimationModule textureSheetAnimation => new TextureSheetAnimationModule(this);

	/// <summary>
	///   <para>Access the particle system lights module.</para>
	/// </summary>
	public LightsModule lights => new LightsModule(this);

	/// <summary>
	///   <para>Access the particle system trails module.</para>
	/// </summary>
	public TrailModule trails => new TrailModule(this);

	/// <summary>
	///   <para>Access the particle system Custom Data module.</para>
	/// </summary>
	public CustomDataModule customData => new CustomDataModule(this);

	/// <summary>
	///   <para>Start delay in seconds.</para>
	/// </summary>
	[Obsolete("startDelay property is deprecated.Use main.startDelay or main.startDelayMultiplier instead.", false)]
	public float startDelay
	{
		get
		{
			return main.startDelayMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startDelayMultiplier = value;
		}
	}

	/// <summary>
	///   <para>Is the particle system looping?</para>
	/// </summary>
	[Obsolete("loop property is deprecated.Use main.loop instead.", false)]
	public bool loop
	{
		get
		{
			return main.loop;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.loop = value;
		}
	}

	/// <summary>
	///   <para>If set to true, the particle system will automatically start playing on startup.</para>
	/// </summary>
	[Obsolete("playOnAwake property is deprecated.Use main.playOnAwake instead.", false)]
	public bool playOnAwake
	{
		get
		{
			return main.playOnAwake;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.playOnAwake = value;
		}
	}

	/// <summary>
	///   <para>The duration of the particle system in seconds (Read Only).</para>
	/// </summary>
	[Obsolete("duration property is deprecated.Use main.duration instead.", false)]
	public float duration => main.duration;

	/// <summary>
	///   <para>The playback speed of the particle system. 1 is normal playback speed.</para>
	/// </summary>
	[Obsolete("playbackSpeed property is deprecated.Use main.simulationSpeed instead.", false)]
	public float playbackSpeed
	{
		get
		{
			return main.simulationSpeed;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.simulationSpeed = value;
		}
	}

	/// <summary>
	///   <para>When set to false, the particle system will not emit particles.</para>
	/// </summary>
	[Obsolete("enableEmission property is deprecated.Use emission.enabled instead.", false)]
	public bool enableEmission
	{
		get
		{
			return emission.enabled;
		}
		set
		{
			EmissionModule emissionModule = emission;
			emissionModule.enabled = value;
		}
	}

	/// <summary>
	///   <para>The rate of emission.</para>
	/// </summary>
	[Obsolete("emissionRate property is deprecated.Use emission.rateOverTime, emission.rateOverDistance, emission.rateOverTimeMultiplier or emission.rateOverDistanceMultiplier instead.", false)]
	public float emissionRate
	{
		get
		{
			return emission.rateOverTimeMultiplier;
		}
		set
		{
			EmissionModule emissionModule = emission;
			emissionModule.rateOverTime = value;
		}
	}

	/// <summary>
	///   <para>The initial speed of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
	/// </summary>
	[Obsolete("startSpeed property is deprecated.Use main.startSpeed or main.startSpeedMultiplier instead.", false)]
	public float startSpeed
	{
		get
		{
			return main.startSpeedMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startSpeedMultiplier = value;
		}
	}

	/// <summary>
	///   <para>The initial size of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
	/// </summary>
	[Obsolete("startSize property is deprecated.Use main.startSize or main.startSizeMultiplier instead.", false)]
	public float startSize
	{
		get
		{
			return main.startSizeMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startSizeMultiplier = value;
		}
	}

	/// <summary>
	///   <para>The initial color of particles when emitted.</para>
	/// </summary>
	[Obsolete("startColor property is deprecated.Use main.startColor instead.", false)]
	public Color startColor
	{
		get
		{
			return main.startColor.color;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startColor = value;
		}
	}

	/// <summary>
	///   <para>The initial rotation of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
	/// </summary>
	[Obsolete("startRotation property is deprecated.Use main.startRotation or main.startRotationMultiplier instead.", false)]
	public float startRotation
	{
		get
		{
			return main.startRotationMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startRotationMultiplier = value;
		}
	}

	/// <summary>
	///   <para>The initial 3D rotation of particles when emitted. When using curves, this values acts as a scale on the curves.</para>
	/// </summary>
	[Obsolete("startRotation3D property is deprecated.Use main.startRotationX, main.startRotationY and main.startRotationZ instead. (Or main.startRotationXMultiplier, main.startRotationYMultiplier and main.startRotationZMultiplier).", false)]
	public Vector3 startRotation3D
	{
		get
		{
			return new Vector3(main.startRotationXMultiplier, main.startRotationYMultiplier, main.startRotationZMultiplier);
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startRotationXMultiplier = value.x;
			mainModule.startRotationYMultiplier = value.y;
			mainModule.startRotationZMultiplier = value.z;
		}
	}

	/// <summary>
	///   <para>The total lifetime in seconds that particles will have when emitted. When using curves, this values acts as a scale on the curve. This value is set in the particle when it is created by the particle system.</para>
	/// </summary>
	[Obsolete("startLifetime property is deprecated.Use main.startLifetime or main.startLifetimeMultiplier instead.", false)]
	public float startLifetime
	{
		get
		{
			return main.startLifetimeMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startLifetimeMultiplier = value;
		}
	}

	/// <summary>
	///   <para>Scale being applied to the gravity defined by Physics.gravity.</para>
	/// </summary>
	[Obsolete("gravityModifier property is deprecated.Use main.gravityModifier or main.gravityModifierMultiplier instead.", false)]
	public float gravityModifier
	{
		get
		{
			return main.gravityModifierMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.gravityModifierMultiplier = value;
		}
	}

	/// <summary>
	///   <para>The maximum number of particles to emit.</para>
	/// </summary>
	[Obsolete("maxParticles property is deprecated.Use main.maxParticles instead.", false)]
	public int maxParticles
	{
		get
		{
			return main.maxParticles;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.maxParticles = value;
		}
	}

	/// <summary>
	///   <para>This selects the space in which to simulate particles. It can be either world or local space.</para>
	/// </summary>
	[Obsolete("simulationSpace property is deprecated.Use main.simulationSpace instead.", false)]
	public ParticleSystemSimulationSpace simulationSpace
	{
		get
		{
			return main.simulationSpace;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.simulationSpace = value;
		}
	}

	/// <summary>
	///   <para>The scaling mode applied to particle sizes and positions.</para>
	/// </summary>
	[Obsolete("scalingMode property is deprecated.Use main.scalingMode instead.", false)]
	public ParticleSystemScalingMode scalingMode
	{
		get
		{
			return main.scalingMode;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.scalingMode = value;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetParticles(Particle[] particles, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern int GetParticles(Particle[] particles);

	public void SetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex)
	{
		SetCustomParticleDataInternal(customData, (int)streamIndex);
	}

	public int GetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex)
	{
		return GetCustomParticleDataInternal(customData, (int)streamIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void SetCustomParticleDataInternal(object customData, int streamIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern int GetCustomParticleDataInternal(object customData, int streamIndex);

	/// <summary>
	///   <para>Fastforwards the particle system by simulating particles over given period of time, then pauses it.</para>
	/// </summary>
	/// <param name="t">Time period in seconds to advance the ParticleSystem simulation by. If restart is true, the ParticleSystem will be reset to 0 time, and then advanced by this value. If restart is false, the ParticleSystem simulation will be advanced in time from its current state by this value.</param>
	/// <param name="withChildren">Fastforward all child particle systems as well.</param>
	/// <param name="restart">Restart and start from the beginning.</param>
	/// <param name="fixedTimeStep">Only update the system at fixed intervals, based on the value in "Fixed Time" in the Time options.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep);

	[ExcludeFromDocs]
	public void Simulate(float t, bool withChildren, bool restart)
	{
		bool fixedTimeStep = true;
		Simulate(t, withChildren, restart, fixedTimeStep);
	}

	[ExcludeFromDocs]
	public void Simulate(float t, bool withChildren)
	{
		bool fixedTimeStep = true;
		bool restart = true;
		Simulate(t, withChildren, restart, fixedTimeStep);
	}

	[ExcludeFromDocs]
	public void Simulate(float t)
	{
		bool fixedTimeStep = true;
		bool restart = true;
		bool withChildren = true;
		Simulate(t, withChildren, restart, fixedTimeStep);
	}

	/// <summary>
	///   <para>Starts the particle system.</para>
	/// </summary>
	/// <param name="withChildren">Play all child particle systems as well.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Play([DefaultValue("true")] bool withChildren);

	[ExcludeFromDocs]
	public void Play()
	{
		bool withChildren = true;
		Play(withChildren);
	}

	/// <summary>
	///   <para>Pauses the system so no new particles are emitted and the existing particles are not updated.</para>
	/// </summary>
	/// <param name="withChildren">Pause all child particle systems as well.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Pause([DefaultValue("true")] bool withChildren);

	[ExcludeFromDocs]
	public void Pause()
	{
		bool withChildren = true;
		Pause(withChildren);
	}

	/// <summary>
	///   <para>Stops playing the particle system using the supplied stop behaviour.</para>
	/// </summary>
	/// <param name="withChildren">Stop all child particle systems as well.</param>
	/// <param name="stopBehavior">Stop emitting or stop emitting and clear the system.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Stop([DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior);

	/// <summary>
	///   <para>Stops playing the particle system using the supplied stop behaviour.</para>
	/// </summary>
	/// <param name="withChildren">Stop all child particle systems as well.</param>
	/// <param name="stopBehavior">Stop emitting or stop emitting and clear the system.</param>
	[ExcludeFromDocs]
	public void Stop(bool withChildren)
	{
		ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting;
		Stop(withChildren, stopBehavior);
	}

	[ExcludeFromDocs]
	public void Stop()
	{
		ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting;
		bool withChildren = true;
		Stop(withChildren, stopBehavior);
	}

	/// <summary>
	///   <para>Remove all particles in the particle system.</para>
	/// </summary>
	/// <param name="withChildren">Clear all child particle systems as well.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Clear([DefaultValue("true")] bool withChildren);

	[ExcludeFromDocs]
	public void Clear()
	{
		bool withChildren = true;
		Clear(withChildren);
	}

	/// <summary>
	///   <para>Does the system contain any live particles? (or will produce more).</para>
	/// </summary>
	/// <param name="withChildren">Check all child particle systems as well.</param>
	/// <returns>
	///   <para>True if the particle system is still "alive", false if the particle system is done emitting particles and all particles are dead.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool IsAlive([DefaultValue("true")] bool withChildren);

	[ExcludeFromDocs]
	public bool IsAlive()
	{
		bool withChildren = true;
		return IsAlive(withChildren);
	}

	/// <summary>
	///   <para>Emit count particles immediately.</para>
	/// </summary>
	/// <param name="count">Number of particles to emit.</param>
	public void Emit(int count)
	{
		INTERNAL_CALL_Emit(this, count);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_EmitOld(ref Particle particle);

	public void Emit(EmitParams emitParams, int count)
	{
		Internal_Emit(ref emitParams, count);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_Emit(ref EmitParams emitParams, int count);

	/// <summary>
	///   <para>Triggers the specified sub emitter on all particles of the Particle System.</para>
	/// </summary>
	/// <param name="subEmitterIndex">Index of the sub emitter to trigger.</param>
	public void TriggerSubEmitter(int subEmitterIndex)
	{
		Internal_TriggerSubEmitter(subEmitterIndex, null);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void TriggerSubEmitter(int subEmitterIndex, ref Particle particle);

	public void TriggerSubEmitter(int subEmitterIndex, List<Particle> particles)
	{
		Internal_TriggerSubEmitter(subEmitterIndex, particles);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void Internal_TriggerSubEmitter(int subEmitterIndex, object particles);

	/// <summary>
	///   <para></para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="velocity"></param>
	/// <param name="size"></param>
	/// <param name="lifetime"></param>
	/// <param name="color"></param>
	[Obsolete("Emit with specific parameters is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties", false)]
	public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
	{
		Particle particle = default(Particle);
		particle.position = position;
		particle.velocity = velocity;
		particle.lifetime = lifetime;
		particle.startLifetime = lifetime;
		particle.startSize = size;
		particle.rotation3D = Vector3.zero;
		particle.angularVelocity3D = Vector3.zero;
		particle.startColor = color;
		particle.randomSeed = 5u;
		Internal_EmitOld(ref particle);
	}

	[Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties", false)]
	public void Emit(Particle particle)
	{
		Internal_EmitOld(ref particle);
	}
}
