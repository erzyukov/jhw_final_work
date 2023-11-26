namespace Game.Gameplay
{
	using Game.Core;
	using Game.Field;
	using Game.Units;
	using Game.Utilities;
	using System;
	using System.Collections.Generic;
	using UniRx;
	using Zenject;

	public class TacticalStageHandler : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IFieldEnemyFacade _fieldEnemyFacade;

		Dictionary<IUnitFacade, IDisposable> _unitsLifetime = new Dictionary<IUnitFacade, IDisposable>();

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageHandler())
				.AddTo(this);

			Observable.Merge(
					_fieldHeroFacade.Units.ObserveAdd().Select(element => element.Value),
					_fieldHeroFacade.Units.ObserveReplace().Select(element => element.NewValue)
				)
				.Subscribe(unit => OnUnitAddedHandler(unit))
				.AddTo(this);

			Observable.Merge(
					_fieldHeroFacade.Units.ObserveRemove().Select(element => element.Value),
					_fieldHeroFacade.Units.ObserveReplace().Select(element => element.OldValue)
				)
				.Subscribe(unit => OnUnitRemovedHandler(unit))
				.AddTo(this);

			_fieldHeroFacade.Units.ObserveReset()
				.Subscribe(_ => OnUnitsRemovedHandler())
				.AddTo(this);
		}

		private void OnUnitsRemovedHandler()
		{
            foreach (var unit in _unitsLifetime)
				unit.Value.Dispose();

			_unitsLifetime.Clear();
		}

		private void OnUnitRemovedHandler(IUnitFacade unit)
		{
			_unitsLifetime[unit].Dispose();
			_unitsLifetime.Remove(unit);
		}

		private void OnUnitAddedHandler(IUnitFacade unit)
		{
			unit.SetDraggableActive(true);
			IDisposable unitLifetime = unit.Dropped.Subscribe(_ => unit.ResetPosition());
			_unitsLifetime.Add(unit, unitLifetime);
		}

		private void OnTacticalStageHandler()
		{
			_fieldHeroFacade.SetFieldRenderEnabled(true);
			_fieldHeroFacade.SetDraggableActive(true);
			_fieldEnemyFacade.SetFieldRenderEnabled(true);

			foreach (var unit in _fieldHeroFacade.Units)
				unit.Reset();
		}
	}
}