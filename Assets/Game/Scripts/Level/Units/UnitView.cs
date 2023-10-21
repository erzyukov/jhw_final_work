namespace Game.Units
{
	using UnityEngine;

	public interface IUnitView
	{
		GameObject GameObject { get; }
		void SetParent(Transform transform);
	}

	public class UnitView : MonoBehaviour, IUnitView
	{
		public GameObject GameObject => gameObject;

		public void SetParent(Transform transform) =>
			this.transform.SetParent(transform, false);
	}
}
