namespace Game.Units
{
	using Configs;
	using VContainer;
	using UnityEngine;

	public class UnitViewFactory
	{
		[Inject] UnitsConfig _unitsConfig;

		public IUnitView Create(Unit.Kind type)
		{
			UnitView prefab = _unitsConfig.Units[type].Prefab;
			return Object.Instantiate(prefab);
		}
	}
}