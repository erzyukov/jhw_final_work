namespace Game.Platoon
{
	using Utilities;
	using Units;
	using Unit = Units.Unit;
	using Ui;
	using VContainer;
	using VContainer.Unity;
	using UniRx;
	using System.Collections.Generic;

	public class HeroPlatoonController : ControllerBase, IStartable
	{
		[Inject] private IPlatoon _platoon;
		[Inject] private IHudUnitPanel _hudUnitPanel;
		[Inject] private UnitViewFactory _unitViewFactory;

		private Dictionary<IUnit, CompositeDisposable> _unitDisposable = new Dictionary<IUnit, CompositeDisposable>();

		public void Start()
		{
			_hudUnitPanel.UnitSelectButtonPressed
				.Subscribe(CreateUnit)
				.AddTo(this);

			_platoon.UnitRemoved
				.Subscribe(OnUnitRemoved)
				.AddTo(this);
		}

		private void CreateUnit(Unit.Kind type)
		{
			if (_platoon.HasFreeSpace)
			{
				IUnitView view = _unitViewFactory.Create(type);
				IHeroUnit unit = new HeroUnit(type, view);
				_platoon.AddUnit(unit);

				SubscribeUnit(unit);
			}
		}

		private void SubscribeUnit(IHeroUnit unit)
		{
			_unitDisposable.Add(unit, new CompositeDisposable());
			PlatoonCell platoonCell = _platoon.GetPlatoonCell(unit.Position);

			unit.Rised
				.Subscribe(_ => OnUnitRised(unit))
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

			unit.PutDowned
				.Subscribe(_ => OnUnitPutDowned(unit))
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

			unit.Focused
				.Subscribe(_ => platoonCell.SelectCell())
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

			unit.Blured
				.Subscribe(_ => platoonCell.DeselectCell())
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

		}

		private void OnUnitRemoved(IUnit unit) =>
			_unitDisposable[unit].Dispose();

		private void OnUnitRised(IUnit unit)
		{
			
		}

		private void OnUnitPutDowned(IUnit unit)
		{
			
		}

	}
}