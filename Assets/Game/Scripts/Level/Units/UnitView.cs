namespace Game.Units
{
	using UnityEngine;
	using UnityEngine.AI;

	public interface IUnitView
	{
		Transform Transform { get; }
		Transform ModelContainer { get; }
		NavMeshAgent NavMeshAgent { get; }

		void SetParent(Transform parent);

		void Destroy();
	}

    public class UnitView : MonoBehaviour, IUnitView
	{
		[SerializeField] private Transform _modelContainer;
		[SerializeField] private NavMeshAgent _navMeshAgent;

		#region IUnitView

		public Transform Transform => transform;

		public Transform ModelContainer => _modelContainer;

		public NavMeshAgent NavMeshAgent => _navMeshAgent;

		public void SetParent(Transform parent) => transform.SetParent(parent, false);

		public void Destroy() => Object.Destroy(gameObject);

		#endregion
	}
}
