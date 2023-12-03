namespace Game.Tutorial
{
	using UnityEngine;
	using TMPro;

	public interface IDialogHint
	{
		void SetMessage(string value);
		void SetActive(bool value);
	}

    public class DialogHint : MonoBehaviour, IDialogHint
	{
		[SerializeField] private TextMeshProUGUI _message;

		#region IDialogHint

		public void SetActive(bool value) => gameObject.SetActive(value);

		public void SetMessage(string value) => _message.text = value;
		
		#endregion
	}
}
