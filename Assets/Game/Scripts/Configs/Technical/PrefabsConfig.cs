namespace Game.Configs
{
	using Game.Fx;
	using Game.Ui;
	using Sirenix.OdinInspector;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Prefabs", menuName = "Configs/Prefabs", order = (int)Config.Prefabs)]
	public class PrefabsConfig : SerializedScriptableObject
	{
		[Header("UI")]
		public UiUpgradeUnitView UpgradeUnit;

		[Header("Bullet / Technical")]
		[SerializeField] private Dictionary<VfxElement, PooledParticleFx> _vfxPrefabs = new Dictionary<VfxElement, PooledParticleFx>();

		public Dictionary<VfxElement, PooledParticleFx> VfxPrefabs => _vfxPrefabs;

		public PooledParticleFx GetVfx(VfxElement type)
		{
			if (_vfxPrefabs.TryGetValue(type, out PooledParticleFx element))
				return element;

			return null;
		}
	}
}