namespace Game.Units
{
	using System;
	using UnityEngine;

	[Serializable]
	public struct UnitGrade
	{
		public GameObject Prefab;
		public float AttackDelay;
		public float Damage;
		public float Health;
		public int SoftCurrencyReward;
		public int ExperienceReward;
	}
}