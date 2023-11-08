namespace Game.Battle
{
	using Configs;
	using Core;
	using System.Collections.Generic;
	using Zenject;
	using Kind = Units.Unit.Kind;

	public interface ITargetFinderSelector
	{
		ITargetFinder GetTargetFinder(Kind type);
	}

	public class TargetFinderSelector : IInitializable, ITargetFinderSelector
	{
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private BattleFieldConfig _battleFieldConfig;

		private Dictionary<Kind, TargetFinder> _targetFinders;

		public void Initialize()
		{
			_targetFinders = new Dictionary<Kind, TargetFinder>();

			foreach (var unit in _unitsConfig.Units)
            {
				switch (unit.Key)
				{
					case Kind.HeavyTank:
						_targetFinders.Add(unit.Key, new HeavyTankTargetFinder(_battleFieldConfig.TeamFieldSize));
						break;

					case Kind.LightTank:
						_targetFinders.Add(unit.Key, new LightTankTargetFinder(_battleFieldConfig.TeamFieldSize));
						break;

					case Kind.Howitzer:
						_targetFinders.Add(unit.Key, new HowitzerTargetFinder(_battleFieldConfig.TeamFieldSize));
						break;

					default:
						_targetFinders.Add(unit.Key, null);
						break;
				}
			}
        }

		public ITargetFinder GetTargetFinder(Kind type) => _targetFinders[type];
	}
}
