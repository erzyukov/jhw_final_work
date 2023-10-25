namespace Game.Battle
{
	using Configs;
	using Core;
	using System.Collections.Generic;
	using VContainer;
	using VContainer.Unity;
	using Kind = Units.Unit.Kind;

	public interface ITargetFinderSelector
	{
		ITargetFinder GetTargetFinder(Kind type);
	}

	public class TargetFinderSelector : IStartable, ITargetFinderSelector
	{
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private IGameLevel _gameLevel;

		private Dictionary<Kind, TargetFinder> _targetFinders;

		public void Start()
		{
			_targetFinders = new Dictionary<Kind, TargetFinder>();

			foreach (var unit in _unitsConfig.Units)
            {
				switch (unit.Key)
				{
					case Kind.HeavyTank:
						_targetFinders.Add(unit.Key, new HeavyTankTargetFinder(_gameLevel.PlatoonSize));
						break;

					case Kind.LightTank:
						_targetFinders.Add(unit.Key, new LightTankTargetFinder(_gameLevel.PlatoonSize));
						break;

					case Kind.Howitzer:
						_targetFinders.Add(unit.Key, new HowitzerTargetFinder(_gameLevel.PlatoonSize));
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
