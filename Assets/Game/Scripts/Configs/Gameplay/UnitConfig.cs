namespace Game.Configs
{
	using Game.Units;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit", order = (int)Config.Unit)]
	public class UnitConfig : ScriptableObject
	{
		[SerializeField] private string _title;
		[SerializeField] private Class _class;
		[SerializeField] private float _attackRange;
		[SerializeField] private UnitGrade[] _grades;

		public string Title => _title;
		public Class Class => _class;
		public float AttackRange => _attackRange;
		public UnitGrade[] Grades => _grades;
	}
}