#if UNITY_EDITOR
namespace Game.Editor
{
	using Game.Configs;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.Build;
	using UnityEditor.Build.Reporting;
	using UnityEngine;

	class CustomBuildProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder { get { return 0; } }

		public void OnPreprocessBuild(BuildReport report)
		{
			UpdateBundleVersionCode();
			
			Debug.Log("CustomBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
		}

		public void UpdateBundleVersionCode()
		{
			string guid			= AssetDatabase.FindAssets("Development", new string[] { "Assets/Game/Data" }).FirstOrDefault();
			string path			= AssetDatabase.GUIDToAssetPath(guid);
			DevConfig config	= AssetDatabase.LoadAssetAtPath<DevConfig>(path);

			config.BundleVersionCode	= PlayerSettings.Android.bundleVersionCode;

			EditorUtility.SetDirty( config );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log( $"Updated BundleVersionCode = {config.BundleVersionCode}" );
		}
	}
}
#endif