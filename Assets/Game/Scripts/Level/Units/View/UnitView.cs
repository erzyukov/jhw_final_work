namespace Game.Units
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.AI;

	public interface IUnitView
	{
		ReactiveCommand MergeInitiated { get; }
		ReactiveCommand MergeCanceled { get; }

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

		private void OnTriggerEnter(Collider other) => 
			MergeInitiated.Execute();

		private void OnTriggerExit(Collider other) =>
			MergeCanceled.Execute();

		#region IUnitView

		public ReactiveCommand MergeInitiated { get; } = new ReactiveCommand();
		
		public ReactiveCommand MergeCanceled { get; } = new ReactiveCommand();

		public Transform Transform => transform;

		public Transform ModelContainer => _modelContainer;

		public NavMeshAgent NavMeshAgent => _navMeshAgent;

		public void SetParent(Transform parent) => transform.SetParent(parent, false);

		public void SetActive(bool value) => gameObject.SetActive(value);

		public void ResetPosition()
		{
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
