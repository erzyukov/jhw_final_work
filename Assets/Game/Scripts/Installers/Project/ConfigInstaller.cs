namespace Game.Installers
{
	using Game.Configs;
	using UnityEngine;
	using Zenject;

	[CreateAssetMenu(fileName = "ConfigInstaller", menuName = "Installers/ConfigInstaller")]
	public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
	{
		[SerializeField] private TutorialConfig _tutorial;
		[SerializeField] private DevConfig _dev;
		[SerializeField] private MenuConfig _menu;

		[Header("Tech")]
		[SerializeField] private ScenesConfig _scenes;
		[SerializeField] private LocalizationConfig _localization;
		[SerializeField] private AudioConfig _audio;
		[SerializeField] private PrefabsConfig _prefabs;

		[Header("Balance")]
		[SerializeField] private LevelsConfig _levels;
		[SerializeField] private CurrencyConfig _currency;
		[SerializeField] private TimingsConfig _timings;
		[SerializeField] private ExperienceConfig _experience;
		[SerializeField] private EnergyConfig _energy;

		[Header("Gameplay")]
		[SerializeField] private UnitsConfig _units;
		[SerializeField] private BattleFieldConfig _battleField;
		[SerializeField] private WeaponsConfig _weapons;
		[SerializeField] private UpgradesConfig _upgrades;

		[Header("Monetization")]
		[SerializeField] private AdsConfig _ads;

		public override void InstallBindings()
		{
			_units.Initialize();
			_tutorial.Initialize();
			_upgrades.Initialize();

			Container.BindInstance(_scenes);
			Container.BindInstance(_units);
			Container.BindInstance(_battleField);
			Container.BindInstance(_levels);
			Container.BindInstance(_currency);
			Container.BindInstance(_timings);
			Container.BindInstance(_localization);
			Container.BindInstance(_tutorial);
			Container.BindInstance(_dev);
			Container.BindInstance(_experience);
			Container.BindInstance(_weapons);
			Container.BindInstance(_menu);
			Container.BindInstance(_upgrades);
			Container.BindInstance(_energy);
			Container.BindInstance(_audio);
			Container.BindInstance(_prefabs);
			Container.BindInstance(_ads);
		}
	}
}