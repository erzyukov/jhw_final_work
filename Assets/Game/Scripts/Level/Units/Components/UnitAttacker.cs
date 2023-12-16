namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;
	using UnityEngine;
	using Game.Core;

	public interface IUnitAttacker
	{
		ReactiveCommand AttackRangeBroken { get; }
		void TryAttack(IUnitFacade target);
	}

	public class UnitAttacker : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] IUnitView _view;
		[Inject] private Species _species;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] UnitGrade _grade;
		[Inject] UnitConfig _config;

		private ITimer _atackTimer;
		private float _currentDamage;

		public void Initialize()
		{
			_atackTimer = new Timer();
			_currentDamage = _gameUpgrades.GetUnitDamage(_species) * _grade.DamageMultiplier;
		}

		#region IUnitAttacker

		public ReactiveCommand AttackRangeBroken { get; } = new ReactiveCommand();

		public void TryAttack(IUnitFacade target)
		{
			if (IsTargetClose(target) == false)
			{
				AttackRangeBroken.Execute();

				return;
			}

			if (_atackTimer.IsReady == false)
				return;

			target.TakeDamage(_currentDamage);
			_atackTimer.Set(_grade.AttackDelay);
		}

		#endregion

		private bool IsTargetClose(IUnitFacade target) =>
			(_view.Transform.position - target.Transform.position).sqrMagnitude 
			< _config.AttackRange * _config.AttackRange + Mathf.Epsilon;
	}
}