// TransformFollow.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   March 21, 2021

using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Helper class that records a follow point between two transforms.
	/// </summary>
	public struct TransformFollow
	{
		public readonly Transform Target;
		public readonly Transform Follower;

		public readonly Vector3 OffsetPosition;
		public readonly Quaternion OffsetRotation;

		public Vector3 CalculatedPosition { get { return (Target != null) ? (Target.position + (Target.rotation * OffsetPosition)) : (Vector3.zero); } }
		public Quaternion CalculatedRotation { get { return (Target != null) ? (Target.rotation * OffsetRotation) : (Quaternion.identity); } }

		/// <summary>
		/// Construct with both transforms.
		/// </summary>
		public TransformFollow(Transform target, Transform follower)
		{
			Target = target;
			Follower = follower;
			if (Target != null && Follower != null)
			{
				OffsetRotation = Quaternion.Inverse(target.rotation) * follower.rotation;
				OffsetPosition = Quaternion.Inverse(target.rotation) * (follower.position - target.position);
			}
			else
			{
				OffsetRotation = default;
				OffsetPosition = default;
			}
		}

		/// <summary>
		/// Move the follow transform based on its target.
		/// </summary>
		public void Apply()
		{
			if (Target != null && Follower != null)
			{
				Follower.position = Target.position + (Target.rotation * OffsetPosition);
				Follower.rotation = Target.rotation * OffsetRotation;
			}
		}
	}
}