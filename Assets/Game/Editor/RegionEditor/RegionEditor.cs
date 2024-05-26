#if UNITY_EDITOR
namespace Game.Editor
{
	using Game;
	using Game.Configs;
	using Game.Units;
	using Sirenix.Utilities;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UniRx;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;
	using WaveUnit = Game.Configs.WaveConfig.WaveUnit;

	public class RegionEditor : EditorWindow
	{
		[SerializeField] private VisualTreeAsset _visualTreeAsset	= default;
		[SerializeField] private VisualTreeAsset _vtaWaveElement	= default;

		private VisualElement		_root;
		private LevelsConfig		_levelsConfig;
		private UnitsConfig			_unitsConfig;
		private WaveConfig			_waveConfig;
		private ExperienceConfig	_experienceConfig;
		private UpgradesConfig		_upgradesConfig;

#region Regions

		private EnumField		_regionSelect;
		private DropdownField	_levelSelect;
		private DropdownField	_waveSelect;
		private GroupBox		_waveContainer;

		private List<int>		_selectedRegionLevelIndexes;
		private List<int>		_selectedLevelWaveIndexes;

		private ReactiveProperty<LevelConfig>		_selectedLevel		= new();
		private ReactiveProperty<WaveConfig>		_selectedWave		= new();
		private int									_selectedLevelIndex;
		private int									_selectedWaveIndex;

		private List<WaveElement>			_waves					= new();
		private List<IDisposable>			_elementsSubscribes		= new();

#endregion

#region WaveStats, TotalWaveStats

		private Label			_waveRewardField;
		private Label			_waveExpirienceField;
		private Label			_waveHealthField;
		private Label			_waveDamageField;
		private Label			_totalRewardField;
		private Label			_totalExpirienceField;
		private Label			_minPlayerLevelField;
		private Label			_unitListLabel;
		private Label			_unitListLevelsLabel;
		private Label			_unitsTotalPowerLabel;
		private Label			_unitsTotalDamageLabel;
		private Label			_unitsTotalHealthLabel;
		private Label			_enemyTotalPowerLabel;
		private Label			_enemyTotalDamageLabel;
		private Label			_enemyTotalHealthLabel;
		private Label			_heroLifetimeLabel;
		private Label			_enemyLifetimeLabel;
		private Label			_totalBalanceLabel;

		private float			_totalReward = 0;
		private float			_totalExpirience = 0;
		private int				_minPlayerLevel = 0;
		private int				_playerTotalPower = 0;
		private float			_playerTotalDamage = 0;
		private float			_playerTotalHealth = 0;
		private int				_enemyTotalPower = 0;
		private float			_enemyTotalDamage = 0;
		private float			_enemyTotalHealth = 0;

#endregion

		private Dictionary<Species, int>	_maxUpgrades = new();

		[MenuItem("Game/Tools/RegionEditor")]
		public static void ShowRegionEditorWindow()
		{
			RegionEditor wnd = GetWindow<RegionEditor>();
			wnd.titleContent = new GUIContent("RegionEditor");
		}

		public void CreateGUI()
		{
			_root = rootVisualElement;

			// Instantiate UXML
			VisualElement labelFromUXML = _visualTreeAsset.Instantiate();
			_root.Add(labelFromUXML);

			_unitsConfig	= GetUnitsConfig( out _ );

			InitData();
			InitFields();

			Subscribe();
		}

		private void OnDestroy()
		{
			_elementsSubscribes.ForEach( e => e.Dispose() );
			_elementsSubscribes.Clear();
		}

#region Init

		private void InitData()
		{
			_levelsConfig		= GetConfig<LevelsConfig>( "Levels" );
			_experienceConfig	= GetConfig<ExperienceConfig>( "Experience" );
			_upgradesConfig		= GetConfig<UpgradesConfig>( "_Upgrades" );

			_upgradesConfig.Initialize();
			_upgradesConfig.UnitsUpgrades.Keys
				.ForEach( v => _maxUpgrades.Add( v, 0 ) );
		}

		private void InitFields()
		{
			_regionSelect	= _root.Q<EnumField>( "RegionField" );
			_levelSelect	= _root.Q<DropdownField>( "LevelField" );
			_waveSelect		= _root.Q<DropdownField>( "WaveField" );
			_waveContainer	= _root.Q<GroupBox>("WaveElementsContainer");

			_waveRewardField		= _root.Q<Label>( "WaveReward" );
			_waveExpirienceField	= _root.Q<Label>( "WaveExperience" );
			_waveHealthField		= _root.Q<Label>( "WaveHealth" );
			_waveDamageField		= _root.Q<Label>( "WaveAvrDamage" );
			_totalRewardField		= _root.Q<Label>( "TotalReward" );
			_totalExpirienceField	= _root.Q<Label>( "TotalExperience" );

			_minPlayerLevelField	= _root.Q<Label>( "HeroLevel" );

			_unitListLabel				= _root.Q<Label>( "UnitList" );
			_unitListLevelsLabel			= _root.Q<Label>( "UnitLevels" );
			_unitsTotalPowerLabel		= _root.Q<Label>( "UnitsTotalPower" );
			_unitsTotalDamageLabel		= _root.Q<Label>( "UnitsTotalDamage" );
			_unitsTotalHealthLabel		= _root.Q<Label>( "UnitsTotalHealth" );
			_enemyTotalPowerLabel		= _root.Q<Label>( "EnemyTotalPower" );
			_enemyTotalDamageLabel		= _root.Q<Label>( "EnemyTotalDamage" );
			_enemyTotalHealthLabel		= _root.Q<Label>( "EnemyTotalHealth" );
			_heroLifetimeLabel			= _root.Q<Label>( "HeroLifetime" );
			_enemyLifetimeLabel			= _root.Q<Label>( "EnemyLifetime" );
			_totalBalanceLabel			= _root.Q<Label>( "TotalBalance" );
		}

		private void Subscribe()
		{
			_regionSelect.RegisterValueChangedCallback( e => OnRegionSelected( (Region)e.newValue ) );
			_levelSelect.RegisterValueChangedCallback( e => OnLevelSelected( _levelSelect.choices.IndexOf(e.newValue) ) );
			_waveSelect.RegisterValueChangedCallback( e => OnWaveSelected( _waveSelect.choices.IndexOf(e.newValue) ) );
		}

#endregion

#region Wave Selection

		private void OnRegionSelected( Region region )
		{
			var levels		= _levelsConfig.Levels
				.Select( (level, index) => ( level, index) )
				.Where( r => r.level.Region == region )
				.ToList();

			_levelSelect.SetValueWithoutNotify( string.Empty );
			_waveSelect.SetValueWithoutNotify( string.Empty );
			_waveSelect.choices.Clear();

			List<string> titles			= levels.Select( r => $"Level: {r.level.Title}" ).ToList();
			_selectedRegionLevelIndexes	= levels.Select( r => r.index ).ToList();
			_levelSelect.choices		= titles;
		}

		private void OnLevelSelected( int optionIndex )
		{
			_selectedLevelIndex		= _selectedRegionLevelIndexes[optionIndex];
			_selectedLevel.Value	= _levelsConfig.Levels[_selectedLevelIndex];

			_waveSelect.SetValueWithoutNotify( string.Empty );

			List<string> titles		= _selectedLevel.Value.Waves
				.Select( (wave, index) => $"Wave {index + 1}" )
				.ToList();
			_waveSelect.choices			= titles;

			Debug.LogWarning($"{_selectedLevel.Value.Title} [{_selectedLevel.Value.Region}]");
		}

		private void OnWaveSelected( int index )
		{
			_selectedWaveIndex		= index;
			_selectedWave.Value		= _selectedLevel.Value.Waves[index];
			_waveConfig				= _levelsConfig.Levels[_selectedLevelIndex].Waves[_selectedWaveIndex];
			RefreshWave();
			UpdateStats();
		}
		private void RefreshWave()
		{
			_waveContainer.Clear();
			_elementsSubscribes.ForEach( e => e.Dispose() );
			_elementsSubscribes.Clear();

            for (int y = 4; y >= 0; y--)
            {
                for (int x = 0; x < 5; x++)
                {
					var unit = _selectedWave.Value.Units
						.Where(u => u.Position == new Vector2Int( x, y ) )
						.FirstOrDefault();

					unit.Position	= new Vector2Int( x, y );

					var waveElement = new WaveElement(new WaveElementArgs(){
						Template	= _vtaWaveElement,
						Unit		= unit,
					});

					IDisposable disposable = waveElement.ElementChanged
						.Subscribe( OnWaveElementChanged );

					_elementsSubscribes.Add( disposable );

					_waveContainer.Add( waveElement.Element );
                }
            }

			this.Repaint();
		}

#endregion

		private void OnWaveElementChanged( WaveUnit unit )
		{
			WaveUnit[] units	= _waveConfig.Units;

			var founded = units
				.Select(( u, i ) => ( u, i ))
				.Where( e => e.u.Position == unit.Position );
			
			if (founded.Any())
			{
				var r		= founded.First();

				if (unit.Species == Units.Species.None)
				{
					DeleteWaveElement(ref units, r.i);
					_waveConfig.Units = units;
					Debug.LogWarning(units.Length);
					RefreshWave();
				}
				else
				{
					units[r.i]	= unit;
				}
			}
			else
			{
				int newSize = units.Length + 1;
				Array.Resize(ref units, newSize);
				units[newSize - 1] = unit;
				
				_waveConfig.Units = units;

				Debug.LogWarning(units.Length);
			}

			EditorUtility.SetDirty( _waveConfig );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			UpdateStats();
		}


		private void UpdateStats()
		{
			UpdateWaveStats();
			UpdatePlayerStats();
			UpdateUpgrades();
			UpdatePower();
			UpdatePlayerParams();
			UpdateEnemyPower();
			UpdateEnemyParams();
			UpdateBalance();
		}

		private void UpdateWaveStats()
		{
			float waveReward		= GetWaveReward( _waveConfig );
			float waveExpirience	= GetWaveExp( _waveConfig );

			_waveRewardField.text		= waveReward.ToString();
			_waveExpirienceField.text	= waveExpirience.ToString();
			_waveHealthField.text		= GetWaveHealth( _waveConfig ).ToString();
			_waveDamageField.text		= GetWaveAvgDamage( _waveConfig ).ToString();

			_totalReward			= GetWaveTotalReward();
			_totalExpirience		= GetWaveTotalExp();

			// With current wave
			float totalReward		= _totalReward + waveReward;
			float totalExpirience	= _totalExpirience + waveExpirience;

			_totalRewardField.text		= totalReward.ToString();
			_totalExpirienceField.text	= totalExpirience.ToString();
		}

		private void UpdatePlayerStats()
		{
			_minPlayerLevel = 0;

			float lastLevelTotalExpirience	= _levelsConfig.Levels
				.Where( ( l, i ) => i < _selectedLevelIndex )
				.SelectMany( l => l.Waves)
				.Sum( w => GetWaveExp(w) );

			int i           = 0;
			int expGaind	= 0;
			int count		= _experienceConfig.HeroLevels.Length;
			
			while (expGaind <= lastLevelTotalExpirience && i < count - 1)
			{
				expGaind += _experienceConfig.HeroLevels[i].ExperienceToLevel;
				i++;
			}

			_minPlayerLevel		= Mathf.Max(1, i - 1);
			_minPlayerLevelField.text	= $"{_minPlayerLevel} ({_minPlayerLevel+1} -> {expGaind})";
		}

		private void UpdateUpgrades()
		{
			float lastLevelTotalReward	= _levelsConfig.Levels
				.Where( ( l, i ) => i < _selectedLevelIndex )
				.SelectMany( l => l.Waves)
				.Sum( w => GetWaveReward(w) );

			ClearMaxUpgrades();
			List<Species> species	= _upgradesConfig.UnitsUpgrades.Keys.ToList();
			int totalSum			= 0;

			for ( int i = 0; i < _minPlayerLevel; i++ )
			{
				for ( int j = 0; j < species.Count; j++)
				{
					Species s	= species[j];
					var uc		= _upgradesConfig.UnitsUpgrades[s];
					if (_minPlayerLevel >= uc.UnlockHeroLevel)
					{
						int price	= _upgradesConfig.UnitsUpgrades[s].Price[i];
						totalSum	+= price;

						if ( totalSum <= lastLevelTotalReward)
							_maxUpgrades[s]++;
					}
				}
			}

			_unitListLabel.text = "";
			_unitListLevelsLabel.text = "";

			_maxUpgrades.ForEach( e =>
			{
				_unitListLabel.text			+= $"{e.Key}\n";
				_unitListLevelsLabel.text	+= $"{e.Value}\n";
			});
		}

		private void UpdatePower()
		{
			var upgraded			= _maxUpgrades.Where( u => u.Value != 0);
			int totalUpgradePower	= upgraded
				.Select( u => (u.Value - 1) * _upgradesConfig.UpgradePowerBonus )
				.Sum();
			int avgUpgradePower		= totalUpgradePower / upgraded.Count();

			int initCount					= (_selectedWaveIndex + 1) * 2;

			int firstGradeCount		= initCount;
			int secondGradeCount	= initCount / 2;
			int thirdGradeCount		= initCount / 4;
			int fourthGradeCount	= initCount / 8;

			_playerTotalPower =
				firstGradeCount * avgUpgradePower +
				secondGradeCount * _unitsConfig.AdditionalMergeGradePower[ 0 ] +
				thirdGradeCount * _unitsConfig.AdditionalMergeGradePower[ 1 ] +
				fourthGradeCount * _unitsConfig.AdditionalMergeGradePower[ 2 ];

			_unitsTotalPowerLabel.text	= $"{_playerTotalPower}";
		}

		private void UpdatePlayerParams()
		{
			var upgraded				= _maxUpgrades.Where( u => u.Value != 0);
			int count					= upgraded.Count();
			float damageSum				= 0;
			float damageMultiplierSum	= 0;
			float healthSum				= 0;
			float healthMultiplierSum	= 0;

			upgraded.ForEach( u =>
			{
				damageSum				+= _unitsConfig.Units[ u.Key ].Damage / _unitsConfig.Units[ u.Key ].AttackDelay;
				damageMultiplierSum		+= _unitsConfig.Units[ u.Key ].DamagePowerMultiplier / _unitsConfig.Units[ u.Key ].AttackDelay;
				healthSum				+= _unitsConfig.Units[ u.Key ].Health;
				healthMultiplierSum		+= _unitsConfig.Units[ u.Key ].HealthPowerMultiplier;
			});

			_playerTotalDamage	= damageSum / count + damageMultiplierSum / count * _playerTotalPower;
			_playerTotalHealth	= healthSum / count + healthMultiplierSum / count * _playerTotalPower;

			_unitsTotalDamageLabel.text = Mathf.RoundToInt(_playerTotalDamage).ToString();
			_unitsTotalHealthLabel.text = Mathf.RoundToInt(_playerTotalHealth).ToString();
		}

		private void UpdateEnemyPower()
		{
			_enemyTotalPower	= 0;
			_selectedWave.Value.Units.ForEach( u =>
			{
				_enemyTotalPower += u.Power;
			});
			_enemyTotalPowerLabel.text		= _enemyTotalPower.ToString();
		}

		private void UpdateEnemyParams()
		{
			var units		= _selectedWave.Value.Units.Select( u => u.Species ).Distinct();

			int count					= units.Count();
			float damageSum				= 0;
			float damageMultiplierSum	= 0;
			float healthSum				= 0;
			float healthMultiplierSum	= 0;

			units.Select( s => _unitsConfig.Units[ s ] )
				.ForEach( c =>
				{
					damageSum				+= c.Damage / c.AttackDelay;
					damageMultiplierSum		+= c.DamagePowerMultiplier / c.AttackDelay;
					healthSum				+= c.Health;
					healthMultiplierSum		+= c.HealthPowerMultiplier;
				});

			_enemyTotalDamage	= damageSum / count + damageMultiplierSum / count * _enemyTotalPower;
			_enemyTotalHealth	= healthSum / count + healthMultiplierSum / count * _enemyTotalPower;

			_enemyTotalDamageLabel.text = Mathf.RoundToInt(_enemyTotalDamage).ToString();
			_enemyTotalHealthLabel.text = Mathf.RoundToInt(_enemyTotalHealth).ToString();
		}

		private void UpdateBalance()
		{
			float hl		= _playerTotalHealth / _enemyTotalDamage;
			float el		= _enemyTotalHealth / _playerTotalDamage;
			float balance	= hl - el;

			_heroLifetimeLabel.text		= hl.ToString();
			_enemyLifetimeLabel.text	= el.ToString();
			_totalBalanceLabel.text		= balance.ToString();
		}

		private void ClearMaxUpgrades()
		{
			_upgradesConfig.UnitsUpgrades.Keys
				.ForEach( v => _maxUpgrades[v] = 0 );
		}

#region Calculation

		private float GetWaveReward( WaveConfig wave )
		{
			return wave.Units.Sum(unit => 
				Mathf.CeilToInt(
					_unitsConfig.Units[unit.Species].SoftReward + 
					_unitsConfig.Units[unit.Species].SoftRewardPowerMultiplier * unit.Power
				)
			);
		}

		private float GetWaveExp( WaveConfig wave )
		{
			return wave.Units.Sum(unit =>
				Mathf.CeilToInt(
					_unitsConfig.Units[unit.Species].Experience +
					_unitsConfig.Units[unit.Species].ExperiencePowerMultiplier * unit.Power
				)
			);
		}

		private float GetWaveHealth( WaveConfig wave )
		{
			return wave.Units.Sum(unit =>
				Mathf.CeilToInt(
					_unitsConfig.Units[unit.Species].Health +
					_unitsConfig.Units[unit.Species].HealthPowerMultiplier * unit.Power
				)
			);
		}

		private float GetWaveAvgDamage( WaveConfig wave )
		{
			return wave.Units.Sum(unit =>
				Mathf.Round(
					(_unitsConfig.Units[unit.Species].Damage +
					_unitsConfig.Units[unit.Species].DamagePowerMultiplier * unit.Power) /
					_unitsConfig.Units[unit.Species].AttackDelay *
					100
				) / 100
			);
		}

		private float GetWaveTotalReward()		=> GetWaveTotalParam( GetWaveReward );

		private float GetWaveTotalExp()			=> GetWaveTotalParam( GetWaveExp );

		private delegate float CalcWaveParamDelegate( WaveConfig wave );
		private float GetWaveTotalParam( CalcWaveParamDelegate Calc )
		{
			float sum = 0;
			sum	+= _levelsConfig.Levels
				.Where( ( l, i ) => i < _selectedLevelIndex )
				.SelectMany( l => l.Waves)
				.Sum( w => Calc(w) );

			sum	+= _levelsConfig.Levels
				.Where( ( l, i ) => i == _selectedLevelIndex )
				.SelectMany( l => l.Waves)
				.Where( ( w, i ) => i < _selectedWaveIndex )
				.Sum( w => Calc(w) );

			return sum;
		}

#endregion

		private void DeleteWaveElement( ref WaveUnit[] units, int index )
		{
			WaveUnit[] newUnits = new WaveUnit[units.Length - 1];
			int newIndex = 0;
			units.Where(( u, i ) => i != index ).ForEach( u =>
			{
				newUnits[newIndex] = u;
				newIndex++;
			} );

			units = newUnits;
		}


#region Misc

		const string UnitsConfigAssetName = "Units";
		const string AssetExtention = ".asset";

		protected UnitsConfig GetUnitsConfig(out string assetName)
		{
			assetName = UnitsConfigAssetName;
			string guid = AssetDatabase.FindAssets(UnitsConfigAssetName, new string[] { "Assets/Game/Data" }).FirstOrDefault();
			string path = AssetDatabase.GUIDToAssetPath(guid) + AssetExtention;
			UnitsConfig unitsConfig = AssetDatabase.LoadAssetAtPath<UnitsConfig>(path);
			
			return unitsConfig;
		}

		protected T GetConfig<T>(string assetName) where T : ScriptableObject
		{
			string guid = AssetDatabase.FindAssets(assetName, new string[] { "Assets/Game/Data" }).FirstOrDefault();
			string path = AssetDatabase.GUIDToAssetPath(guid);
			
			return AssetDatabase.LoadAssetAtPath<T>(path);
		}

#endregion
	}
}
#endif