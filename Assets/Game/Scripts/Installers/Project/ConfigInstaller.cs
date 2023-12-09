namespace Game.Installers
{
	using Game.Configs;
	using UnityEngine;
	using Zenject;

	[CreateAssetMenu(fileName = "ConfigInstaller", menuName = "Installers/ConfigInstaller")]
	public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
	{
		[SerializeField] private ScenesConfig _scenes;
		[SerializeField] private UnitsConfig _units;
		[SerializeField] private BattleFieldConfig _battleField;
		[SerializeField] private EnemyConfig _enemy;
		[SerializeField] private LevelsConfig _levels;
		[SerializeField] private CurrencyConfig _currency;
		[SerializeField] private TimingsConfig _timings;
		[SerializeField] private LocalizationConfig _localization;
		[SerializeField] private TutorialConfig _tutorial;
		[SerializeField] private DevConfig _dev;

		public override void InstallBindings()
		{
			_units.Initialize();
			_tutorial.Initialize();

			Container.BindInstance(_scenes);
			Container.BindInstance(_units);
			Container.BindInstance(_battleField);
			Container.BindInstance(_enemy);
			Container.BindInstance(_levels);
			Container.BindInstance(_currency);
			Container.BindInstance(_timings);
			Container.BindInstance(_localization);
			Container.BindInstance(_tutorial);
			Container.BindInstance(_dev);
		}
	}
}