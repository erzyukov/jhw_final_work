namespace Game.Dev
{
	using System.Collections.Generic;
	using System.Linq;
	using TMPro;
	using UnityEngine;

	public class FpsCounter : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;

		private const int SamplesCount = 30;

		private readonly List<float> _samples = new List<float>(SamplesCount);
		private int _index;

		private void Update()
		{
			GetSample();

			float deltaTime = _samples.Average();
			float fps = 1 / deltaTime;

			_text.text = Mathf.RoundToInt(fps).ToString();
		}

		private void GetSample()
		{
			float deltaTime = Time.unscaledDeltaTime;

			if (_samples.Count < _samples.Capacity)
			{
				_samples.Add(deltaTime);
			}
			else
			{
				_samples[_index++] = deltaTime;
				_index %= _samples.Count;
			}
		}
	}
}