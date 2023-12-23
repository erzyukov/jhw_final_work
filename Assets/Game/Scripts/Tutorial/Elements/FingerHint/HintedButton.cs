namespace Game.Tutorial
{
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(Button))]
    public class HintedButton : MonoBehaviour
    {
		[SerializeField] private FingerPlace _place;        
		[SerializeField] private Transform _hintPoint;
		[SerializeField] private bool _isLeft;

		public FingerPlace Place => _place;

		public Parameters HintParameters => new Parameters() { Point = _hintPoint, IsLeft = _isLeft };

		public struct Parameters
		{
			public Transform Point;
			public bool IsLeft;
		}
    }
}
