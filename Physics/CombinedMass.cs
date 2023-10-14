// CombinedMass.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 21, 2022

using System.Collections.Generic;
using UnityEngine;

namespace Zop.Unity
{
	/// <summary>
	/// Helper class that records a follow point between two transforms.
	/// </summary>
	public class CombinedMass : MonoBehaviour
	{
		public bool AutoJoinStart = true;

		public float Mass { get { return (_rigidbody == null) ? (0) : (_rigidbody.isKinematic ? UnityPhysicsUtil.MASS_KINEMATIC : _rigidbody.mass); } }
		public bool IsKinematic { get { return (_rigidbody != null) ? (_rigidbody.isKinematic) : (false); } }

		private Rigidbody _rigidbody;
		private HashSet<Transform> _joinedStatics = new HashSet<Transform>(ObjectComparer.Instance);
		private HashSet<Rigidbody> _joinedBodies = new HashSet<Rigidbody>(ObjectComparer.Instance);
		private HashSet<CombinedMass> _joinedCombos = new HashSet<CombinedMass>(ObjectComparer.Instance);
		private DictionaryDuo<Transform, Collider, Vector3> _collisionStatics = new DictionaryDuo<Transform, Collider, Vector3>(ObjectComparer.Instance, ObjectComparer.Instance);
		private DictionaryDuo<Rigidbody, Collider, Vector3> _collisionBodies = new DictionaryDuo<Rigidbody, Collider, Vector3>(ObjectComparer.Instance, ObjectComparer.Instance);
		private DictionaryDuo<CombinedMass, Collider, Vector3> _collisionCombos = new DictionaryDuo<CombinedMass, Collider, Vector3>(ObjectComparer.Instance, ObjectComparer.Instance);

		private static CombinedMass _searchInvoker;
		private static readonly HashSet<Transform> _searchStatics = new HashSet<Transform>(ObjectComparer.Instance);
		private static readonly HashSet<Rigidbody> _searchBodies = new HashSet<Rigidbody>(ObjectComparer.Instance);
		private static readonly HashSet<CombinedMass> _searchRoots = new HashSet<CombinedMass>(ObjectComparer.Instance);
		private static readonly Dictionary<Rigidbody, CombinedMass> _lookup = new Dictionary<Rigidbody, CombinedMass>(ObjectComparer.Instance);

		private static readonly List<Rigidbody> _jBuffer = new List<Rigidbody>();
		private static readonly List<Rigidbody> _cBuffer = new List<Rigidbody>();

		/// <summary>
		/// Initialize.
		/// </summary>
		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			if (_rigidbody != null)
			{
				_lookup[_rigidbody] = this;
			}
		}

		/// <summary>
		/// Attempt to join with other bodies.
		/// </summary>
		private void Start()
		{
			if (AutoJoinStart)
			{
				Joint[] joints = GetComponents<Joint>();
				if (joints != null)
				{
					for (int i = 0; i < joints.Length; i++)
					{
						if (joints[i].connectedBody != null)
						{
							TryJoin(joints[i].transform);
						}
					}
				}
			}
		}

		/// <summary>
		/// Terminate.
		/// </summary>
		private void OnDestroy()
		{
			if (_rigidbody != null)
			{
				_lookup.Remove(_rigidbody);
			}
		}

		/// <summary>
		/// Record collisions for directional mass.
		/// </summary>
		private void OnCollisionEnter(Collision collision)
		{
			// Average normal.
			Vector3 normal = collision.GetContact(0).normal;
			for (int i = 1; i < collision.contactCount; i++)
			{
				normal += collision.GetContact(i).normal;
			}

			// Set the normal.
			if (collision.rigidbody != null)
			{
				_collisionBodies.Set(collision.rigidbody, collision.collider, normal);
				CombinedMass combo = collision.rigidbody.GetComponent<CombinedMass>();
				if (combo != null)
				{
					_collisionCombos.Set(combo, collision.collider, normal);
				}
			}
			else
			{
				_collisionStatics.Set(collision.collider.transform, collision.collider, normal);
			}
		}

		/// <summary>
		/// Record collisions for directional mass.
		/// </summary>
		private void OnCollisionStay(Collision collision)
		{
			OnCollisionEnter(collision);
		}

		/// <summary>
		/// Record collisions for directional mass.
		/// </summary>
		private void OnCollisionExit(Collision collision)
		{
			if (collision.rigidbody != null)
			{
				_collisionBodies.Remove(collision.rigidbody, collision.collider);
				CombinedMass combo = collision.rigidbody.GetComponent<CombinedMass>();
				if (combo != null)
				{
					_collisionCombos.Remove(combo, collision.collider);
				}
			}
			else
			{
				_collisionStatics.Remove(collision.collider.transform, collision.collider);
			}
		}

		/// <summary>
		/// Join this body with the combined mass.
		/// </summary>
		public void Join(CombinedMass body)
		{
			if (body != null)
			{
				if (body._rigidbody != null)
				{
					_joinedBodies.Add(body._rigidbody);
				}
				_joinedCombos.Add(body);
			}
		}

		/// <summary>
		/// Join this body with the combined mass.
		/// </summary>
		public void Join(Rigidbody body)
		{
			if (body != null && body != _rigidbody)
			{
				CombinedMass combined = body.GetComponent<CombinedMass>();
				if (combined != null)
				{
					Join(combined);
				}
				else
				{
					_joinedBodies.Add(body);
				}
			}
		}

		/// <summary>
		/// Join this body with the combined mass.
		/// Without a rigidbody, this transform adds kinematic mass.
		/// </summary>
		public void Join(Transform body)
		{
			if (body != null && body != transform)
			{
				CombinedMass combined = body.GetComponent<CombinedMass>();
				if (combined != null)
				{
					Join(combined);
				}
				else
				{
					_joinedStatics.Add(body);
				}
			}
		}

		/// <summary>
		/// Join this body with the combined mass.
		/// </summary>
		public void TryJoin(Transform other)
		{
			if (other != null)
			{
				CombinedMass combined = other.GetComponent<CombinedMass>();
				if (combined != null)
				{
					Join(combined);
				}
				else
				{
					Rigidbody rigidbody = other.GetComponent<Rigidbody>();
					if (rigidbody != null)
					{
						Join(rigidbody);
					}
					else
					{
						Join(other);
					}
				}
			}
		}

		/// <summary>
		/// Join this body with the combined mass.
		/// </summary>
		public void TryJoin(GameObject other)
		{
			if (other != null)
			{
				TryJoin(other.transform);
			}
		}

		/// <summary>
		/// Break this body away from the combined mass.
		/// </summary>
		public void Break(CombinedMass body)
		{
			if (body != null)
			{
				if (body._rigidbody != null)
				{
					_joinedBodies.Remove(body._rigidbody);
				}
				_joinedCombos.Remove(body);
			}
		}

		/// <summary>
		/// Break this body away from the combined mass.
		/// </summary>
		public void Break(Rigidbody body)
		{
			if (body != null && body != _rigidbody)
			{
				CombinedMass combined = body.GetComponent<CombinedMass>();
				if (combined != null)
				{
					Break(combined);
				}
				else
				{
					_joinedBodies.Remove(body);
				}
			}
		}

		/// <summary>
		/// Break this body away from the combined mass.
		/// Without a rigidbody, this transform adds kinematic mass.
		/// </summary>
		public void Break(Transform body)
		{
			if (body != null && body != transform)
			{
				CombinedMass combined = body.GetComponent<CombinedMass>();
				if (combined != null)
				{
					Break(combined);
				}
				else
				{
					_joinedStatics.Remove(body);
				}
			}
		}

		/// <summary>
		/// Break this body away from the combined mass.
		/// </summary>
		public void TryBreak(Transform other)
		{
			if (other != null)
			{
				CombinedMass combined = other.GetComponent<CombinedMass>();
				if (combined != null)
				{
					Break(combined);
				}
				else
				{
					Rigidbody rigidbody = other.GetComponent<Rigidbody>();
					if (rigidbody != null)
					{
						Break(rigidbody);
					}
					else
					{
						Break(other);
					}
				}
			}
		}

		/// <summary>
		/// Break this body away from the combined mass.
		/// </summary>
		public void TryBreak(GameObject other)
		{
			if (other != null)
			{
				TryBreak(other.transform);
			}
		}

		/// <summary>
		/// Returns the mass of objects joined to this body.
		/// </summary>
		public float GetJointMass()
		{
			return GetJointMass(new Vector3());
		}

		/// <summary>
		/// Returns the mass of objects joined to this body.
		/// </summary>
		public float GetJointMass(Vector3 direction)
		{
			float mass = 0;

			// The origin of this calculation.
			if (_searchInvoker == null)
			{
				_searchInvoker = this;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}
			_searchRoots.Add(this);

			// Compare each mass.
			foreach (var t in _joinedStatics)
			{
				if (t != null && !_searchStatics.Contains(t))
				{
					_searchStatics.Add(t);
					mass += UnityPhysicsUtil.MASS_KINEMATIC;
				}
			}
			foreach (var r in _joinedBodies)
			{
				if (r != null && !_searchBodies.Contains(r))
				{
					_searchBodies.Add(r);
					mass += r.isKinematic ? UnityPhysicsUtil.MASS_KINEMATIC : r.mass;
				}
			}
			foreach (var c in _joinedCombos)
			{
				if (c != null && !_searchRoots.Contains(c))
				{
					mass += c.GetAllMass(direction);
				}
			}

			// Clean-up
			if (_searchInvoker == this)
			{
				_searchInvoker = null;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}

			// Return
			return mass;
		}

		/// <summary>
		/// Returns the mass of objects joined to this body.
		/// </summary>
		public float GetJointMass(List<Rigidbody> buffer)
		{
			return GetJointMass(new Vector3(), buffer);
		}

		/// <summary>
		/// Returns the mass of objects joined to this body.
		/// </summary>
		public float GetJointMass(Vector3 direction, List<Rigidbody> buffer)
		{
			float mass = 0;
			bool hasBuffer = buffer != null;

			// The origin of this calculation.
			if (_searchInvoker == null)
			{
				_searchInvoker = this;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}
			_searchRoots.Add(this);

			// Compare each mass.
			foreach (var t in _joinedStatics)
			{
				if (t != null && !_searchStatics.Contains(t))
				{
					_searchStatics.Add(t);
					mass += UnityPhysicsUtil.MASS_KINEMATIC;
				}
			}
			foreach (var r in _joinedBodies)
			{
				if (r != null && !_searchBodies.Contains(r))
				{
					_searchBodies.Add(r);

					// Add to the list of bodies.
					if (hasBuffer)
					{
						buffer.Add(r);
					}

					// Add mass.
					mass += r.isKinematic ? UnityPhysicsUtil.MASS_KINEMATIC : r.mass;
				}
			}
			foreach (var c in _joinedCombos)
			{
				if (c != null && !_searchRoots.Contains(c))
				{
					mass += c.GetAllMass(direction, buffer);
				}
			}

			// Clean-up
			if (_searchInvoker == this)
			{
				_searchInvoker = null;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}

			// Return
			return mass;
		}

		/// <summary>
		/// Returns the mass of objects obstructing this direction.
		/// </summary>
		public float GetCollisionMass(Vector3 direction)
		{
			float mass = 0;

			// Ignore a directionless pull.
			if (direction == default)
			{
				return mass;
			}

			// The origin of this calculation.
			if (_searchInvoker == null)
			{
				_searchInvoker = this;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}
			_searchRoots.Add(this);

			// Compare each mass.
			foreach (var kvp in _collisionStatics)
			{
				Transform t = kvp.Key;
				if (t != null && !_searchStatics.Contains(t))
				{
					foreach (Vector3 normal in kvp.Value.Values)
					{
						if (Vector3.Dot(direction, normal) < 0)
						{
							_searchStatics.Add(t);
							mass += UnityPhysicsUtil.MASS_KINEMATIC;
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionBodies)
			{
				Rigidbody r = kvp.Key;
				if (r != null && !_searchBodies.Contains(r))
				{
					foreach (Vector3 normal in kvp.Value.Values)
					{
						if (Vector3.Dot(direction, normal) < 0)
						{
							_searchBodies.Add(r);
							mass += r.isKinematic ? UnityPhysicsUtil.MASS_KINEMATIC : r.mass;
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionCombos)
			{
				CombinedMass c = kvp.Key;
				if (c != null && !_searchRoots.Contains(c))
				{
					mass += c.GetAllMass(direction);
				}
			}

			// Clean-up
			if (_searchInvoker == this)
			{
				_searchInvoker = null;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}

			// Return
			return mass;
		}

		/// <summary>
		/// Returns the mass of objects obstructing this direction.
		/// </summary>
		public float GetCollisionMass(Vector3 direction, List<Rigidbody> buffer)
		{
			float mass = 0;
			bool hasBuffer = buffer != null;

			// Ignore a directionless pull.
			if (direction == default)
			{
				return mass;
			}

			// The origin of this calculation.
			if (_searchInvoker == null)
			{
				_searchInvoker = this;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}
			_searchRoots.Add(this);

			// Compare each mass.
			foreach (var kvp in _collisionStatics)
			{
				Transform t = kvp.Key;
				if (t != null && !_searchStatics.Contains(t))
				{
					foreach (Vector3 normal in kvp.Value.Values)
					{
						if (Vector3.Dot(direction, normal) < 0)
						{
							_searchStatics.Add(t);
							mass += UnityPhysicsUtil.MASS_KINEMATIC;
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionBodies)
			{
				Rigidbody r = kvp.Key;
				if (r != null && !_searchBodies.Contains(r))
				{
					foreach (Vector3 normal in kvp.Value.Values)
					{
						if (Vector3.Dot(direction, normal) < 0)
						{
							_searchBodies.Add(r);

							// Add to the list of bodies.
							if (hasBuffer)
							{
								buffer.Add(r);
							}

							// Add mass.
							mass += r.isKinematic ? UnityPhysicsUtil.MASS_KINEMATIC : r.mass;
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionCombos)
			{
				CombinedMass c = kvp.Key;
				if (c != null && !_searchRoots.Contains(c))
				{
					mass += c.GetAllMass(direction, buffer);
				}
			}

			// Clean-up
			if (_searchInvoker == this)
			{
				_searchInvoker = null;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}

			// Return
			return mass;
		}

		/// <summary>
		/// Returns the mass of all objects, including objects obstructing this direction.
		/// </summary>
		public float GetAllMass(Vector3 direction)
		{
			float mass = 0;

			// The origin of this calculation.
			if (_searchInvoker == null)
			{
				_searchInvoker = this;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}
			_searchRoots.Add(this);

			// NOTE: May already be part of a mega-mass-search.
			if (_rigidbody != null && !_searchBodies.Contains(_rigidbody))
			{
				_searchBodies.Add(_rigidbody);
				mass += Mass;
			}
			mass += GetJointMass(direction);
			mass += GetCollisionMass(direction);

			// Clean-up
			if (_searchInvoker == this)
			{
				_searchInvoker = null;
				_searchRoots.Clear();
				_searchBodies.Clear();
				_searchStatics.Clear();
			}

			// Return
			return mass;
		}

		/// <summary>
		/// Returns the mass of all objects, including objects obstructing this direction.
		/// </summary>
		public float GetAllMass(Vector3 direction, List<Rigidbody> buffer)
		{
			float mass = Mass;
			if (buffer != null && _rigidbody != null)
			{
				buffer.Add(_rigidbody);
			}
			mass += GetJointMass(buffer);
			mass += GetCollisionMass(direction, buffer);

			// Return
			return mass;
		}

		/// <summary>
		/// Add this force across all bodies.
		/// </summary>
		public void AddForce(Vector3 force)
		{
			_jBuffer.Clear();
			_cBuffer.Clear();
			float mass = Mass;
			float jMass = GetJointMass(_jBuffer);
			float cMass = GetCollisionMass(force, _cBuffer);
			Vector3 jForce = force * (jMass / (mass + jMass + cMass));
			Vector3 cForce = force * (cMass / (mass + jMass + cMass));

			// Apply
			if (_rigidbody != null)
			{
				_rigidbody.AddForce(force * (mass / (mass + jMass + cMass)));
			}
			_jBuffer.ShareForce(jForce);
			_cBuffer.ShareForce(cForce); // TODO: jBuffer and cBuffer may contain overlapping bodies.

			// Clean up.
			_cBuffer.Clear();
		}
	}
}