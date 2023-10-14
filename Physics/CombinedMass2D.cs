// CombinedMass2D.cs
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
	public class CombinedMass2D : MonoBehaviour
	{
		private static readonly float DOT_STABLE = Mathf.Cos(15 * Mathf.Rad2Deg);
		private static readonly float DOT_SLIDE = Mathf.Cos(75 * Mathf.Rad2Deg);
		private static readonly float DOT_MIN = DOT_STABLE * DOT_STABLE;
		private static readonly float DOT_MAX = 1.0f - (DOT_SLIDE * DOT_SLIDE);
		private static readonly float DOT_MULTI = 1.0f / (DOT_MIN + DOT_MAX);

		public bool AutoJoinStart = true;

		public float Mass { get { return (_rigidbody == null) ? (0) : (_rigidbody.isKinematic ? UnityPhysicsUtil2D.MASS_KINEMATIC : _rigidbody.mass); } }
		public bool IsKinematic { get { return (_rigidbody != null) ? (_rigidbody.isKinematic) : (false); } }

		private Rigidbody2D _rigidbody;
		private HashSet<Transform> _joinedStatics = new HashSet<Transform>(ObjectComparer.Instance);
		private HashSet<Rigidbody2D> _joinedBodies = new HashSet<Rigidbody2D>(ObjectComparer.Instance);
		private HashSet<CombinedMass2D> _joinedCombos = new HashSet<CombinedMass2D>(ObjectComparer.Instance);
		private DictionaryDuo<Transform, Collider2D, Vector2> _collisionStatics = new DictionaryDuo<Transform, Collider2D, Vector2>(ObjectComparer.Instance, ObjectComparer.Instance);
		private DictionaryDuo<Rigidbody2D, Collider2D, Vector2> _collisionBodies = new DictionaryDuo<Rigidbody2D, Collider2D, Vector2>(ObjectComparer.Instance, ObjectComparer.Instance);
		private DictionaryDuo<CombinedMass2D, Collider2D, Vector2> _collisionCombos = new DictionaryDuo<CombinedMass2D, Collider2D, Vector2>(ObjectComparer.Instance, ObjectComparer.Instance);

		private static CombinedMass2D _searchInvoker;
		private static readonly HashSet<Transform> _searchStatics = new HashSet<Transform>(ObjectComparer.Instance);
		private static readonly HashSet<Rigidbody2D> _searchBodies = new HashSet<Rigidbody2D>(ObjectComparer.Instance);
		private static readonly HashSet<CombinedMass2D> _searchRoots = new HashSet<CombinedMass2D>(ObjectComparer.Instance);
		private static readonly Dictionary<Rigidbody2D, CombinedMass2D> _lookup = new Dictionary<Rigidbody2D, CombinedMass2D>(ObjectComparer.Instance);

		private static readonly List<Rigidbody2D> _jBuffer = new List<Rigidbody2D>();
		private static readonly List<Rigidbody2D> _cBuffer = new List<Rigidbody2D>();

		/// <summary>
		/// Initialize.
		/// </summary>
		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
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
				Joint2D[] joints = GetComponents<Joint2D>();
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
		private void OnCollisionEnter2D(Collision2D collision)
		{
			// Average normal.
			Vector2 normal = collision.GetContact(0).normal;
			for (int i = 1; i < collision.contactCount; i++)
			{
				normal += collision.GetContact(i).normal;
			}
			if (collision.contactCount > 1)
			{
				normal = normal.normalized;
			}

			// Set the normal.
			if (collision.rigidbody != null)
			{
				_collisionBodies.Set(collision.rigidbody, collision.collider, normal);
				CombinedMass2D combo = collision.rigidbody.GetComponent<CombinedMass2D>();
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
		private void OnCollisionStay2D(Collision2D collision)
		{
			OnCollisionEnter2D(collision);
		}

		/// <summary>
		/// Record collisions for directional mass.
		/// </summary>
		private void OnCollisionExit2D(Collision2D collision)
		{
			if (collision.rigidbody != null)
			{
				_collisionBodies.Remove(collision.rigidbody, collision.collider);
				CombinedMass2D combo = collision.rigidbody.GetComponent<CombinedMass2D>();
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
		public void Join(CombinedMass2D body)
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
		public void Join(Rigidbody2D body)
		{
			if (body != null && body != _rigidbody)
			{
				CombinedMass2D combined = body.GetComponent<CombinedMass2D>();
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
				CombinedMass2D combined = body.GetComponent<CombinedMass2D>();
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
				CombinedMass2D combined = other.GetComponent<CombinedMass2D>();
				if (combined != null)
				{
					Join(combined);
				}
				else
				{
					Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
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
		public void Break(CombinedMass2D body)
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
		public void Break(Rigidbody2D body)
		{
			if (body != null && body != _rigidbody)
			{
				CombinedMass2D combined = body.GetComponent<CombinedMass2D>();
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
				CombinedMass2D combined = body.GetComponent<CombinedMass2D>();
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
				CombinedMass2D combined = other.GetComponent<CombinedMass2D>();
				if (combined != null)
				{
					Break(combined);
				}
				else
				{
					Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
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
			return GetJointMass(new Vector2());
		}

		/// <summary>
		/// Returns the mass of objects joined to this body.
		/// </summary>
		public float GetJointMass(Vector2 direction)
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
					mass += UnityPhysicsUtil2D.MASS_KINEMATIC;
				}
			}
			foreach (var r in _joinedBodies)
			{
				if (r != null && !_searchBodies.Contains(r))
				{
					_searchBodies.Add(r);
					mass += r.isKinematic ? UnityPhysicsUtil2D.MASS_KINEMATIC : r.mass;
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
		public float GetJointMass(List<Rigidbody2D> buffer)
		{
			return GetJointMass(new Vector2(), buffer);
		}

		/// <summary>
		/// Returns the mass of objects joined to this body.
		/// </summary>
		public float GetJointMass(Vector2 direction, List<Rigidbody2D> buffer)
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
					mass += UnityPhysicsUtil2D.MASS_KINEMATIC;
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
					mass += r.isKinematic ? UnityPhysicsUtil2D.MASS_KINEMATIC : r.mass;
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
		public float GetCollisionMass(Vector2 direction)
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
			Vector2 directionNormal = direction.normalized;
			foreach (var kvp in _collisionStatics)
			{
				Transform t = kvp.Key;
				if (t != null && !_searchStatics.Contains(t))
				{
					foreach (Vector2 normal in kvp.Value.Values)
					{
						float dot = Vector2.Dot(directionNormal, normal);
						if (dot < 0)
						{
							float percent = Mathf.Clamp01(((dot * dot) - DOT_MIN) * DOT_MULTI); // TODO: Pick highest.
							if (percent > 0)
							{
								_searchStatics.Add(t);
								mass += UnityPhysicsUtil2D.MASS_KINEMATIC * percent;
							}
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionBodies)
			{
				Rigidbody2D r = kvp.Key;
				if (r != null && !_searchBodies.Contains(r))
				{
					foreach (Vector2 normal in kvp.Value.Values)
					{
						float dot = Vector2.Dot(directionNormal, normal);
						if (dot < 0)
						{
							float percent = Mathf.Clamp01(((dot * dot) - DOT_MIN) * DOT_MULTI); // TODO: Pick highest.
							if (percent > 0)
							{
								_searchBodies.Add(r);
								mass += (r.isKinematic ? UnityPhysicsUtil2D.MASS_KINEMATIC : r.mass) * percent;
							}
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionCombos)
			{
				CombinedMass2D c = kvp.Key;
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
		public float GetCollisionMass(Vector2 direction, List<Rigidbody2D> buffer)
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
			Vector2 directionNormal = direction.normalized;
			foreach (var kvp in _collisionStatics)
			{
				Transform t = kvp.Key;
				if (t != null && !_searchStatics.Contains(t))
				{
					foreach (Vector2 normal in kvp.Value.Values)
					{
						float dot = Vector2.Dot(directionNormal, normal);
						if (dot < 0)
						{
							float percent = Mathf.Clamp01(((dot * dot) - DOT_MIN) * DOT_MULTI); // TODO: Pick highest.
							if (percent > 0)
							{
								_searchStatics.Add(t);
								mass += UnityPhysicsUtil2D.MASS_KINEMATIC * percent;
							}
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionBodies)
			{
				Rigidbody2D r = kvp.Key;
				if (r != null && !_searchBodies.Contains(r))
				{
					foreach (Vector2 normal in kvp.Value.Values)
					{
						float dot = Vector2.Dot(directionNormal, normal);
						if (dot < 0)
						{
							float percent = Mathf.Clamp01(((dot * dot) - DOT_MIN) * DOT_MULTI); // TODO: Pick highest.
							if (percent > 0)
							{
								_searchBodies.Add(r);

								// Add to the list of bodies.
								if (hasBuffer)
								{
									buffer.Add(r);
								}

								// Add mass.
								mass += (r.isKinematic ? UnityPhysicsUtil2D.MASS_KINEMATIC : r.mass) * percent;
							}
							break;
						}
					}
				}
			}
			foreach (var kvp in _collisionCombos)
			{
				CombinedMass2D c = kvp.Key;
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
		public float GetAllMass(Vector2 direction)
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
		public float GetAllMass(Vector2 direction, List<Rigidbody2D> buffer)
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
		public void AddForce(Vector2 force)
		{
			_jBuffer.Clear();
			_cBuffer.Clear();
			float mass = Mass;
			float jMass = GetJointMass(_jBuffer);
			float cMass = GetCollisionMass(force, _cBuffer);
			Vector2 jForce = force * (jMass / (mass + jMass + cMass));
			Vector2 cForce = force * (cMass / (mass + jMass + cMass));

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