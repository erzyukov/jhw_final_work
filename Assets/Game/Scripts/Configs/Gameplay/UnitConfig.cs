namespace Game.Configs
{
	using Game.Units;
	using System;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit", order = (int)Config.Unit)]
	public class UnitConfig : ScriptableObject
	{
		[SerializeField] private string _title;
		[SerializeField] private Class _class;
		[SerializeField] private float _attackRange;
		[SerializeField] private Grade[] _grades;

		public string Title => _title;
		public Class Class => _class;
		public float AttackRange => _attackRange;
		public Grade[] Grades => _grades;

		[Serializable]
		public struct Grade
		{
			public GameObject Prefab;
			public float AttackDelay;
			public float Damage;
			public float Health;
		}
	}
}