namespace Game.Ui
{
	using Game.Utilities;
	using UnityEngine;

	public interface IUiWavesHud
	{
		UiWaveView WavePrefab { get; }
		UiWaveDelimiterView DelimiterPrefab { get; }
		void AddWave(Transform wave);
		void AddDelimiter(Transform delimiter);
		void AddOffsetBlock();
		void Clear();
	}

    public class UiWavesHud : MonoBehaviour, IUiWavesHud
	{
		[SerializeField] private Transform _waveContainer;
		[SerializeField] private UiWaveView _wavePrefab;
		[SerializeField] private UiWaveDelimiterView _delimiterPrefab;
		[SerializeField] private Transform _wavesOffsetPrefab;

		#region IUiWavesHud

		public UiWaveView WavePrefab => _wavePrefab;

		public UiWaveDelimiterView DelimiterPrefab => _delimiterPrefab;

		public void AddWave(Transform wave)
		{
			wave.SetParent(_waveContainer, false);
		}

		public void AddDelimiter(Transform delimiter)
		{
			delimiter.SetParent(_waveContainer, false);
		}

		public void AddOffsetBlock()
		{
			Transform block = Instantiate(_wavesOffsetPrefab);
			block.SetParent(_waveContainer, false);
		}

		public void Clear() =>
			_waveContainer.DestroyChildren();

		#endregion
	}
}
