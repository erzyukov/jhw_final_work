namespace Game.Ui
{
	using Game.Configs;
	using Game.Profiles;
	using Game.Utilities;
	using System.Linq;
	using UniRx;
	using Zenject;

	public class UiLevelWaves : ControllerBase, IInitializable
	{
		[Inject] private IUiWavesBuilder _builder;
		[Inject] private IUiWavesHud _wavesHud;
		[Inject] private LevelConfig _levelConfig;
		[Inject] private GameProfile _gameProfile;

		private int WavesCount => _levelConfig.Waves.Length;

		public void Initialize()
		{
			InitWaves(WavesCount);
			InitDelimiters(WavesCount - 1);

			for (int i = 0; i < WavesCount; i++)
				_builder.Waves[i].SetActive(true);

			_gameProfile.WaveNumber
				.Where(number => number > 0)
				.Select(number => number - 1)
				.Subscribe(OnWaveNumberChangedHandler)
				.AddTo(this);
		}

		private void OnWaveNumberChangedHandler(int index)
		{
			for (int i = 0; i < WavesCount; i++)
				ResetWave(i);
			for (int i = 0; i < WavesCount - 1; i++)
				ResetDelimiter(i);

			_builder.Waves[index].SetSelectionIconActive(true);
			_builder.Waves[index].SetCurrentIconActive(true);

			for (int i = 0; i < index; i++)
				_builder.Waves[i].SetCompleteIconActive(true);

			for (int i = index + 1; i < WavesCount; i++)
				_builder.Waves[i].SetNotAchievedIconActive(true);

			if (index > 0)
			{
				_builder.Delimiters[index - 1].SetDefaultIconActive(false);
				_builder.Delimiters[index - 1].SetCurrentIconActive(true);
			}

			_wavesHud.ScrollToWave(_builder.Waves[index].Ancor);
		}

		private void InitWaves(int activeCount)
		{
			for (int i = 0; i < activeCount; i++)
				_builder.Waves[i].SetActive(true);

			for (int i = activeCount; i < _builder.Waves.Length; i++)
				_builder.Waves[i].SetActive(false);
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
			_builder.Waves[index].SetSelectionIconActive(false);
			_builder.Waves[index].SetCurrentIconActive(false);
			_builder.Waves[index].SetCompleteIconActive(false);
			_builder.Waves[index].SetNotAchievedIconActive(false);
		}

		private void ResetDelimiter(int index)
		{
			_builder.Delimiters[index].SetDefaultIconActive(true);
			_builder.Delimiters[index].SetCurrentIconActive(false);
		}
	}
}
