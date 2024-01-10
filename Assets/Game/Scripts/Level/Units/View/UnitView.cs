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
        Transform RendererContainer { get; }
        NavMeshAgent NavMeshAgent { get; }
        Transform ModelRendererTransform { get; }

        void SetParent(Transform parent, bool worldPositionStays = false);
        void SetActive(bool value);
        void SetModelRendererTransform(Transform transform);
		void SetMergeActive(bool value);
        void Destroy();
    }

    [SelectionBase]
    public class UnitView : MonoBehaviour, IUnitView
    {
        [SerializeField] private Transform _modelContainer;
        [SerializeField] private NavMeshAgent _navMeshAgent;

		private bool _isMergeActive = true;

        private void OnTriggerEnter(Collider other)
		{
			if (_isMergeActive)
				MergeInitiated.Execute();
		}

        private void OnTriggerExit(Collider other)
		{
			if (_isMergeActive)
				MergeCanceled.Execute();
		}

        #region IUnitView

        public ReactiveCommand MergeInitiated { get; } = new ReactiveCommand();

        public ReactiveCommand MergeCanceled { get; } = new ReactiveCommand();

        public Transform ModelRendererTransform { get; private set; }

        public Transform Transform => transform;

        public Transform RendererContainer => _modelContainer;

        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        public void SetParent(Transform parent, bool worldPositionStays = false) => transform.SetParent(parent, worldPositionStays);

        public void SetActive(bool value) => gameObject.SetActive(value);

        public void SetModelRendererTransform(Transform transform) =>
            ModelRendererTransform = transform;

		public void SetMergeActive(bool value)
			=> _isMergeActive = value;

		public void Destroy()
        {
            SetActive(false);
            Object.Destroy(gameObject);
        }

        #endregion
    }
}
