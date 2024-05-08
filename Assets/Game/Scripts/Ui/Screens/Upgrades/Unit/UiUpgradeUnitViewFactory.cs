namespace Game.Ui
{
	using Game.Configs;
	using Game.Units;
	using UnityEngine;
	using Zenject;

	public class UiUpgradeUnitViewFactory : IFactory<UiUpgradeUnitViewFactory.Args, IUiUpgradeUnitView>
	{
		public struct Args
		{
			public Species			Species;
			public UnitConfig		Config;
		}

		[Inject] PrefabsConfig		_prefabsConfig;
		[Inject] DiContainer		_container;

		public IUiUpgradeUnitView Create( Args args )
		{
			_container.Unbind<Args>();
			_container.BindInstance( args );

			UiUpgradeUnitView prefab		= _prefabsConfig.UpgradeUnit;
			IUiUpgradeUnitView itemView		= _container.InstantiatePrefabForComponent<IUiUpgradeUnitView>( prefab );

			return itemView;
		}
	}
}
