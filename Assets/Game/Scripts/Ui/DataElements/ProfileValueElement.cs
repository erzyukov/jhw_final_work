namespace Game.Ui
{
	using Game.Profiles;
	using UnityEngine;
	using TMPro;
	using Zenject;
	using DG.Tweening;

	public abstract class ProfileValueElement : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _value;
		
		[Header("Low Recource Alert")]
		[SerializeField] private ImageAlertInput _alert;

		[Inject] protected GameProfile Profile;

		private Tween _alertTween;

		private void Awake() => Subscribes();

		protected abstract void Subscribes();

		protected void SetValue(int value)
		{
			_value.text = value.ToString();
		}
    
        protected void SetValue(string value)
        {
            _value.text = value;
        }

		protected void PlayLowResourceAlert()
		{
			if (_alert.Image == null)
				return;

			_alertTween?.Rewind();

			_alertTween = _alert.Image.DOColor( _alert.Color, _alert.Duration )
				.SetLoops( _alert.Repeats, LoopType.Yoyo )
				.SetEase( Ease.InOutSine );
		}
    }
}