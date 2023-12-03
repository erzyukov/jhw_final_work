namespace Game.Tutorial
{
	using UnityEngine;

	public interface IFingerHint
	{
		void SetPosition(Vector3 value);
		void SetActive(bool value);
	}

	public class FingerHint : MonoBehaviour, IFingerHint
	{
		#region IFingerHint

		public void SetActive(bool value) =>
			gameObject.SetActive(value);

		public void SetPosition(Vector3 value) =>
			transform.position = value;

		#endregion
	}
}
