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
	using Game.Field;
	using Game.Profiles;
	using System;

	public interface IGameAudio
	{
		void PlayUnitShoot(Species species);
		void PlayUiClick();
	}

	public class GameAudio : ControllerBase, IGameAudio, IInitializable
	{
		[Inject] private IAudioSources _audioSources;
		[Inject] private AudioConfig _audioConfig;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private List<Button> _uiButtons;
		[Inject] private List<Toggle> _uiToggles;
		[Inject] private IGameplayEvents _gameplayEvents;
		[Inject] private GameProfile _gameProfile;

		public void Initialize()
		{
			_gameProfile.IsMusicEnabled
				.Subscribe(OnMusicEnabledChanged)
				.AddTo(this);

			_gameProfile.IsSoundEnabled
				.Subscribe(OnSoundEnabledChanged)
				.AddTo(this);

			_gameCycle.State
				.Where(_ => _gameProfile.IsMusicEnabled.Value)
				.Subscribe(PlayGameStateTheme)
				.AddTo(this);

			_gameplayEvents.UnitsMerged
				.Where(_ => _gameProfile.IsSoundEnabled.Value)
				.Subscribe(_ => PlayUnitMerge())
				.AddTo(this);

			Observable.Timer(TimeSpan.FromSeconds(1))
				.Subscribe(_ => RegisterUiElements())
				.AddTo(this);
		}

		private void OnMusicEnabledChanged(bool value)
		{
			_audioSources.Music.volume = (value) ? _audioConfig.DefaultMusicVolume : 0;

			if (value)
				PlayGameStateTheme(_gameCycle.State.Value);
		}

		private void OnSoundEnabledChanged(bool value)
		{
			_audioSources.Sound.volume = (value) ? _audioConfig.DefaultSoundVolume : 0;
			_audioSources.Ui.volume = (value) ? _audioConfig.DefaultUiVolume : 0;
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
			if (_gameProfile.IsMusicEnabled.Value == false)
				return;

			_audioSources.Music.clip = clip;
			_audioSources.Music.Play();
			_audioSources.Music.DOFade(volume, _audioConfig.FadeDuration);
		}

		private void PlayUnitMerge()
		{
			if (_gameProfile.IsSoundEnabled.Value)
				_audioSources.Ui.PlayOneShot(_audioConfig.UnitMerge, 1);
		}

		private void RegisterUiElements()
		{
			_uiButtons.ForEach(button => button.OnClickAsObservable().Subscribe(_ => PlayUiClick()));
			_uiToggles.ForEach(toggle => toggle.OnValueChangedAsObservable().Skip(1).Subscribe(_ => PlayUiClick()));
		}

		#region IGameAudio

		public void PlayUnitShoot(Species species)
		{
			if (_gameProfile.IsSoundEnabled.Value == false)
				return;

			AudioClip clip = _audioConfig.GetShootClip(species);

			if (clip != null)
				_audioSources.Sound.PlayOneShot(clip);
		}

		public void PlayUiClick()
		{
			//if (_gameProfile.IsSoundEnabled.Value)
				//_audioSources.Ui.PlayOneShot(_audioConfig.UiButtonClick);
		}

		#endregion
	}
}
