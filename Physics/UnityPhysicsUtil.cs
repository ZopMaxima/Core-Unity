// UnityPhysicsUtil.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 23, 2022

using System.Collections.Generic;
using UnityEngine;
using Zop.Unity;

/// <summary>
/// Extend unity physics.
/// </summary>
public static class UnityPhysicsUtil
{
	public const float MASS_KINEMATIC = 10000;

	private static readonly List<Rigidbody> _hashBuffer = new List<Rigidbody>();

	/// <summary>
	/// Returns the total mass of all bodies.
	/// </summary>
	private static float CountMass(Rigidbody[] bodies)
	{
		float total = 0;

		// Count
		for (int i = 0; i < bodies.Length; i++)
		{
			Rigidbody r = bodies[i];
			total += (r == null) ? (0) : (r.isKinematic ? MASS_KINEMATIC : r.mass);
		}

		// Return
		return total;
	}

	/// <summary>
	/// Returns the total mass of all bodies.
	/// </summary>
	private static float CountMass(List<Rigidbody> bodies)
	{
		float total = 0;

		// Count
		for (int i = 0; i < bodies.Count; i++)
		{
			Rigidbody r = bodies[i];
			total += (r == null) ? (0) : (r.isKinematic ? MASS_KINEMATIC : r.mass);
		}

		// Return
		return total;
	}

	/// <summary>
	/// Returns the total mass of all bodies.
	/// </summary>
	private static float CountMass(HashSet<Rigidbody> bodies)
	{
		float total = 0;

		// Count
		foreach (Rigidbody r in bodies)
		{
			total += (r == null) ? (0) : (r.isKinematic ? MASS_KINEMATIC : r.mass);
		}

		// Return
		return total;
	}

	/// <summary>
	/// Split this force across each body based on mass.
	/// </summary>
	public static void ShareForce(this Rigidbody[] bodies, Vector3 force)
	{
		// Cannot be null.
		if (bodies == null || force.x == 0 && force.y == 0)
		{
			return;
		}

		// Apply
		float total = CountMass(bodies);
		for (int i = 0; i < bodies.Length; i++)
		{
			Rigidbody r = bodies[i];
			if (r != null)
			{
				float mass = r.isKinematic ? MASS_KINEMATIC : r.mass;
				r.AddForce(force * (mass / total));
			}
		}
	}

	/// <summary>
	/// Split this force across each body based on mass.
	/// </summary>
	public static void ShareForce(this List<Rigidbody> bodies, Vector3 force)
	{
		// Cannot be null.
		if (bodies == null || force.x == 0 && force.y == 0)
		{
			return;
		}

		// Apply
		float total = CountMass(bodies);
		for (int i = 0; i < bodies.Count; i++)
		{
			Rigidbody r = bodies[i];
			if (r != null)
			{
				float mass = r.isKinematic ? MASS_KINEMATIC : r.mass;
				r.AddForce(force * (mass / total));
			}
		}
	}

	/// <summary>
	/// Split this force across each body based on mass.
	/// </summary>
	public static void ShareForce(this HashSet<Rigidbody> bodies, Vector3 force)
	{
		// Cannot be null.
		if (bodies == null || force.x == 0 && force.y == 0)
		{
			return;
		}

		// Apply
		float total = CountMass(bodies);
		foreach (Rigidbody r in bodies)
		{
			if (r != null)
			{
				float mass = r.isKinematic ? MASS_KINEMATIC : r.mass;
				r.AddForce(force * (mass / total));
			}
		}
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void PullForce(this Rigidbody[] bodies, float magnitude)
	{
		// Cannot be null.
		if (bodies == null || bodies.Length <= 1 || magnitude == 0)
		{
			return;
		}

		// Apply
		int iEnd = bodies.Length - 1;
		int jEnd = bodies.Length;
		for (int i = 0; i < iEnd; i++)
		{
			for (int j = i + 1; j < jEnd; j++)
			{
				bodies[i].PullForce(bodies[j], magnitude);
			}
		}
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void PullForce(this List<Rigidbody> bodies, float magnitude)
	{
		// Cannot be null.
		if (bodies == null || bodies.Count <= 1 || magnitude == 0)
		{
			return;
		}

		// Apply
		int iEnd = bodies.Count - 1;
		int jEnd = bodies.Count;
		for (int i = 0; i < iEnd; i++)
		{
			for (int j = i + 1; j < jEnd; j++)
			{
				bodies[i].PullForce(bodies[j], magnitude);
			}
		}
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void PullForce(this HashSet<Rigidbody> bodies, float magnitude)
	{
		// Cannot be null.
		if (bodies == null || bodies.Count <= 1 || magnitude == 0)
		{
			return;
		}

		// Apply
		_hashBuffer.Clear();
		_hashBuffer.AddRange(bodies);
		PullForce(_hashBuffer, magnitude);
		_hashBuffer.Clear();
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void PullForce(this Rigidbody body, Rigidbody other, float magnitude)
	{
		// Cannot be null.
		if (body == null || other == null || magnitude == 0)
		{
			return;
		}

		// Apply
		Vector3 force = (other.position - body.position).normalized * magnitude;
		if (!body.isKinematic)
		{
			if (!other.isKinematic)
			{
				float ratio = body.mass / (body.mass + other.mass);
				body.AddForce(force * (1 - ratio));
				other.AddForce(-(force * ratio));
			}
			else
			{
				body.AddForce(force);
			}
		}
		else if (!other.isKinematic)
		{
			other.AddForce(-force);
		}
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void PullForce(this CombinedMass body, Rigidbody other, Vector3 bodyToOtherAxis, float magnitude)
	{
		PullForce(other, body, -bodyToOtherAxis, magnitude);
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void PullForce(this Rigidbody body, CombinedMass other, Vector3 bodyToOtherAxis, float magnitude)
	{
		// Cannot be null.
		if (body == null || other == null || magnitude == 0)
		{
			return;
		}

		// Apply
		Vector3 force = bodyToOtherAxis.normalized * magnitude;
		float bMass = body.mass;
		float oMass = other.GetAllMass(-bodyToOtherAxis);
		if (!body.isKinematic)
		{
			if (!float.IsInfinity(oMass))
			{
				float ratio = bMass / (bMass + oMass);
				body.AddForce(force * (1 - ratio));
				other.AddForce(-(force * ratio));
			}
			else
			{
				body.AddForce(force);
			}
		}
		else if (!float.IsInfinity(oMass))
		{
			other.AddForce(-force);
		}
		else
		{
			body.AddForce(force * 0.5f);
			other.AddForce(-(force * 0.5f));
		}
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void PullForce(this CombinedMass body, CombinedMass other, Vector3 bodyToOtherAxis, float magnitude)
	{
		// Cannot be null.
		if (body == null || other == null || magnitude == 0)
		{
			return;
		}

		// Apply
		Vector3 force = bodyToOtherAxis.normalized * magnitude;
		float bMass = body.GetAllMass(bodyToOtherAxis);
		float oMass = other.GetAllMass(-bodyToOtherAxis);
		if (!float.IsInfinity(bMass))
		{
			if (!float.IsInfinity(oMass))
			{
				float ratio = bMass / (bMass + oMass);
				body.AddForce(force * (1 - ratio));
				other.AddForce(-(force * ratio));
			}
			else
			{
				body.AddForce(force);
			}
		}
		else if (!float.IsInfinity(oMass))
		{
			other.AddForce(-force);
		}
		else
		{
			body.AddForce(force * 0.5f);
			other.AddForce(-(force * 0.5f));
		}
	}

	/// <summary>
	/// Apply this force across each body based on mass.
	/// </summary>
	public static void TryCombinedPullForce(this Rigidbody body, Rigidbody other, Vector3 bodyToOtherAxis, float magnitude)
	{
		// Cannot be null.
		if (body == null || other == null || magnitude == 0)
		{
			return;
		}

		// Apply
		CombinedMass bCombined = body.GetComponent<CombinedMass>();
		CombinedMass oCombined = other.GetComponent<CombinedMass>();
		if (bCombined != null)
		{
			if (oCombined != null)
			{
				PullForce(bCombined, oCombined, bodyToOtherAxis, magnitude);
			}
			else
			{
				PullForce(bCombined, other, bodyToOtherAxis, magnitude);
			}
		}
		else if (oCombined != null)
		{
			PullForce(body, oCombined, bodyToOtherAxis, magnitude);
		}
		else
		{
			PullForce(body, other, magnitude);
		}
	}
}