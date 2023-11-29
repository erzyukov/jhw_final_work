namespace Game.Ui
{
	using UnityEngine;
	using Zenject;

	public interface IUiWavesBuilder
	{
		IUiWaveView[] Waves { get; }
		IUiWaveDelimiterView[] Delimiters { get; }
	}

	public class UiWavesBuilder : IInitializable, IUiWavesBuilder
	{
		[Inject] private IUiWavesHud _wavesHud;

		private const int MaxWawes = 30;
		private const string WaveTitlePrefix = "Волна";

		private IUiWaveView[] _waves;
		private IUiWaveDelimiterView[] _delimiters;

		public void Initialize()
		{
			_wavesHud.Clear();
			_waves = new UiWaveView[MaxWawes];
			_delimiters = new UiWaveDelimiterView[MaxWawes - 1];

			CreateWavesPool();
		}

		#region IUiWavesBuilder

		public IUiWaveView[] Waves => _waves;

		public IUiWaveDelimiterView[] Delimiters => _delimiters;

		#endregion

		private void CreateWavesPool()
		{
			_wavesHud.AddOffsetBlock();

			for (int i = 0; i < MaxWawes; i++)
			{
				_waves[i] = GameObject.Instantiate(_wavesHud.WavePrefab);
				_waves[i].SetTitle($"{WaveTitlePrefix} {i + 1}");
				_waves[i].SetActive(false);
				_wavesHud.AddWave(_waves[i].Transform);

				if (i != MaxWawes - 1)
				{
					_delimiters[i] = GameObject.Instantiate(_wavesHud.DelimiterPrefab);
					_delimiters[i].SetActive(false);
					_wavesHud.AddDelimiter(_delimiters[i].Transform);
				}
			}

			_wavesHud.AddOffsetBlock();
		}
	}
}