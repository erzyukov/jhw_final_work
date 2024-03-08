namespace Game.Gameplay
{
	using Game.Configs;
	using Game.Field;
	using Game.Fx;
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
		[Inject] private IEffectsSpawner _effectsSpawner;

		private ReactiveProperty<IFieldCell> _targetCell = new();

		private List<IFieldCell> _selectedCells;
		private IUnitFacade _mergeInitiatorUnit;
		private IUnitFacade _mergeAbsorbedUnit;

		public void Initialize()
		{
			_selectedCells = new List<IFieldCell>();

			_fieldHeroFacade.Events.UnitDrag
				.Subscribe( OnUnitDrug )
				.AddTo( this );

			_targetCell
				.Subscribe( MergeProcess )
				.AddTo( this );

			_fieldHeroFacade.Events.UnitPointerDowned
				.Subscribe( OnUnitPointerDownedHandler )
				.AddTo( this );

			_fieldHeroFacade.Events.UnitPointerUped
				.Subscribe( _ => OnUnitPointerUpedHandler() )
				.AddTo( this );
		}

		private void OnUnitDrug( IUnitFacade unit )
		{
			_targetCell.Value = _fieldHeroFacade.GetCell( unit.Transform.position.xz() );
		}

		private void MergeProcess( IFieldCell cell )
		{
			if ( _mergeAbsorbedUnit != null )
			{
				_mergeAbsorbedUnit.ResetSupposedPower();
				_mergeAbsorbedUnit = null;
			}

			if (
				_mergeInitiatorUnit == null ||
				cell == null ||
				cell.HasUnit == false ||
				cell.Unit == _mergeInitiatorUnit
			)
				return;

			IUnitFacade unit = cell.Unit;

			if (
				_mergeInitiatorUnit.GradeIndex != unit.GradeIndex ||
				_mergeInitiatorUnit.Species != unit.Species
			)
				return;

			_mergeAbsorbedUnit = unit;
			unit.SetSupposedPower( GetMergingUnitPower() );
		}

		private void OnUnitPointerUpedHandler()
		{
			TryMergeUnits();
			_mergeInitiatorUnit = null;

			foreach (var cell in _selectedCells)
				cell.Deselect();

			_selectedCells.Clear();
			
			_targetCell.Value = null;
		}

		private void OnUnitPointerDownedHandler( IUnitFacade unit )
		{
			if (_unitsConfig.Units.TryGetValue( unit.Species, out UnitConfig unitConfig ) == false)
				return;

			if (unit.GradeIndex >= unitConfig.GradePrefabs.Length - 1)
				return;

			_mergeInitiatorUnit = unit;
			_selectedCells = _fieldHeroFacade.FindSameUnitCells( unit );

			foreach (var cell in _selectedCells)
				cell.Select();
		}

		private void TryMergeUnits()
		{
			if (
				_mergeInitiatorUnit == null || _mergeAbsorbedUnit == null ||
				_mergeInitiatorUnit.Species != _mergeAbsorbedUnit.Species ||
				_mergeInitiatorUnit.GradeIndex != _mergeAbsorbedUnit.GradeIndex || 
				_unitsConfig.Units.TryGetValue( _mergeInitiatorUnit.Species, out UnitConfig unitConfig ) == false ||
				_mergeInitiatorUnit.GradeIndex >= unitConfig.GradePrefabs.Length - 1
			)
				return;

			Vector2Int cellPosition = _fieldHeroFacade.GetCell(_mergeAbsorbedUnit).Position;
			Species species = _mergeAbsorbedUnit.Species;
			int gradeIndex = _mergeAbsorbedUnit.GradeIndex + 1;
			int power = GetMergingUnitPower();

			_fieldHeroFacade.RemoveUnit( _mergeInitiatorUnit );
			_fieldHeroFacade.RemoveUnit( _mergeAbsorbedUnit );
			_mergeInitiatorUnit.Destroy();
			_mergeAbsorbedUnit.Destroy();
			_mergeInitiatorUnit = null;
			_mergeAbsorbedUnit = null;

			IUnitFacade unit = _heroUnitSummoner.Summon(species, gradeIndex, power, cellPosition);

			_fieldHeroFacade.UnitsMerged.Execute( unit );

			_effectsSpawner.Spawn( VfxElement.UnitMerge, unit.Transform.position );
		}

		private int GetMergingUnitPower() =>
			_unitsConfig.GetAdditionalPower( _mergeAbsorbedUnit.GradeIndex ) +
			_mergeAbsorbedUnit.Power +
			_mergeInitiatorUnit.Power;
	}
}