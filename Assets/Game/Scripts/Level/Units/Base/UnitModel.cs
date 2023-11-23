namespace Game.Units
{
	using UnityEngine;
	using Zenject;

	public interface IUnitModel
	{
		Transform Transform { get; }
	}

	public class UnitModel : MonoBehaviour, IUnitModel
	{
		#region IUnitModel

		public Transform Transform => transform;

		#endregion

		public class Factory : PlaceholderFactory<Object, IUnitModel>
		{
		}
	}
}