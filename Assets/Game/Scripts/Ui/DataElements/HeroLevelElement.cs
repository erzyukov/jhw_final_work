namespace Game.Ui
{
	using Game.Core;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;

	public class HeroLevelElement : ProfileValueElement
	{
		[SerializeField] private Slider _expirience;

		[Inject] private IGameHero _gameHero;

		protected override void Subscribes()
		{
			Profile.HeroLevel
				.Subscribe(SetValue)
				.AddTo(this);

			Profile.HeroLevelExperience
				.Subscribe(_ => _expirience.value = _gameHero.GetExperienceRatio())
				.AddTo(this);
		}
	}
}