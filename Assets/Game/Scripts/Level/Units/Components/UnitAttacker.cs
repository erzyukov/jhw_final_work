namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;
	using UnityEngine;

	public interface IUnitAttacker
	{
		ReactiveCommand AttackRangeBroken { get; }
		void TryAttack(IUnitFacade target);
	}

	public class UnitAttacker : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] IUnitView _view;
		[Inject] UnitGrade _grade;
		[Inject] UnitConfig _config;

		private ITimer _atackTimer;

		public void Initialize()
		{
			_atackTimer = new Timer();
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

			target.TakeDamage(_grade.Damage);
			_atackTimer.Set(_grade.AttackDelay);
		}

		#endregion

		private bool IsTargetClose(IUnitFacade target) =>
			(_view.Transform.position - target.Transform.position).sqrMagnitude 
			< _config.AttackRange * _config.AttackRange + Mathf.Epsilon;
	}
}