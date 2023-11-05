namespace Game.Platoon
{
	using Utilities;
	using Core;
	using VContainer;
	using VContainer.Unity;
	using Configs;
	using Units;
	using SpawnData = Configs.EnemySpawnConfig.SpawnData;

	public class EnemyPlatoonController : ControllerBase, IStartable, ITickable
	{
		[Inject] private IGameLevel _gameLevel;
		[Inject] private EnemyConfig _config;
		[Inject] private IPlatoon _platoon;
		[Inject] private UnitViewFactory _unitViewFactory;
		[Inject] private UnitsConfig _unitsConfig;

		private EnemySpawnConfig _spawnConfig;
		private ITimer _spawnTimer;
		
		private int _spawnIndex;
		private SpawnData _currentSpawnData;
		private PlatoonCell _targetCell;

		public void Start()
		{
			_spawnTimer = new Timer();
			_spawnConfig = _config.GetSpawnConfig(0);
			InitNextSpawnData();
		}

		public void Tick()
		{
			for (int i = 0; i < _platoon.Units.Count; i++)
				_platoon.Units[i].UpdateView();

			if (_spawnTimer.IsReady == false || _targetCell.HasUnit)
				return;

			CreateUnit();

			_spawnIndex++;

			if (_spawnIndex < _spawnConfig.SpawnOrder.Length)
				InitNextSpawnData();
		}

		private void InitNextSpawnData()
		{
			_currentSpawnData = _spawnConfig.SpawnOrder[_spawnIndex];
			_targetCell = _platoon.GetCell(_currentSpawnData.Position);
			_spawnTimer.Set(_currentSpawnData.Delay);
		}

		private void CreateUnit()
		{
			Unit.Kind type = _currentSpawnData.Type;
			IUnitView view = _unitViewFactory.Create(type);
			IUnit unit = new Unit(type, view, _unitsConfig.Units[type]);
			_platoon.AddUnit(unit, _targetCell);
		}
	}
}