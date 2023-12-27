namespace Game.Configs
{
	using Game.Units;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit", order = (int)Config.Unit)]
	public class UnitConfig : ScriptableObject
	{
		[SerializeField] private string _titleKey;
		[SerializeField] private Sprite _icon;
		[SerializeField] private Class _class;
		[SerializeField] private float _attackRange;
		[SerializeField] private float _health;
		[SerializeField] private float _damage;
		[SerializeField] private UnitGrade[] _grades;
		[SerializeField] private bool _isDebug;

		public string TitleKey => _titleKey;
		public Sprite Icon => _icon;
		public Class Class => _class;
		public float AttackRange => _attackRange;
		public float Health => _health;
		public float Damage => _damage;
		public UnitGrade[] Grades => _grades;
		public bool IsDebug => _isDebug;
	}
}