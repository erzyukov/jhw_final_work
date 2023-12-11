namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using UniRx;
	using UnityEngine;
	using UnityEngine.AI;
	using Zenject;

	public interface IUnitView
	{
		ReactiveCommand MergeInitiated { get; }
		ReactiveCommand MergeCanceled { get; }

		Transform Transform { get; }
		Transform ModelContainer { get; }
		NavMeshAgent NavMeshAgent { get; }
		Transform ModelRendererTransform { get; }

		void SetParent(Transform parent);
		void SetActive(bool value);
		void SetModelHeight(float value);
		void SetModelRendererTransform(Transform transform);
		void ResetPosition();
		void Destroy();
	}

	[SelectionBase]
    public class UnitView : MonoBehaviour, IUnitView
	{
		[SerializeField] private Transform _modelContainer;
		[SerializeField] private Transform _uiHealthCanvas;
		[SerializeField] private NavMeshAgent _navMeshAgent;

		private void OnTriggerEnter(Collider other) => 
			MergeInitiated.Execute();

		private void OnTriggerExit(Collider other) =>
			MergeCanceled.Execute();

		#region IUnitView

		public ReactiveCommand MergeInitiated { get; } = new ReactiveCommand();
		
		public ReactiveCommand MergeCanceled { get; } = new ReactiveCommand();
		
		public Transform ModelRendererTransform { get; private set; }

		public Transform Transform => transform;

		public Transform ModelContainer => _modelContainer;

		public NavMeshAgent NavMeshAgent => _navMeshAgent;

		public void SetParent(Transform parent) => transform.SetParent(parent, false);

		public void SetActive(bool value) => gameObject.SetActive(value);

		public void SetModelHeight(float value) =>
			_uiHealthCanvas.localPosition = _uiHealthCanvas.localPosition.WithY(value);

		public void SetModelRendererTransform(Transform transform) =>
			ModelRendererTransform = transform;

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
