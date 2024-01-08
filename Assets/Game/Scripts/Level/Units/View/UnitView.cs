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
        void Destroy();
    }

    [SelectionBase]
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

        public Transform ModelRendererTransform { get; private set; }

        public Transform Transform => transform;

        public Transform RendererContainer => _modelContainer;

        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        public void SetParent(Transform parent, bool worldPositionStays = false) => transform.SetParent(parent, worldPositionStays);

        public void SetActive(bool value) => gameObject.SetActive(value);

        public void SetModelRendererTransform(Transform transform) =>
            ModelRendererTransform = transform;

        public void Destroy()
        {
            SetActive(false);
            Object.Destroy(gameObject);
        }

        #endregion
    }
}
