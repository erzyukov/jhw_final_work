namespace Game.Tutorial
{
	using Game.Utilities;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Zenject;

	public interface IFingerHint
	{
		void Show(FingerPlace place);
		void Hide();
		void SetPosition(Transform value);
		void SetLeft(bool value);
	}

	public class FingerHint : MonoBehaviour, IFingerHint
	{
		[Inject] private List<HintedButton> _hintedButtons;

		#region IFingerHint

		public void Show(FingerPlace place)
		{
			HintedButton hintedButton = _hintedButtons.Where(b => b.Place == place).FirstOrDefault();
			
			if (hintedButton != null)
			{
				SetPosition(hintedButton.HintParameters.Point);
				SetLeft(hintedButton.HintParameters.IsLeft);
			}

			SetActive(true);
		}

		public void Hide() =>
			SetActive(false);

		public void SetPosition(Transform value) =>
            transform.SetParent(value, false);

		public void SetLeft(bool value) =>
			transform.localScale = (value)? Vector3.one.WithX(-1): Vector3.one;

		#endregion

		private void SetActive(bool value) =>
			gameObject.SetActive(value);

		private void SetPosition(Vector3 value) =>
			transform.position = value;
	}
}
