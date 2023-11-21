namespace Game.Units
{
	using Game.Configs;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IUnitFacade
	{
		ReactiveCommand Died { get; }
		string Name { get; }
		Species Species { get; }
		Transform Transform { get; }
		void SetViewParent(Transform parent);
		void TakeDamage(float damage);
		void Reset();
		void Destroy();
	}

	public class UnitFacade : IUnitFacade
	{
		[Inject] private Species _species;
		[Inject] private IUnitView _view;
		[Inject] private UnitConfig _config;
		[Inject] private IUnitHealth _health;
		[Inject] private IUnitTargetFinder _targetFinder;

		#region IUnitFacade

		public ReactiveCommand Died => _health.Died;

		public string Name => _config.Title;

		public Species Species => _species;

		public Transform Transform => _view.Transform;

		public void SetViewParent(Transform parent) => _view.SetParent(parent);

		public void TakeDamage(float damage) => _health.TakeDamage(damage);

		public void Reset()
		{
			// TODO: Reset All
			_health.Reset();
			_targetFinder.Reset();
			_view.ResetPosition();
			_view.SetActive(true);
		}

		public void Destroy()
		{
			_view.Destroy();
		}

		#endregion

		public class Factory : PlaceholderFactory<Species, UnitFacade> {}
	}
}