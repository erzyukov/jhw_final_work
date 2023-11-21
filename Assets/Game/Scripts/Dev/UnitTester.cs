namespace Game
{
	using Game.Units;
	using System.ComponentModel;
	using UnityEngine;
	using Zenject;

	public class UnitTester : MonoBehaviour
    {
		[SerializeField] private float _damage = 5;

		[Inject] IUnitHealth unitHealth;

		[ContextMenu("TakeDamage")]
		public void TakeDamage()
		{
			unitHealth.TakeDamage(_damage);
		}
    }
}
