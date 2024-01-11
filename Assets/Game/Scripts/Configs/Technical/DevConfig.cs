namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Dev", menuName = "Configs/Development", order = (int)Config.Dev)]
	public class DevConfig : ScriptableObject
	{
		[SerializeField] private BuildType _buildType = BuildType.Debug;
		[SerializeField] private float _timeScale = 1;

		public enum BuildType
		{
			Release,
			Debug,
		}

		public BuildType Build => _buildType;

		public float TimeScale => _timeScale;
	}
}