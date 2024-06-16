namespace Game.Utilities
{
	using UnityEngine;


	public class SafeBottomPanel : MonoBehaviour
	{
		void Start()
		{
			float screenHeight		= Screen.height;
			Rect safeArea			= Screen.safeArea;
			RectTransform canvas	= GetComponentInParent<Canvas>().GetComponent<RectTransform>();
			RectTransform panel		= GetComponent<RectTransform>();

			panel.sizeDelta			+= canvas.sizeDelta.y * safeArea.yMin / screenHeight * Vector2.up;
		}
	}
}

