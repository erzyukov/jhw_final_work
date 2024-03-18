namespace Game.Ui
{
	using Game.Core;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using DG.Tweening;

	public class HeroLevelElement : ProfileValueElement
	{
		[SerializeField] private Slider _expirience;
		[SerializeField] private TextMeshProUGUI _expiriencePercent;

		[Header("Low Hero Level Alert")]
		[SerializeField] private Image _backgroun;
		[SerializeField] private Color _alertColor;
		[SerializeField] private float _alertDuration;
		[SerializeField] private int _alertRepeats;

		[Inject] private IGameHero _gameHero;
		[Inject] private IResourceEvents _resourceEvents;

		private Tween _alertTween;

		protected override void Subscribes()
		{
			_gameHero.AnimatedLevelNumber
				.Subscribe( SetValue )
				.AddTo( this );

			if (_expirience != null)
				_gameHero.ExperienceRatio
					.Subscribe( OnValueChanged )
					.AddTo( this );

			_resourceEvents.LowHeroLevelAlert
				.Subscribe( _ => OnLowHeroLevelAlert() )
				.AddTo( this );
		}

		private void OnValueChanged( float value )
		{
			_expirience.value = value;
			if (_expiriencePercent != null)
				_expiriencePercent.text = $"{Mathf.Round( value * 100 )}%";
		}

		private void OnLowHeroLevelAlert()
		{
			_alertTween?.Rewind();

			_alertTween = _backgroun.DOColor( _alertColor, _alertDuration )
				.SetLoops( _alertRepeats, LoopType.Yoyo )
				.SetEase( Ease.InOutSine );
		}
	}
}