namespace Game.Units
{
	using UnityEngine;
	using Zenject;

	public interface IUnitFacade
	{
		Species Species { get; }
	}

	public class UnitFacade : IUnitFacade
	{
		//[Inject] private IUnit _unit;
		[Inject] private Species _species;

		#region IUnitFacade

		public Species Species => _species; // _unit.Species;

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