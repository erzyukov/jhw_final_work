﻿namespace Game.Configs
{
	using Game.Units;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit", order = (int)Config.Unit)]
	public class UnitConfig : ScriptableObject
	{
		[SerializeField] private string _titleKey;
		[SerializeField] private Sprite _icon;

		[Header("Combat")]
		[SerializeField] private Class _class;
		[SerializeField] private float _attackRange;
		[SerializeField] private float _attackDelay;
		[Space]
		[SerializeField] private float _health;
		[SerializeField] private float _damage;
		[SerializeField] private float _healthPowerMultiplier;
		[SerializeField] private float _damagePowerMultiplier;

		[Header("Reward")]
		[SerializeField] private float _softReward;
		[SerializeField] private float _experience;
		[SerializeField] private float _softRewardPowerMultiplier;
		[SerializeField] private float _experiencePowerMultiplier;

		[Header("View")]
		[SerializeField] GameObject[] _gradePrefabs;

		[Header("Debug")]
		[SerializeField] private bool _isDebug;

		public string TitleKey => _titleKey;
		public Sprite Icon => _icon;
		public Class Class => _class;
		public float AttackRange => _attackRange;
		public float AttackDelay => _attackDelay;
		public float Health => _health;
		public float Damage => _damage;
		public float HealthPowerMultiplier => _healthPowerMultiplier;
		public float DamagePowerMultiplier => _damagePowerMultiplier;
		public float SoftReward => _softReward;
		public float Experience => _experience;
		public float SoftRewardPowerMultiplier => _softRewardPowerMultiplier;
		public float ExperiencePowerMultiplier => _experiencePowerMultiplier;
		public GameObject[] GradePrefabs => _gradePrefabs;
		public bool IsDebug => _isDebug;
	}
}