namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;
	using Game.Configs;
	using DG.Tweening;

	public class UnitFx : ControllerBase, IInitializable
	{
		[Inject] private IUnitEvents _events;
		[Inject] private IUnitData _unitData;
		[Inject] private IUnitView _unitView;
		[Inject] private IUnitBuilder _unitBuilder;
		[Inject] private DamageNumberFx.Factory _damageFxFactory;
		[Inject] private UnitsConfig _unitsConfig;

		private Tween _tween;

		public void Initialize()
		{
			ShootFx shootFx = _unitView.Transform.GetComponentInChildren<ShootFx>();
			if (shootFx != null)
			{
				_events.Attacking
					.Subscribe(_ => shootFx.Play())
					.AddTo(this);
			}

			_events.DamageReceived
				.Subscribe(OnDamageReceived)
				.AddTo(this);
		}

		private void OnDamageReceived(float value)
		{
			if (_tween != null)
			{
				_tween.Rewind();
				_tween.Kill();
			}

			_tween = _unitBuilder.UnitRenderer.Renderer.material.DOColor(_unitsConfig.DamageFxMaterialColor, 0.05f)
				.SetEase(Ease.InOutSine)
				.SetLoops(2, LoopType.Yoyo)
				.OnComplete(() => _tween = null);

			Vector3 position = _unitView.Transform.position.WithY(_unitData.RendererHeight);
			Color color = _unitData.IsHero ? _unitsConfig.DamageFxHeroUnitColor : _unitsConfig.DamageFxEnemyUnitColor;
			_damageFxFactory.Create(position, (int)value, color);
		}
	}
}
