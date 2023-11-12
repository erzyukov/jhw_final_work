namespace Game.Units
{
	using UnityEngine;

	public interface IUnitView
	{
		Transform ModelContainer { get; }
	}

    public class UnitView : MonoBehaviour, IUnitView
	{
		[SerializeField] private Transform _modelContainer;

		#region IUnitView

		public Transform ModelContainer => _modelContainer;

		#endregion
	}
}
