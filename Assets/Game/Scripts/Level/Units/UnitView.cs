namespace Game.Units
{
	using UnityEngine;

	public interface IUnitView
	{
		Transform ModelContainer { get; }

		void SetParent(Transform parent);

		void Destroy();
	}

    public class UnitView : MonoBehaviour, IUnitView
	{
		[SerializeField] private Transform _modelContainer;

		#region IUnitView

		public Transform ModelContainer => _modelContainer;

		public void SetParent(Transform parent) => transform.SetParent(parent, false);

		public void Destroy() => Object.Destroy(gameObject);

		#endregion
	}
}
