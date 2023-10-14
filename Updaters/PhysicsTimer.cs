// PhysicsTimer.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   March 28, 2022

using UnityEngine;

namespace Zop
{
	/// <summary>
	/// A synchronized timer for physics events.
	/// </summary>
	public class PhysicsTimer : MonoBehaviour
	{
		public double Offset { get { return _offset; } }

		public double ElapsedSession { get { return _offset + _elapsed; } }
		public double ElapsedTotal { get { return _loaded + _offset + _elapsed; } }

		private bool _run;
		private double _loaded;
		private double _offset;
		private double _elapsed;

		/// <summary>
		/// Add physics time.
		/// </summary>
		public void FixedUpdate()
		{
			if (_run)
			{
				_elapsed += Time.fixedDeltaTime;
			}
		}

		/// <summary>
		/// Load the timer.
		/// </summary>
		public void Load(double elapsed)
		{
			_loaded = elapsed;
		}

		/// <summary>
		/// Offset the timer.
		/// </summary>
		public void SetOffset(double time)
		{
			_offset = time;
		}

		/// <summary>
		/// Offset the timer.
		/// </summary>
		public void AddOffset(double time)
		{
			_offset += time;
		}

		/// <summary>
		/// Start the timer.
		/// </summary>
		public void Start()
		{
			_run = true;
		}

		/// <summary>
		/// Stop the timer.
		/// </summary>
		public void Stop()
		{
			_run = false;
		}

		/// <summary>
		/// Reset the timer.
		/// </summary>
		public void Reset()
		{
			_elapsed = 0;
		}

		/// <summary>
		/// Clear the timer.
		/// </summary>
		public void Clear()
		{
			_run = false;
			_loaded = 0;
			_offset = 0;
			_elapsed = 0;
		}
	}
}