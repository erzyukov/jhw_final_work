namespace Game.Gameplay
{
	using Game.Configs;
	using Game.Field;
	using Game.Level;
	using Game.Units;
	using Game.Utilities;
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UnitMerger : ControllerBase, IInitializable
	{
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IHeroUnitSummoner _heroUnitSummoner;
		[Inject] private UnitsConfig _unitsConfig;

		private List<IFieldCell> _selectedCell;
		private IUnitFacade _mergeInitiatorUnit;
		private IUnitFacade _mergeAbsorbedUnit;

		public void Initialize()
		{
			_selectedCell = new List<IFieldCell>();

			_fieldHeroFacade.Events.UnitPointerDowned
				.Subscribe(OnUnitPointerDownedHandler)
				.AddTo(this);

			_fieldHeroFacade.Events.UnitPointerUped
				.Subscribe(_ => OnUnitPointerUpedHandler())
				.AddTo(this);

			_fieldHeroFacade.Events.UnitMergeCanceled
				.Subscribe(_ => _mergeAbsorbedUnit = null)
				.AddTo(this);

			_fieldHeroFacade.Events.UnitMergeInitiated
				.Where(unit => unit != _mergeInitiatorUnit)
				.Subscribe(OnUnitMergeInitiated)
				.AddTo(this);

			_fieldHeroFacade.Events.UnitMergeCanceled
				.Subscribe(OnUnitMergeCanceled)
				.AddTo(this);
		}

		private void OnUnitMergeInitiated(IUnitFacade unit)
		{
			if (
				_mergeInitiatorUnit == null ||
				_mergeInitiatorUnit.GradeIndex != unit.GradeIndex || 
				_mergeInitiatorUnit.Species != unit.Species
			)
				return;

			_mergeAbsorbedUnit = unit;
			unit.SetSupposedPower(GetMergingUnitPower());
		}

		private void OnUnitMergeCanceled(IUnitFacade unit)
		{
			_mergeAbsorbedUnit = null;
			unit.SetSupposedPower(0);
		}

		private void OnUnitPointerUpedHandler()
		{
			TryMergeUnits();
			_mergeInitiatorUnit = null;

			foreach (var cell in _selectedCell)
				cell.Deselect();

			_selectedCell.Clear();
		}

		private void OnUnitPointerDownedHandler(IUnitFacade unit)
		{
			if (_unitsConfig.Units.TryGetValue(unit.Species, out UnitConfig unitConfig) == false)
				return;

			if (unit.GradeIndex >= unitConfig.GradePrefabs.Length - 1)
				return;

			_mergeInitiatorUnit = unit;
			_selectedCell = _fieldHeroFacade.FindSameUnitCells(unit);

			foreach (var cell in _selectedCell)
				cell.Select();
		}

		private void TryMergeUnits()
		{
			if (_mergeInitiatorUnit == null || _mergeAbsorbedUnit == null)
				return;

			if (_mergeInitiatorUnit.Species != _mergeAbsorbedUnit.Species)
				return;

			if (_mergeInitiatorUnit.GradeIndex != _mergeAbsorbedUnit.GradeIndex)
				return;

			if (_unitsConfig.Units.TryGetValue(_mergeInitiatorUnit.Species, out UnitConfig unitConfig) == false)
				return;

			if (_mergeInitiatorUnit.GradeIndex >= unitConfig.GradePrefabs.Length - 1)
				return;

			Vector2Int cellPosition = _fieldHeroFacade.GetCell(_mergeAbsorbedUnit).Position;
			Species species = _mergeAbsorbedUnit.Species;
			int gradeIndex = _mergeAbsorbedUnit.GradeIndex + 1;
			int power = GetMergingUnitPower();

			_fieldHeroFacade.RemoveUnit(_mergeInitiatorUnit);
			_fieldHeroFacade.RemoveUnit(_mergeAbsorbedUnit);
			_mergeInitiatorUnit.Destroy();
			_mergeAbsorbedUnit.Destroy();
			_mergeInitiatorUnit = null;
			_mergeAbsorbedUnit = null;

			_heroUnitSummoner.Summon(species, gradeIndex, power, cellPosition);
			
			_fieldHeroFacade.UnitsMerged.Execute();
		}

		private int GetMergingUnitPower() =>
			_unitsConfig.GetAdditionalPower(_mergeAbsorbedUnit.GradeIndex) +
			_mergeAbsorbedUnit.Power +
			_mergeInitiatorUnit.Power;
	}
}