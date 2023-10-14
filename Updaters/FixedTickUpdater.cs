// FixedTickUpdater.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   January 28, 2022

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zop
{
	/// <summary>
	/// A chunky timer that broadcasts less-frequent updates.
	/// </summary>
	public class FixedTickUpdater : MonoBehaviour
	{
		public const int TICK_FREQUENCY = 4;
		public const float TICK_DURATION = 1.0f / TICK_FREQUENCY;
		public const float TICK_DURATION_HALF = TICK_DURATION / 2.0f;

		public static double Elapsed { get { return TickTotalToSeconds + TickProgressToSeconds; } }

		public static int TickIndex { get { return _index; } }
		public static int TickTotal { get { return _total; } }
		public static float TickIndexToSeconds { get { return _index * TICK_DURATION; } }
		public static double TickTotalToSeconds { get { return TicksToSeconds(_total); } }
		public static float TickProgressToSeconds { get { return TICK_DURATION - _time; } }
		public static float TickProgress01 { get { return 1.0f - Mathf.Clamp01(_time / TICK_DURATION); } }
		public static bool IsOffbeat { get { return _isOffbeat; } }
		public static float TickOffbeatToSeconds { get { return _isOffbeat ? TICK_DURATION_HALF : 0; } }

		public static bool HasPrimaryHandlers { get { return _updatesPrimary.Count > 0; } }
		public static bool HasOffbeatHandlers { get { return _updatesOffbeat.Count > 0; } }
		public static int PrimaryHandlersCount { get { return _updatesPrimary.Count; } }
		public static int OffbeatHandlersCount { get { return _updatesOffbeat.Count; } }

		private static readonly Dictionary<object, Action<float>> _updatesPrimary = new Dictionary<object, Action<float>>(ObjectComparer.Instance);
		private static readonly Dictionary<object, Action<float>> _updatesOffbeat = new Dictionary<object, Action<float>>(ObjectComparer.Instance);

		private static readonly HashSet<object> _instances = new HashSet<object>(ObjectComparer.Instance);
		private static readonly List<Action<float>> _invoking = new List<Action<float>>();
		private static readonly List<object> _removing = new List<object>();

		private static double _frame = -1;
		private static float _time = TICK_DURATION;
		private static int _index = 0;
		private static int _total = 0;
		private static bool _isOffbeat;
		private static bool _cleanupPrimary;
		private static bool _cleanupOffbeat;

		/// <summary>
		/// Record instances.
		/// </summary>
		public void Awake()
		{
			_instances.Add(this);
			CleanupSet(_updatesPrimary);
			CleanupSet(_updatesOffbeat);
		}

		/// <summary>
		/// Clean up when there are no instances.
		/// </summary>
		public void OnDestroy()
		{
			_instances.Remove(this);
			if (_instances.Count == 0)
			{
				_invoking.Clear();
				_removing.Clear();
				_cleanupPrimary = false;
				CleanupSet(_updatesPrimary);
				_cleanupOffbeat = false;
				CleanupSet(_updatesOffbeat);
			}
		}

		/// <summary>
		/// Update.
		/// </summary>
		public void FixedUpdate()
		{
			OnUpdate(Time.fixedTimeAsDouble, Time.fixedDeltaTime);
		}

		/// <summary>
		/// Update.
		/// </summary>
		public void OnUpdate(double frame, float delta)
		{
			if (_frame < frame && !Mathf.Approximately((float)_frame, (float)frame))
			{
				_frame = frame;
				_time -= delta;

				// Clear these lists a frame after using.
				bool isClearing = _invoking.Count > 0 || _removing.Count > 0;

				// Invoke, else move to cleanup.
				bool approxZero = Mathf.Approximately(_time, 0.0f);
				if (_time <= 0 || approxZero)
				{
					// NOTE: Trying to stay consistent, this number will stray if not a power of 2.
					if (approxZero)
					{
						_time = TICK_DURATION;
					}
					else
					{
						_time += TICK_DURATION;
					}

					// Backup clear if we run out of in-between updates.
					if (_invoking.Count > 0)
					{
						_invoking.Clear();
					}

					// Update
					_index++;
					_total++;
					_isOffbeat = false;
					InvokeSet(_updatesPrimary, TICK_DURATION);
					_cleanupPrimary = true;
				}
				else if ((_time <= TICK_DURATION_HALF || Mathf.Approximately(_time, TICK_DURATION_HALF)) && _time + delta > TICK_DURATION_HALF)
				{
					// Backup clear if we run out of in-between updates.
					if (_invoking.Count > 0)
					{
						_invoking.Clear();
					}

					// Update
					_isOffbeat = true;
					InvokeSet(_updatesOffbeat, TICK_DURATION);
					_cleanupOffbeat = true;
				}
				else if (isClearing)
				{
					// Clear lists.
					if (_invoking.Count > 0)
					{
						_invoking.Clear();
					}
					else
					{
						_removing.Clear();
					}
				}
				else
				{
					// Not invoking or clearing, check for nulls.
					if (_cleanupPrimary)
					{
						_cleanupPrimary = false;
						CleanupSet(_updatesPrimary);
					}
					else if (_cleanupOffbeat)
					{
						_cleanupOffbeat = false;
						CleanupSet(_updatesOffbeat);
					}
				}
			}
		}

		/// <summary>
		/// Invoke a set of update actions.
		/// </summary>
		private static void InvokeSet(Dictionary<object, Action<float>> dictionary, float delta)
		{
			foreach (Action<float> action in dictionary.Values)
			{
				_invoking.Add(action);
			}
			int end = _invoking.Count;
			for (int i = 0; i < end; i++)
			{
				_invoking[i].Try(delta);
			}
		}

		/// <summary>
		/// Search a set for Unity nulls and remove them.
		/// </summary>
		private static void CleanupSet(Dictionary<object, Action<float>> dictionary)
		{
			foreach (object key in dictionary)
			{
				if (key == null)
				{
					_removing.Add(key);
				}
			}
			int end = _removing.Count;
			for (int i = 0; i < end; i++)
			{
				dictionary.Remove(_removing[i]);
			}
		}

		/// <summary>
		/// Convert seconds to ticks.
		/// </summary>
		public static int SecondsToTicks(double seconds)
		{
			return (int)((seconds * TICK_FREQUENCY) + 0.000001f);
		}

		/// <summary>
		/// Convert seconds to ticks.
		/// </summary>
		public static double TicksToSeconds(int ticks)
		{
			int whole = ticks / TICK_FREQUENCY;
			return whole + ((ticks - (whole * TICK_FREQUENCY)) * TICK_DURATION);
		}

		/// <summary>
		/// Attempts to add this updater.
		/// </summary>
		public static bool Add(object updater, Action<float> action, bool isOffBeat = false)
		{
			return Add((isOffBeat) ? (_updatesOffbeat) : (_updatesPrimary), updater, action);
		}

		/// <summary>
		/// Attempts to remove this updater.
		/// </summary>
		public static bool Remove(object updater, bool isOffBeat = false)
		{
			return Remove((isOffBeat) ? (_updatesOffbeat) : (_updatesPrimary), updater);
		}

		/// <summary>
		/// Reset timers and counters.
		/// </summary>
		public static void Reset()
		{
			_frame = -1;
			_time = TICK_DURATION;
			_index = 0;
			_total = 0;
			_isOffbeat = false;
			if (_cleanupPrimary)
			{
				_cleanupPrimary = false;
				CleanupSet(_updatesPrimary);
			}
			if (_cleanupOffbeat)
			{
				_cleanupOffbeat = false;
				CleanupSet(_updatesOffbeat);
			}
		}

		/// <summary>
		/// Remove all updaters.
		/// </summary>
		public static void Clear()
		{
			_frame = -1;
			_time = TICK_DURATION;
			_index = 0;
			_total = 0;
			_isOffbeat = false;
			_updatesPrimary.Clear();
			_updatesOffbeat.Clear();
			_invoking.Clear();
			_removing.Clear();
			_cleanupPrimary = false;
			_cleanupOffbeat = false;
		}

		/// <summary>
		/// Load an elapsed time into the updater.
		/// </summary>
		public static void Load(double elapsed)
		{
			_total = SecondsToTicks(elapsed);
			SetDelta((float)(elapsed % 1));
		}

		/// <summary>
		/// Adjust the delta time of the updater.
		/// </summary>
		public static void SetDelta(float delta)
		{
			// Clamp
			delta %= 1;
			if (delta < 0)
			{
				delta = 1 - delta;
			}

			// Apply
			_time = TICK_DURATION - (delta % TICK_DURATION);
			_index = SecondsToTicks(delta);
			_isOffbeat = _time < TICK_DURATION_HALF && !Mathf.Approximately(_time, TICK_DURATION_HALF);
		}

		/// <summary>
		/// Attempts to add this updater.
		/// </summary>
		private static bool Add(Dictionary<object, Action<float>> dictionary, object updater, Action<float> action)
		{
			// Cannot be null.
			if (updater == null)
			{
				Debug.LogError("Failed to add null updater.");
				return false;
			}

			// Cannot be null.
			if (updater == null)
			{
				Debug.LogError("Failed to add null updater action.");
				return false;
			}

			// Add
			if (updater != null && action != null && !dictionary.ContainsKey(updater))
			{
				dictionary.Add(updater, action);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Attempts to remove this updater.
		/// </summary>
		private static bool Remove(Dictionary<object, Action<float>> dictionary, object updater)
		{
			// Cannot be null.
			if (updater == null)
			{
				Debug.LogError("Failed to remove null updater.");
				return false;
			}

			// Remove
			return dictionary.Remove(updater);
		}
	}
}