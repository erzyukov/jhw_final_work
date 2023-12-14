namespace Game.Configs
{
	using Game.Units;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit", order = (int)Config.Unit)]
	public class UnitConfig : ScriptableObject
	{
		[SerializeField] private string _title;
		[SerializeField] private Sprite _icon;
		[SerializeField] private Class _class;
		[SerializeField] private float _attackRange;
		[SerializeField] private UnitGrade[] _grades;
		[SerializeField] private bool _isDebug;

		public string Title => _title;
		public Sprite Icon => _icon;
		public Class Class => _class;
		public float AttackRange => _attackRange;
		public UnitGrade[] Grades => _grades;
		public bool IsDebug => _isDebug;
	}
}