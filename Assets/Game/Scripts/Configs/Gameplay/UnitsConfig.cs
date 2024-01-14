namespace Game.Configs
{
	using Game.Units;
	using System;
	using UnityEngine;
	using System.Collections.Generic;

	[CreateAssetMenu(fileName = "Units", menuName = "Configs/Units", order = (int)Config.Units)]
	public class UnitsConfig : ScriptableObject
	{
		[Header("Visual")]
		[SerializeField] private GameObject _unitPrefab;
		[SerializeField] private float _uiHealthIndent;
		[SerializeField] private Sprite[] _gradeSprites;
		[SerializeField] private DamageFx _damageFxPrefab;
		[SerializeField] private int _damageFxPoolSize;
		[SerializeField] private Color _damageFxHeroUnitColor;
		[SerializeField] private Color _damageFxEnemyUnitColor;

		[Header("Gameplay")]
		[SerializeField] private float _speed;
		[Tooltip("Seconds to full rotate (360gr)")]
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private UnitData[] _units;

		[Header("Hero")]
		[SerializeField] private List<Species> _heroUnits;

		[Header("Merge")]
		[SerializeField] private int[] _additionalMergeGradePower;

		[Header("Dev")]
		[SerializeField] private List<Species> _heroDefaultSquad;

		private Dictionary<Species, UnitConfig> _unitData;

		public GameObject UnitPrefab => _unitPrefab;
		public float UiHealthIndent => _uiHealthIndent;
		public Sprite[] GradeSprites => _gradeSprites;
		public DamageFx DamageFxPrefab => _damageFxPrefab;
		public int DamageFxPoolSize => _damageFxPoolSize;
		public Color DamageFxHeroUnitColor => _damageFxHeroUnitColor;
		public Color DamageFxEnemyUnitColor => _damageFxEnemyUnitColor;
		public float Speed => _speed;
		public float RotationSpeed => _rotationSpeed;
		public Dictionary<Species, UnitConfig> Units => _unitData;
		public List<Species> HeroDefaultSquad => _heroDefaultSquad;
		public List<Species> HeroUnits => _heroUnits;

		public void Initialize()
		{
			_unitData = new Dictionary<Species, UnitConfig>();
            foreach (var unit in _units)
				_unitData.Add(unit.Species, unit.Config);
		}
		
		public int GetAdditionalPower(int gradeIndex)
		{
			if (gradeIndex >= _additionalMergeGradePower.Length)
				return 0;

			return _additionalMergeGradePower[gradeIndex];
		}

		[Serializable]
		public struct UnitData
		{
			public Species Species;
			public UnitConfig Config;
		}
    }
}