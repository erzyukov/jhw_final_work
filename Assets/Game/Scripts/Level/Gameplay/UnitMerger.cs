namespace Game.Gameplay
{
	using Game.Field;
	using Game.Units;
	using Game.Utilities;
	using System.Collections.Generic;
	using UniRx;
	using Zenject;

	public class UnitMerger : ControllerBase, IInitializable
	{
		[Inject] private IFieldHeroFacade _fieldHeroFacade;

		List<IFieldCell> _selectedCell;

		public void Initialize()
		{
			_fieldHeroFacade.Events.UnitPointerDowned
				.Subscribe(OnUnitPointerDownedHandler)
				.AddTo(this);

			_fieldHeroFacade.Events.UnitPointerUped
				.Subscribe(_ => OnUnitPointerUpedHandler())
				.AddTo(this);
		}

		private void OnUnitPointerUpedHandler()
		{
			foreach (var cell in _selectedCell)
				cell.Deselect();

			_selectedCell.Clear();
		}

		private void OnUnitPointerDownedHandler(IUnitFacade unit)
		{
			_selectedCell = _fieldHeroFacade.FindSameUnitCells(unit);

			foreach (var cell in _selectedCell)
				cell.Select();
		}
	}
}