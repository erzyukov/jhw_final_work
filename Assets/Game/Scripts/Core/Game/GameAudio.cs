namespace Game.Core
{
	using Game.Configs;
	using Game.Units;
	using Game.Utilities;
	using UnityEngine;
	using Zenject;
	using UniRx;
	using DG.Tweening;
	using UnityEngine.UI;
	using System.Collections.Generic;

	public interface IGameAudio
	{
		void PlayUnitShoot(Species species);
		void PlayUiClick();
		void PlayUnitMerge();
	}

	public class GameAudio : ControllerBase, IGameAudio, IInitializable
	{
		[Inject] private IAudioSources _audioSources;
		[Inject] private AudioConfig _audioConfig;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private List<Button> _uiButtons;

		public void Initialize()
		{
			_audioSources.Sound.volume = _audioConfig.DefaultSoundVolume;
			_audioSources.Music.volume = _audioConfig.DefaultMusicVolume;
			_audioSources.Ui.volume = _audioConfig.DefaultUiVolume;

			_gameCycle.State
				.Subscribe(PlayGameStateTheme)
				.AddTo(this);

			_uiButtons.ForEach(button => button.OnClickAsObservable().Subscribe(_ => PlayUiClick()));
		}

		private void PlayGameStateTheme(GameState state)
		{
			AudioClip clip = _audioConfig.GetGameStateTheme(state, out float volume);

			if (clip == null || _audioSources.Music.clip == clip)
			{
				if (_audioSources.Music.volume != volume && volume != 0)
					_audioSources.Music.DOFade(volume, _audioConfig.FadeDuration);

				return;
			}

			if (_audioSources.Music.isPlaying)
			{
				_audioSources.Music.DOFade(0, _audioConfig.FadeDuration)
					.OnComplete(() =>
					{
						_audioSources.Music.Stop();
						PlaySmoothly(clip, volume);
					});
			}
            else
            {
				_audioSources.Music.volume = 0;
				PlaySmoothly(clip, volume);
			}
		}

		private void PlaySmoothly(AudioClip clip, float volume)
		{
			_audioSources.Music.clip = clip;
			_audioSources.Music.Play();
			_audioSources.Music.DOFade(volume, _audioConfig.FadeDuration);
		}

		#region IGameAudio

		public void PlayUnitShoot(Species species)
		{
			AudioClip clip = _audioConfig.GetShootClip(species);

			if (clip != null)
				_audioSources.Sound.PlayOneShot(clip);
		}

		public void PlayUiClick() =>
			_audioSources.Ui.PlayOneShot(_audioConfig.UiButtonClick);

		public void PlayUnitMerge() =>
			_audioSources.Ui.PlayOneShot(_audioConfig.UnitMerge, 1);

		#endregion
	}
}
