namespace Game.Units
{
	using UnityEngine;
	using Zenject;

	public interface IUnitModel
	{
		Transform Transform { get; }
		Renderer Renderer { get; }
		Transform RendererTransform { get; }
	}

	public class UnitModel : MonoBehaviour, IUnitModel
	{
		[SerializeField] Renderer _renderer;

		private void Awake()
		{
			RendererTransform = _renderer.transform;
		}

		#region IUnitModel

		public Transform Transform => transform;

		public Renderer Renderer => _renderer;

		public Transform RendererTransform { get; private set; }

		#endregion

		public class Factory : PlaceholderFactory<Object, IUnitModel>
		{
		}
	}
}