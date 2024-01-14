namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;
	using Game.Configs;

	public class UnitFx : ControllerBase, IInitializable
	{
		[Inject] private IUnitEvents _events;
		[Inject] private IUnitData _unitData;
		[Inject] private IUnitView _unitView;
		[Inject] private DamageFx.Factory _damageFxFactory;
		[Inject] private UnitsConfig _unitsConfig;

		public void Initialize()
		{
			_events.DamageReceived
				.Subscribe(OnDamageReceived)
				.AddTo(this);
		}

		private void OnDamageReceived(float value)
		{
			Vector3 position = _unitView.Transform.position.WithY(_unitData.RendererHeight);
			Color color = _unitData.IsHero ? _unitsConfig.DamageFxHeroUnitColor : _unitsConfig.DamageFxEnemyUnitColor;
			_damageFxFactory.Create(position, (int)value, color);
		}
	}
}
