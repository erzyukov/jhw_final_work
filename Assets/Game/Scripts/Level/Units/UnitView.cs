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
		void SetActive(bool value);
		void ResetPosition();
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

		public void SetActive(bool value) => gameObject.SetActive(value);

		public void ResetPosition()
		{
			//transform.SetParent(transform.parent, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}

		public void Destroy()
		{
			SetActive(false);
			Object.Destroy(gameObject);
		}

		#endregion
	}
}
