namespace Game.Configs
{
	using Game.Units;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit", order = (int)Config.Unit)]
	public class UnitConfig : ScriptableObject
	{
		[SerializeField] private string _title;
		[SerializeField] private UnitView _prefab;
		[SerializeField] private float _shootDelay;
		[SerializeField] private float _damage;
		[SerializeField] private float _health;

		public string Title => _title;
		public UnitView Prefab => _prefab;
		public float ShootDelay => _shootDelay;
		public float Damage => _damage;
		public float Health => _health;
	}
}