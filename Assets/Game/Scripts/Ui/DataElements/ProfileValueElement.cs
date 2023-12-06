namespace Game.Ui
{
	using Game.Profiles;
	using UnityEngine;
	using TMPro;
	using Zenject;

	public abstract class ProfileValueElement : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _value;

		[Inject] protected GameProfile Profile;

		private void Awake() => Subscribes();

		protected abstract void Subscribes();

		protected void SetValue(int value)
		{
			_value.text = value.ToString();
		}
	}
}