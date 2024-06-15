namespace Game
{
	using UnityEngine;

	public static class Logger
	{
		public static void Log(Module module, string text) => Debug.Log(Format(module, text));
		public static void LogError(Module module, string text) => Debug.LogError(Format(module, text));


		public static string Format(Module module, string text) => $"{module}: {text}";

		public enum Module
		{
			Core,
			Ads,
		}
	}
}
