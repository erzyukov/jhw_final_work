namespace Game.Gameplay
{
	using Game.Core;
	using Game.Field;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using UniRx;
	using UnityEngine;
	using Zenject;
	using System.Linq;
	using System.Collections.Generic;
	using Game.Level;

	public class TacticalStageHandler : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IFieldEnemyFacade _fieldEnemyFacade;
		[Inject] private GameProfile _gameProfile;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IUnitSpawner _unitSpawner;

		public void Initialize()
		{
			_gameLevel.LevelLoading
				.Subscribe(_ => OnGameLevelLoadingHandler())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageHandler())
				.AddTo(this);

			_fieldHeroFacade.Events.UnitAdded
				.Subscribe(OnUnitAddedHandler)
				.AddTo(this);

			_fieldHeroFacade.Events.UnitRemoved
				.Subscribe(OnUnitRemovedHandler)
				.AddTo(this);

			_fieldHeroFacade.Events.UnitDropped
				.Subscribe(unit => unit.ResetPosition())
				.AddTo(this);
		}

		private void OnGameLevelLoadingHandler()
		{
			if (_gameProfile.HeroField.Units.Count == 0)
				return;

			LoadUnitsToField();
		}

		private void LoadUnitsToField()
		{
			List<HeroFieldProfile.Unit> savedUnits = _gameProfile.HeroField.Units;
			_gameProfile.HeroField.Units = new List<HeroFieldProfile.Unit>();

			foreach (var savedUnit in savedUnits)
			{
				IUnitFacade unit = _unitSpawner.SpawnHeroUnit(savedUnit.Species, savedUnit.GradeIndex);
				_fieldHeroFacade.AddUnit(unit, savedUnit.Position);
			}
		}

		private void OnTacticalStageHandler()
		{
			_fieldHeroFacade.SetFieldRenderEnabled(true);
			_fieldHeroFacade.SetDraggableActive(true);
			_fieldHeroFacade.ResetAlives();
			_fieldEnemyFacade.SetFieldRenderEnabled(true);

			foreach (var unit in _fieldHeroFacade.Units)
				unit.Reset();
		}

		private void OnUnitAddedHandler(IUnitFacade unit)
		{
			unit.SetDraggableActive(true);

			Vector2Int position = _fieldHeroFacade.GetCell(unit).Position;
			HeroFieldProfile.Unit profileUnit = new HeroFieldProfile.Unit()
			{
				Species = unit.Species,
				GradeIndex = unit.GradeIndex,
				Position = (SVector2Int)position
			};
			_gameProfile.HeroField.Units.Add(profileUnit);
			_gameProfileManager.Save();
		}

		private void OnUnitRemovedHandler(IUnitFacade unit)
		{
			Vector2Int position = _fieldHeroFacade.GetCell(unit).Position;
			HeroFieldProfile.Unit unitToRemove = _gameProfile.HeroField.Units
				.Where(profileUnit => profileUnit.Position == position)
				.First();
			_gameProfile.HeroField.Units.Remove(unitToRemove);
			_gameProfileManager.Save();
		}
	}
}