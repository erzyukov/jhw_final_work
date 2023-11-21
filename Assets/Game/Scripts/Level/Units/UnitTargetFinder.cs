namespace Game.Units
{
	using Game.Core;
	using Game.Field;
	using Game.Utilities;
	using System;
	using System.Linq;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IUnitTargetFinder
	{
		ReactiveCommand<IUnitFacade> TargetFound { get; }
		ReactiveCommand TargetLost { get; }
		void Reset();
	}

	public class UnitTargetFinder : ControllerBase, IUnitTargetFinder, IInitializable, ITickable
	{
		[Inject] private IGameCycle _levelCycle;
		[Inject] private IFieldFacade[] _fields;
		[Inject] private UnitFacade _unitFacade;

		private IUnitFacade _target;
		private IFieldFacade _alliedField;
		private IFieldFacade _enemyField;
		private IDisposable _targetDisposable;

		public void Initialize()
		{
			_levelCycle.State
				.Where(state => state == GameState.BattleStage)
				.Subscribe(_ => OnBattleStageHandler())
				.AddTo(this);
		}

		public void Tick()
		{
			if (_target == null && _enemyField != null && _enemyField.Units.Count != 0 && _levelCycle.State.Value == GameState.BattleStage)
			{
				SelectTarget();
			}
		}

		#region IUnitTargetFinder

		public ReactiveCommand<IUnitFacade> TargetFound { get; } = new ReactiveCommand<IUnitFacade>();

		public ReactiveCommand TargetLost { get; } = new ReactiveCommand();

		public void Reset()
		{
			TargetLost.Execute();
			_target = null;
			_targetDisposable.Dispose();
		}

		#endregion

		private void OnBattleStageHandler()
		{
			InitFields();
			SelectTarget();
        }

		private void SelectTarget()
		{
			_target = _enemyField.Units
				.OrderBy(u => (_unitFacade.Transform.position - u.Transform.position).sqrMagnitude)
				.FirstOrDefault();

			if (_target != null)
			{
				TargetFound.Execute(_target);

				_targetDisposable = _target.Died
					.Subscribe(_ => OnTargetDiedHandler());
			}
		}

		private void InitFields()
		{
			for (int i = 0; i < _fields.Length; i++)
			{
				if (_fields[i].HasUnit(_unitFacade))
					_alliedField = _fields[i];

				if (_fields[i].HasUnit(_unitFacade) == false)
					_enemyField = _fields[i];
			}
		}

		private void OnTargetDiedHandler()
		{
			Reset();
		}
	}
}
