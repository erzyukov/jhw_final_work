namespace Game.Ui
{
	using Game.Utilities;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiWavesHud
	{
		UiWaveView WavePrefab { get; }
		UiWaveDelimiterView DelimiterPrefab { get; }
		void AddWave(Transform wave);
		void AddDelimiter(Transform delimiter);
		void AddOffsetBlock();
		void ScrollToWave(RectTransform ancor);
		void Clear();
	}

    public class UiWavesHud : MonoBehaviour, IUiWavesHud
	{
		[SerializeField] private Transform _waveContainer;
		[SerializeField] private UiWaveView _wavePrefab;
		[SerializeField] private UiWaveDelimiterView _delimiterPrefab;
		[SerializeField] private Transform _wavesOffsetPrefab;
		[SerializeField] private ScrollRect _scrollrect;
		[SerializeField] private RectTransform _viewportPanel;
		[SerializeField] private RectTransform _contentPanel;

		private float offsetBlockWidth;

		private void Start()
		{
			RectTransform offsetBlockRectTransform = _wavesOffsetPrefab.GetComponent<RectTransform>();
			offsetBlockWidth = offsetBlockRectTransform.rect.width;
		}

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

		public void ScrollToWave(RectTransform ancor)
		{
			Canvas.ForceUpdateCanvases();

			Vector2 ancorContentPosition = (Vector2)_contentPanel.transform.InverseTransformPoint(ancor.position);
			_scrollrect.horizontalNormalizedPosition = 
				(ancorContentPosition.x - offsetBlockWidth) / 
				(_contentPanel.rect.width - offsetBlockWidth * 2 - ancor.rect.width);
		}

		public void Clear() =>
			_waveContainer.DestroyChildren();

		#endregion
	}
}
