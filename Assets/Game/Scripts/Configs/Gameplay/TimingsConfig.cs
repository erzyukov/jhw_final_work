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

		[Header("Units")]
		[SerializeField] private float _unitDeathVanishDelay;

		[Header("RemoteConfig")]
		public float MaxWaitLoadingRemoteConfig;


		public float ExperienceAnimationDuration => _experienceAnimationDuration;
		public float WaveTransitionDelay => _waveTransitionDelay;
		public float UnitDeathVanishDelay => _unitDeathVanishDelay;
	}
}