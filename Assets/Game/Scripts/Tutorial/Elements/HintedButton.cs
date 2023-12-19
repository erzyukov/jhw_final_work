namespace Game.Tutorial
{
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(Button))]
    public class HintedButton : MonoBehaviour
    {
        // TODO: Transform -> RectTransform
        [SerializeField] private Transform _hintPoint;
		[SerializeField] private bool _isLeft;

		public Parameters HintParameters => new Parameters() { Point = _hintPoint, IsLeft = _isLeft };

		public struct Parameters
		{
			public Transform Point;
			public bool IsLeft;
		}
    }
}
