namespace Game.Units
{
	using Configs;
	using VContainer;
	using UnityEngine;

	public class UnitViewFactory
	{
		[Inject] UnitsConfig _unitsConfig;

		public UnitView Create(Unit.Type type)
		{
			UnitView prefab = _unitsConfig.GetPrefab(type);
			return Object.Instantiate(prefab);
		}
	}
}