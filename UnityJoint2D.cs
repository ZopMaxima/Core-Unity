// UnityJoint2D.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 26, 2022

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Selectable enumeration of Unity joints.
	/// </summary>
	public enum UnityJoint2D
	{
		None = 0,
		Fixed = 1 << 0,
		Hinge = 1 << 1,
		Spring = 1 << 2,
		Distance = 1 << 3,
		Relative = 1 << 4,
		Target = 1 << 5,
		Friction = 1 << 6,
		Slider = 1 << 7,
		Wheel = 1 << 8,
	}

	/// <summary>
	/// Selectable enumeration of Unity joints.
	/// </summary>
	[Flags]
	public enum UnityJoint2DMask
	{
		None = 0,
		Fixed = UnityJoint2D.Fixed,
		Hinge = UnityJoint2D.Hinge,
		Spring = UnityJoint2D.Spring,
		Distance = UnityJoint2D.Distance,
		Relative = UnityJoint2D.Relative,
		Target = UnityJoint2D.Target,
		Friction = UnityJoint2D.Friction,
		Slider = UnityJoint2D.Slider,
		Wheel = UnityJoint2D.Wheel,
	}

	/// <summary>
	/// Extends component instantiation.
	/// </summary>
	public static class UnityJoint2DUtil
	{
		private static readonly Joint2D[] EMPTY = new Joint2D[0];
		private static readonly List<Joint2D> _buffer = new List<Joint2D>();
		private static readonly List<FixedJoint2D> _bufferFixed = new List<FixedJoint2D>();
		private static readonly List<HingeJoint2D> _bufferHinge = new List<HingeJoint2D>();
		private static readonly List<SpringJoint2D> _bufferSpring = new List<SpringJoint2D>();
		private static readonly List<DistanceJoint2D> _bufferDistance = new List<DistanceJoint2D>();
		private static readonly List<RelativeJoint2D> _bufferRelative = new List<RelativeJoint2D>();
		private static readonly List<TargetJoint2D> _bufferTarget = new List<TargetJoint2D>();
		private static readonly List<FrictionJoint2D> _bufferFriction = new List<FrictionJoint2D>();
		private static readonly List<SliderJoint2D> _bufferSlider = new List<SliderJoint2D>();
		private static readonly List<WheelJoint2D> _bufferWheel = new List<WheelJoint2D>();

		/// <summary>
		/// Returns a newly attached component of the requested type.
		/// </summary>
		public static Joint2D AddJoint(this Component c, UnityJoint2D type)
		{
			return (c != null) ? (c.gameObject.AddJoint(type)) : (null);
		}

		/// <summary>
		/// Returns a newly attached component of the requested type.
		/// </summary>
		public static Joint2D AddJoint(this GameObject go, UnityJoint2D type)
		{
			// Cannot be null.
			if (go == null)
			{
				return null;
			}

			// Add
			switch (type)
			{
				default:
				case UnityJoint2D.None: return null;
				case UnityJoint2D.Fixed: return go.AddComponent<FixedJoint2D>();
				case UnityJoint2D.Hinge: return go.AddComponent<HingeJoint2D>();
				case UnityJoint2D.Spring: return go.AddComponent<SpringJoint2D>();
				case UnityJoint2D.Distance: return go.AddComponent<DistanceJoint2D>();
				case UnityJoint2D.Relative: return go.AddComponent<RelativeJoint2D>();
				case UnityJoint2D.Target: return go.AddComponent<TargetJoint2D>();
				case UnityJoint2D.Friction: return go.AddComponent<FrictionJoint2D>();
				case UnityJoint2D.Slider: return go.AddComponent<SliderJoint2D>();
				case UnityJoint2D.Wheel: return go.AddComponent<WheelJoint2D>();
			}
		}

		/// <summary>
		/// Returns newly attached components of the requested type.
		/// </summary>
		public static Joint2D[] AddJoints(this Component c, UnityJoint2DMask types)
		{
			return (c != null) ? (c.gameObject.AddJoints(types)) : (null);
		}

		/// <summary>
		/// Returns newly attached components of the requested type.
		/// </summary>
		public static Joint2D[] AddJoints(this GameObject go, UnityJoint2DMask types)
		{
			// Cannot be null.
			if (go == null)
			{
				return EMPTY;
			}

			// Add
			_buffer.Clear();
			if ((types & UnityJoint2DMask.Fixed) != 0)
			{
				_buffer.Add(go.AddComponent<FixedJoint2D>());
			}
			if ((types & UnityJoint2DMask.Hinge) != 0)
			{
				_buffer.Add(go.AddComponent<HingeJoint2D>());
			}
			if ((types & UnityJoint2DMask.Spring) != 0)
			{
				_buffer.Add(go.AddComponent<SpringJoint2D>());
			}
			if ((types & UnityJoint2DMask.Distance) != 0)
			{
				_buffer.Add(go.AddComponent<DistanceJoint2D>());
			}
			if ((types & UnityJoint2DMask.Relative) != 0)
			{
				_buffer.Add(go.AddComponent<RelativeJoint2D>());
			}
			if ((types & UnityJoint2DMask.Target) != 0)
			{
				_buffer.Add(go.AddComponent<TargetJoint2D>());
			}
			if ((types & UnityJoint2DMask.Friction) != 0)
			{
				_buffer.Add(go.AddComponent<FrictionJoint2D>());
			}
			if ((types & UnityJoint2DMask.Slider) != 0)
			{
				_buffer.Add(go.AddComponent<SliderJoint2D>());
			}
			if ((types & UnityJoint2DMask.Wheel) != 0)
			{
				_buffer.Add(go.AddComponent<WheelJoint2D>());
			}

			// Return
			Joint2D[] result = _buffer.ToArray();
			_buffer.Clear();
			return result;
		}

		/// <summary>
		/// Returns a component of the requested type.
		/// </summary>
		public static Joint2D GetJoint(this Component c, UnityJoint2D type)
		{
			return (c != null) ? (c.gameObject.GetJoint(type)) : (null);
		}

		/// <summary>
		/// Returns a component of the requested type.
		/// </summary>
		public static Joint2D GetJoint(this GameObject go, UnityJoint2D type)
		{
			// Cannot be null.
			if (go == null)
			{
				return null;
			}

			// Add
			switch (type)
			{
				default:
				case UnityJoint2D.None: return null;
				case UnityJoint2D.Fixed: return go.GetComponent<FixedJoint2D>();
				case UnityJoint2D.Hinge: return go.GetComponent<HingeJoint2D>();
				case UnityJoint2D.Spring: return go.GetComponent<SpringJoint2D>();
				case UnityJoint2D.Distance: return go.GetComponent<DistanceJoint2D>();
				case UnityJoint2D.Relative: return go.GetComponent<RelativeJoint2D>();
				case UnityJoint2D.Target: return go.GetComponent<TargetJoint2D>();
				case UnityJoint2D.Friction: return go.GetComponent<FrictionJoint2D>();
				case UnityJoint2D.Slider: return go.GetComponent<SliderJoint2D>();
				case UnityJoint2D.Wheel: return go.GetComponent<WheelJoint2D>();
			}
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint2D[] GetJoints(this Component c, UnityJoint2D type)
		{
			return (c != null) ? (c.gameObject.GetJoints(type)) : (EMPTY);
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint2D[] GetJoints(this GameObject go, UnityJoint2D type)
		{
			// Cannot be null.
			if (go == null)
			{
				return EMPTY;
			}

			// Add
			switch (type)
			{
				default:
				case UnityJoint2D.None: return null;
				case UnityJoint2D.Fixed: return go.GetComponents<FixedJoint2D>();
				case UnityJoint2D.Hinge: return go.GetComponents<HingeJoint2D>();
				case UnityJoint2D.Spring: return go.GetComponents<SpringJoint2D>();
				case UnityJoint2D.Distance: return go.GetComponents<DistanceJoint2D>();
				case UnityJoint2D.Relative: return go.GetComponents<RelativeJoint2D>();
				case UnityJoint2D.Target: return go.GetComponents<TargetJoint2D>();
				case UnityJoint2D.Friction: return go.GetComponents<FrictionJoint2D>();
				case UnityJoint2D.Slider: return go.GetComponents<SliderJoint2D>();
				case UnityJoint2D.Wheel: return go.GetComponents<WheelJoint2D>();
			}
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint2D[] GetJoints(this Component c, UnityJoint2DMask types)
		{
			return (c != null) ? (c.gameObject.GetJoints(types)) : (EMPTY);
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint2D[] GetJoints(this GameObject go, UnityJoint2DMask types)
		{
			// Cannot be null.
			if (go == null)
			{
				return EMPTY;
			}

			// Add
			_buffer.Clear();
			if ((types & UnityJoint2DMask.Fixed) != 0)
			{
				_bufferFixed.Clear();
				go.GetComponents(_bufferFixed);
				_buffer.AddRange(_bufferFixed);
				_bufferFixed.Clear();
			}
			if ((types & UnityJoint2DMask.Hinge) != 0)
			{
				_bufferHinge.Clear();
				go.GetComponents(_bufferHinge);
				_buffer.AddRange(_bufferHinge);
				_bufferHinge.Clear();
			}
			if ((types & UnityJoint2DMask.Spring) != 0)
			{
				_bufferSpring.Clear();
				go.GetComponents(_bufferSpring);
				_buffer.AddRange(_bufferSpring);
				_bufferSpring.Clear();
			}
			if ((types & UnityJoint2DMask.Distance) != 0)
			{
				_bufferDistance.Clear();
				go.GetComponents(_bufferDistance);
				_buffer.AddRange(_bufferDistance);
				_bufferDistance.Clear();
			}
			if ((types & UnityJoint2DMask.Relative) != 0)
			{
				_bufferRelative.Clear();
				go.GetComponents(_bufferRelative);
				_buffer.AddRange(_bufferRelative);
				_bufferRelative.Clear();
			}
			if ((types & UnityJoint2DMask.Target) != 0)
			{
				_bufferTarget.Clear();
				go.GetComponents(_bufferTarget);
				_buffer.AddRange(_bufferTarget);
				_bufferTarget.Clear();
			}
			if ((types & UnityJoint2DMask.Friction) != 0)
			{
				_bufferFriction.Clear();
				go.GetComponents(_bufferFriction);
				_buffer.AddRange(_bufferFriction);
				_bufferFriction.Clear();
			}
			if ((types & UnityJoint2DMask.Slider) != 0)
			{
				_bufferSlider.Clear();
				go.GetComponents(_bufferSlider);
				_buffer.AddRange(_bufferSlider);
				_bufferSlider.Clear();
			}
			if ((types & UnityJoint2DMask.Wheel) != 0)
			{
				_bufferWheel.Clear();
				go.GetComponents(_bufferWheel);
				_buffer.AddRange(_bufferWheel);
				_bufferWheel.Clear();
			}

			// Return
			Joint2D[] result = _buffer.ToArray();
			_buffer.Clear();
			return result;
		}
	}
}