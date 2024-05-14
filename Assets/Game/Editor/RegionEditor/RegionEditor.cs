#if UNITY_EDITOR
namespace Game.Editor
{
	using Game;
	using Game.Configs;
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

		private VisualElement	_root;
		private LevelsConfig	_levelsConfig;

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

			InitData();
			InitFields();

			Subscribe();
		}

		private void OnDestroy()
		{
			_elementsSubscribes.ForEach( e => e.Dispose() );
			_elementsSubscribes.Clear();
		}

		private void InitData()
		{
			string guid		= AssetDatabase.FindAssets("Levels", new string[] { "Assets/Game/Data" }).FirstOrDefault();
			string path		= AssetDatabase.GUIDToAssetPath(guid);
			_levelsConfig	= AssetDatabase.LoadAssetAtPath<LevelsConfig>(path);
		}

		private void InitFields()
		{
			_regionSelect	= _root.Q<EnumField>( "RegionField" );
			_levelSelect	= _root.Q<DropdownField>( "LevelField" );
			_waveSelect		= _root.Q<DropdownField>( "WaveField" );
			_waveContainer	= _root.Q<GroupBox>("WaveElementsContainer");
		}

		private void Subscribe()
		{
			_regionSelect.RegisterValueChangedCallback( e => OnRegionSelected( (Region)e.newValue ) );
			_levelSelect.RegisterValueChangedCallback( e => OnLevelSelected( _levelSelect.choices.IndexOf(e.newValue) ) );
			_waveSelect.RegisterValueChangedCallback( e => OnWaveSelected( _waveSelect.choices.IndexOf(e.newValue) ) );
		}

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
			RefreshWave();
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

		private void OnWaveElementChanged( WaveUnit unit )
		{
			WaveConfig waveConfig	= _levelsConfig.Levels[_selectedLevelIndex].Waves[_selectedWaveIndex];
			WaveUnit[] units		= waveConfig.Units;

			var founded = units
				.Select(( u, i ) => ( u, i ))
				.Where( e => e.u.Position == unit.Position );
			
			if (founded.Any())
			{
				var r		= founded.First();

				Debug.LogWarning($">>> founded: {r.u.Position}");

				if (unit.Species == Units.Species.None)
				{
					DeleteWaveElement(ref units, r.i);
					waveConfig.Units = units;
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
				
				waveConfig.Units = units;

				Debug.LogWarning(units.Length);
			}

			EditorUtility.SetDirty( waveConfig );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.LogWarning( ">>> Saved" );
		}

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
	}
}
#endif