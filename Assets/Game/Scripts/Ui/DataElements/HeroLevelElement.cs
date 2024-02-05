namespace Game.Ui
{
	using Game.Core;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;

	public class HeroLevelElement : ProfileValueElement
	{
		[SerializeField] private Slider _expirience;
		[SerializeField] private TextMeshProUGUI _expiriencePercent;

		[Inject] private IGameHero _gameHero;

		protected override void Subscribes()
		{
			_gameHero.AnimatedLevelNumber
				.Subscribe(SetValue)
				.AddTo(this);

			if (_expirience != null )
				_gameHero.ExperienceRatio
					.Subscribe(value =>
					{
						_expirience.value = value;
						if (_expiriencePercent != null )
							_expiriencePercent.text = $"{Mathf.Round(value * 100)}%";
					})
					.AddTo(this);
		}
	}
}