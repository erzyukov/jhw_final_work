namespace Game.Units
{
    using UnityEngine;
    using UnityEngine.AI;

    public interface IUnitView
    {
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

        #region IUnitView

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
