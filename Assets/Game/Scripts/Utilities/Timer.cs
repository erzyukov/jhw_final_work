namespace Game.Utilities
{
	using System;

	public interface ITimer
	{
		bool IsReady { get; }
		bool IsPaused { get; }
		float Progress { get; }
		float Remained { get; }
		float Duration { get; }

		void Add(float duration);
		void Set(float? duration);
		void Set(float duration);
		void SetDuration(float duration);
		void Reset();
		void Pause();
		void Unpause();
	}

	public class Timer : ITimer
	{
		private bool _isUnscaled;
		private double _t_Bgn;
		private double _t_End;
		private double? _t_Paused;
		private double _duration;

		public Timer(bool unscaled = false)
		{
			_isUnscaled = unscaled;
		}

		public bool IsReady => PausableTime >= _t_End;
		public bool IsPaused => _t_Paused != null;
		public float Progress => _duration <= 0 ? 1 : (float)Math.Clamp((PausableTime - _t_Bgn) / _duration, 0, 1);
		public float Remained => (float)(_duration * (1 - Progress));
		public float Duration => (float)_duration;

		private double PausableTime => _t_Paused ?? Time;
		private double Time => _isUnscaled ? UnityEngine.Time.unscaledTime : UnityEngine.Time.time;

		public void Add(float duration)
		{
			_t_End += duration;
			_duration += duration;
		}

		public void Set(float? duration) => 
			Set(duration.HasValue ? duration.Value : float.PositiveInfinity);

		public void Set(float duration)
		{
			_t_Paused = null;
			_t_Bgn = Time;
			_t_End = Time + duration;
			_duration = duration;
		}

		public void SetDuration(float duration)
		{
			_t_End = _t_Bgn + duration;
			_duration = duration;
		}

		public void Reset()
		{
			_t_Paused = null;
			_t_Bgn = 0;
			_t_End = 0;
			_duration = 0;
		}

		public void Pause()
		{
			_t_Paused = Time;
		}

		public void Unpause()
		{
			if (IsPaused == false)
				return;

			double pauseTime = Time - _t_Paused.Value;

			_t_Bgn += pauseTime;
			_t_End += pauseTime;

			_t_Paused = null;
		}
	}
}

