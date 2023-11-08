namespace Game.Dev
{
	using System.Runtime.InteropServices;
	//using VContainer.Unity;

	public static class WebGLDebug
	{
		[DllImport("__Internal")]
		private static extern void WebGLConsoleLog(string message);

		public static void Log(string message)
		{
#if UNITY_EDITOR
			Game.Logger.Log(Logger.Module.Core, message);
#elif UNITY_WEBGL
			WebGLConsoleLog(message);
#endif
		}
	}
}
