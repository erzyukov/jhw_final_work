namespace Game.Configs
{
	using System.Linq;
	using UnityEditor;

	public class ConfigEditor : Editor
	{
		const string UnitsConfigAssetName = "Units";
		const string AssetExtention = ".asset";

		protected UnitsConfig GetUnitsConfig(out string assetName)
		{
			assetName = UnitsConfigAssetName;
			string guid = AssetDatabase.FindAssets(UnitsConfigAssetName, new string[] { "Assets/Game/Data" }).FirstOrDefault();
			string path = AssetDatabase.GUIDToAssetPath(guid) + AssetExtention;
			UnitsConfig unitsConfig = AssetDatabase.LoadAssetAtPath<UnitsConfig>(path);
			
			if (unitsConfig != null)
				unitsConfig.Initialize();

			return unitsConfig;
		}
	}
}