namespace Game.Dev
{
	using Game.Configs;
	using UnityEngine;
	using Zenject;

	public class DevHud : MonoBehaviour
    {
		[Inject] DevConfig _devConfig;

		private void Awake()
		{
			gameObject.SetActive(_devConfig.Build == DevConfig.BuildType.Debug);
		}
	}
}
