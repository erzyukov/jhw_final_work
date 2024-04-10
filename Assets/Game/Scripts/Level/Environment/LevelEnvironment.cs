namespace Game.Level
{
	using Game.Configs;
	using Game.Profiles;
	using UnityEngine;
	using Zenject;

	public class LevelEnvironment : MonoBehaviour
    {
		[SerializeField] Renderer _groundRenderer;

		[Inject] GameProfile _gameProfile;
		[Inject] LevelsConfig _config;

		private void Start()
		{
			int index = _gameProfile.LevelNumber.Value - 1;
			Sprite environment = _config.Levels[index].Environment;
			_groundRenderer.material.SetTexture( "_MainTex", environment.texture );
		}

	}
}
