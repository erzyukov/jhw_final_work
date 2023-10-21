namespace Game.Units
{
	using Ui;
	using Utilities;
	using VContainer;
	using VContainer.Unity;
	using UniRx;
	using Game.Platoon;

	public class HeroUnitCreator : ControllerBase, IStartable
	{
		[Inject] private IHudUnitPanel _hudUnitPanel;
		[Inject] private UnitViewFactory _unitViewFactory;
		[Inject] private IPlatoon _platoon;

		public void Start()
		{
			_hudUnitPanel.UnitSelectButtonPressed
				.Subscribe(CreateUnit)
				.AddTo(this);
		}

		private void CreateUnit(Unit.Kind type)
		{
			if (_platoon.HasFreeSpace)
			{
				IUnitView view = _unitViewFactory.Create(type);
				IUnit unit = new Unit(type, view);
				_platoon.AddUnit(unit);
			}
		}
	}
}