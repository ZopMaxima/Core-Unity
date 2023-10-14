// UnityJoint.cs
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
	public enum UnityJoint
	{
		None = 0,
		Fixed = 1 << 0,
		Hinge = 1 << 1,
		Spring = 1 << 2,
		Configurable = 1 << 3,
		Character = 1 << 4,
	}

	/// <summary>
	/// Selectable enumeration of Unity joints.
	/// </summary>
	[Flags]
	public enum UnityJointMask
	{
		None = 0,
		Fixed = UnityJoint.Fixed,
		Hinge = UnityJoint.Hinge,
		Spring = UnityJoint.Spring,
		Configurable = UnityJoint.Configurable,
		Character = UnityJoint.Character,
	}

	/// <summary>
	/// Extends component instantiation.
	/// </summary>
	public static class UnityJointUtil
	{
		private static readonly Joint[] EMPTY = new Joint[0];
		private static readonly List<Joint> _buffer = new List<Joint>();
		private static readonly List<FixedJoint> _bufferFixed = new List<FixedJoint>();
		private static readonly List<HingeJoint> _bufferHinge = new List<HingeJoint>();
		private static readonly List<SpringJoint> _bufferSpring = new List<SpringJoint>();
		private static readonly List<ConfigurableJoint> _bufferConfigurable = new List<ConfigurableJoint>();
		private static readonly List<CharacterJoint> _bufferCharacter = new List<CharacterJoint>();

		/// <summary>
		/// Returns a newly attached component of the requested type.
		/// </summary>
		public static Joint AddJoint(this Component c, UnityJoint type)
		{
			return (c != null) ? (c.gameObject.AddJoint(type)) : (null);
		}

		/// <summary>
		/// Returns a newly attached component of the requested type.
		/// </summary>
		public static Joint AddJoint(this GameObject go, UnityJoint type)
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
				case UnityJoint.None: return null;
				case UnityJoint.Fixed: return go.AddComponent<FixedJoint>();
				case UnityJoint.Hinge: return go.AddComponent<HingeJoint>();
				case UnityJoint.Spring: return go.AddComponent<SpringJoint>();
				case UnityJoint.Configurable: return go.AddComponent<ConfigurableJoint>();
				case UnityJoint.Character: return go.AddComponent<CharacterJoint>();
			}
		}

		/// <summary>
		/// Returns newly attached components of the requested type.
		/// </summary>
		public static Joint[] AddJoints(this Component c, UnityJointMask types)
		{
			return (c != null) ? (c.gameObject.AddJoints(types)) : (null);
		}

		/// <summary>
		/// Returns newly attached components of the requested type.
		/// </summary>
		public static Joint[] AddJoints(this GameObject go, UnityJointMask types)
		{
			// Cannot be null.
			if (go == null)
			{
				return EMPTY;
			}

			// Add
			_buffer.Clear();
			if ((types & UnityJointMask.Fixed) != 0)
			{
				_buffer.Add(go.AddComponent<FixedJoint>());
			}
			if ((types & UnityJointMask.Hinge) != 0)
			{
				_buffer.Add(go.AddComponent<HingeJoint>());
			}
			if ((types & UnityJointMask.Spring) != 0)
			{
				_buffer.Add(go.AddComponent<SpringJoint>());
			}
			if ((types & UnityJointMask.Configurable) != 0)
			{
				_buffer.Add(go.AddComponent<ConfigurableJoint>());
			}
			if ((types & UnityJointMask.Character) != 0)
			{
				_buffer.Add(go.AddComponent<CharacterJoint>());
			}

			// Return
			Joint[] result = _buffer.ToArray();
			_buffer.Clear();
			return result;
		}

		/// <summary>
		/// Returns a component of the requested type.
		/// </summary>
		public static Joint GetJoint(this Component c, UnityJoint type)
		{
			return (c != null) ? (c.gameObject.GetJoint(type)) : (null);
		}

		/// <summary>
		/// Returns a component of the requested type.
		/// </summary>
		public static Joint GetJoint(this GameObject go, UnityJoint type)
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
				case UnityJoint.None: return null;
				case UnityJoint.Fixed: return go.GetComponent<FixedJoint>();
				case UnityJoint.Hinge: return go.GetComponent<HingeJoint>();
				case UnityJoint.Spring: return go.GetComponent<SpringJoint>();
				case UnityJoint.Configurable: return go.GetComponent<ConfigurableJoint>();
				case UnityJoint.Character: return go.GetComponent<CharacterJoint>();
			}
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint[] GetJoints(this Component c, UnityJoint type)
		{
			return (c != null) ? (c.gameObject.GetJoints(type)) : (EMPTY);
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint[] GetJoints(this GameObject go, UnityJoint type)
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
				case UnityJoint.None: return null;
				case UnityJoint.Fixed: return go.GetComponents<FixedJoint>();
				case UnityJoint.Hinge: return go.GetComponents<HingeJoint>();
				case UnityJoint.Spring: return go.GetComponents<SpringJoint>();
				case UnityJoint.Configurable: return go.GetComponents<ConfigurableJoint>();
				case UnityJoint.Character: return go.GetComponents<CharacterJoint>();
			}
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint[] GetJoints(this Component c, UnityJointMask types)
		{
			return (c != null) ? (c.gameObject.GetJoints(types)) : (EMPTY);
		}

		/// <summary>
		/// Returns components of the requested type.
		/// </summary>
		public static Joint[] GetJoints(this GameObject go, UnityJointMask types)
		{
			// Cannot be null.
			if (go == null)
			{
				return EMPTY;
			}

			// Add
			_buffer.Clear();
			if ((types & UnityJointMask.Fixed) != 0)
			{
				_bufferFixed.Clear();
				go.GetComponents(_bufferFixed);
				_buffer.AddRange(_bufferFixed);
				_bufferFixed.Clear();
			}
			if ((types & UnityJointMask.Hinge) != 0)
			{
				_bufferHinge.Clear();
				go.GetComponents(_bufferHinge);
				_buffer.AddRange(_bufferHinge);
				_bufferHinge.Clear();
			}
			if ((types & UnityJointMask.Spring) != 0)
			{
				_bufferSpring.Clear();
				go.GetComponents(_bufferSpring);
				_buffer.AddRange(_bufferSpring);
				_bufferSpring.Clear();
			}
			if ((types & UnityJointMask.Configurable) != 0)
			{
				_bufferConfigurable.Clear();
				go.GetComponents(_bufferConfigurable);
				_buffer.AddRange(_bufferConfigurable);
				_bufferConfigurable.Clear();
			}
			if ((types & UnityJointMask.Character) != 0)
			{
				_bufferCharacter.Clear();
				go.GetComponents(_bufferCharacter);
				_buffer.AddRange(_bufferCharacter);
				_bufferCharacter.Clear();
			}

			// Return
			Joint[] result = _buffer.ToArray();
			_buffer.Clear();
			return result;
		}
	}
}