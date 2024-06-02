namespace Game.Configs
{
	using Game.Units;
	using Game.Weapon;
	using UnityEngine;
	using UnityEngine.Localization;

	[CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit", order = (int)Config.Unit)]
	public class UnitConfig : ScriptableObject
	{
		[SerializeField] private LocalizedString _name;
		[SerializeField] private Sprite _icon;
		[SerializeField] private Sprite _fullLength;

		[Header("Combat")]
		[SerializeField] private Class _class;
		[SerializeField] private float _attackRange;
		[SerializeField] private float _attackDelay;
		[SerializeField] private ProjectileType _projectileType;
		public float			AoeRange;
		public VfxElement		DamageVfx;

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

		public string Name => _name.GetLocalizedString();
		public Sprite Icon => _icon;
		public Sprite FullLength => _fullLength;

		public Class Class => _class;
		public float AttackRange => _attackRange;
		public float AttackDelay => _attackDelay;
		public ProjectileType ProjectileType => _projectileType;
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

// TODO: convert private fields to public (for config)
/*
		private void OnValidate()
		{
			// string path         = AssetDatabase.GetAssetPath( this );

			//if (string.IsNullOrWhiteSpace( path ))
				//return;

			//Debug.LogWarning(path);

			//UnitConfig unitConfig = AssetDatabase.LoadAssetAtPath<UnitConfig>(path);

			DamageRange = _damageRange;
			//unitConfig.DamageRange = _damageRange;
			//unitConfig.Name = _name;

			EditorUtility.SetDirty( this );
			//AssetDatabase.SaveAssets();
			//AssetDatabase.Refresh();
		}
*/
	}
}