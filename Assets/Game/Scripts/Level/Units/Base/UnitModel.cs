namespace Game.Units
{
	using UnityEngine;
	using Zenject;

	public interface IUnitModel
	{
		Transform Transform { get; }
		Renderer Renderer { get; }
	}

	public class UnitModel : MonoBehaviour, IUnitModel
	{
		[SerializeField] Renderer _renderer;

		#region IUnitModel

		public Transform Transform => transform;

		public Renderer Renderer => _renderer;

		#endregion

		public class Factory : PlaceholderFactory<Object, IUnitModel>
		{
		}
	}
}