namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Profiles;
	using Game.Utilities;
	using System.Collections.Generic;
	using System.Linq;
	using UniRx;
	using Zenject;

	public class UiLevelWaves : ControllerBase, IInitializable
	{
		[Inject] private IUiWavesBuilder _builder;
		[Inject] private IUiWavesHud _wavesHud;
		[Inject] private LevelConfig _levelConfig;
		[Inject] private GameProfile _gameProfile;
		[Inject] private ILocalizator _localizator;
		[Inject] private IGameLevel _gameLevel;

		private const string WaveTitlePrefixKey = "wave";

		private string _waveTitlePrefix;
		private List<IUiWaveView> _levelWaves;
		private int WavesCount => _levelConfig.Waves.Length;

		public void Initialize()
		{
			_waveTitlePrefix = _localizator.GetString(WaveTitlePrefixKey);

			InitWaves(WavesCount);
			InitDelimiters(WavesCount - 1);

			Observable.Merge(
				_gameLevel.LevelLoading
					.DelayFrame(1)
					.Select(_ => _gameProfile.WaveNumber.Value - 1),
				_gameProfile.WaveNumber
					.Where(number => number > 0)
					.Select(number => number - 1)
			)
				.Subscribe(OnWaveNumberChangedHandler)
				.AddTo(this);
		}

		private void OnWaveNumberChangedHandler(int index)
		{
			for (int i = 0; i < WavesCount; i++)
				ResetWave(i);
			for (int i = 0; i < WavesCount - 1; i++)
				ResetDelimiter(i);

			_levelWaves[index].SetSelectionIconActive(true);
			_levelWaves[index].SetCurrentIconActive(true);

			for (int i = 0; i < index; i++)
				_levelWaves[i].SetCompleteIconActive(true);

			for (int i = index + 1; i < WavesCount; i++)
				_levelWaves[i].SetNotAchievedIconActive(true);

			if (index > 0)
			{
				_builder.Delimiters[index - 1].SetDefaultIconActive(false);
				_builder.Delimiters[index - 1].SetCurrentIconActive(true);
			}

			_wavesHud.ScrollToWave(_levelWaves[index].Ancor);
		}

		private void InitWaves(int activeCount)
		{
			_levelWaves = new List<IUiWaveView>();

			for (int i = 0; i < _builder.Waves.Length; i++)
				_builder.Waves[i].SetActive(false);

			for (int i = 0; i < activeCount - 1; i++)
				_levelWaves.Add(_builder.Waves[i]);

			_levelWaves.Add(_builder.Waves[_builder.Waves.Length - 1]);

			for (int i = 0; i < _levelWaves.Count; i++)
			{
				_levelWaves[i].SetTitle($"{_waveTitlePrefix} {i + 1}");
				_levelWaves[i].SetActive(true);
			}
		}

		private void InitDelimiters(int activeCount)
		{
			for (int i = 0; i < activeCount; i++)
				_builder.Delimiters[i].SetActive(true);

			for (int i = activeCount; i < _builder.Waves.Length - 1; i++)
				_builder.Delimiters[i].SetActive(false);
		}

		private void ResetWave(int index)
		{
			_levelWaves[index].SetSelectionIconActive(false);
			_levelWaves[index].SetCurrentIconActive(false);
			_levelWaves[index].SetCompleteIconActive(false);
			_levelWaves[index].SetNotAchievedIconActive(false);
		}

		private void ResetDelimiter(int index)
		{
			_builder.Delimiters[index].SetDefaultIconActive(true);
			_builder.Delimiters[index].SetCurrentIconActive(false);
		}
	}
}
