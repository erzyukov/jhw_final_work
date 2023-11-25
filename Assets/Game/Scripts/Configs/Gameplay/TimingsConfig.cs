namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Timings", menuName = "Configs/Timings", order = (int)Config.Timings)]
	public class TimingsConfig : ScriptableObject
	{
		[Header("Win Screen")]
		[SerializeField] private float _experienceAnimationDuration;
		
		[Header("Levels")]
		[SerializeField] private float _waveTransitionDelay;

		public float ExperienceAnimationDuration => _experienceAnimationDuration;
		public float WaveTransitionDelay => _waveTransitionDelay;
	}
}