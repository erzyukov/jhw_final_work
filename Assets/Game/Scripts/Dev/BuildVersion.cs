namespace Game.Dev
{
	using TMPro;
	using UnityEngine;

	public class BuildVersion : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;

		private void Start()
		{
			_text.text = "v" + Application.version;
		}
	}
}