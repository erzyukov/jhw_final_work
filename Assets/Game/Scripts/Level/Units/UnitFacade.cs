namespace Game.Units
{
	using Game.Configs;
	using UnityEngine;
	using Zenject;

	public interface IUnitFacade
	{
		string Name { get; }
		Species Species { get; }
		Transform Transform { get; }
		void SetViewParent(Transform parent);
		void DestroyView();
	}

	public class UnitFacade : IUnitFacade
	{
		//[Inject] private IUnit _unit;
		[Inject] private Species _species;
		[Inject] private IUnitView _view;
		[Inject] private UnitConfig _config;

		#region IUnitFacade

		public string Name => _config.Title;

		public Species Species => _species; // _unit.Species;

		public Transform Transform => _view.Transform;

		public void SetViewParent(Transform parent) => _view.SetParent(parent);

		public void DestroyView() => _view.Destroy();

		#endregion

		//public class Factory : PlaceholderFactory<Object, IUnitFacade> {}
		public class Factory : PlaceholderFactory<Species, UnitFacade> {}
	}

	/*
	public class UnitFactory : IFactory<Object, IUnitFacade>
	{
		[Inject] readonly DiContainer _container;

		public IUnitFacade Create(Object prefab)
		{
			IUnitFacade unitFacade = _container.InstantiatePrefabForComponent<IUnitFacade>(prefab);

			

			return unitFacade;
		}
	}
	*/
}