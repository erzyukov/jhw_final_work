namespace Game
{
	using Io.AppMetrica;
	using UnityEngine;

	public static class AppMetricaInit
	{
		public static string ApiKey = "6b4760cf-ecd4-4073-b1e9-1dc3b6fc535a";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Activate()
		{
			Debug.LogWarning("[Appmetrica] Activated");
			AppMetrica.Activate( new AppMetricaConfig( ApiKey ) {
				FirstActivationAsUpdate = !IsFirstLaunch(),
			} );
		}

		private static bool IsFirstLaunch()
		{
			// Implement logic to detect whether the app is opening for the first time.
			// For example, you can check for files (settings, databases, and so on),
			// which the app creates on its first launch.
			return true;
		}
	}
}
