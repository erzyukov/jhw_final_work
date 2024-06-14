namespace Game.Dev
{
	public static class WebGLDebug
	{
		public static void Log(string message)
		{
			Game.Logger.Log(Logger.Module.Core, message);
		}
	}
}
