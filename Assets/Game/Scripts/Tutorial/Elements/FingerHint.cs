namespace Game.Tutorial
{
	using Game.Utilities;
	using UnityEngine;

	public interface IFingerHint
	{
		void SetPosition(Vector3 value);
		void SetPosition(Transform value);
		void SetActive(bool value);
		void SetLeft(bool value);
	}

	public class FingerHint : MonoBehaviour, IFingerHint
	{
		#region IFingerHint

		public void SetActive(bool value) =>
			gameObject.SetActive(value);

		public void SetPosition(Vector3 value) =>
			transform.position = value;

        public void SetPosition(Transform value)
        {
            transform.SetParent(value, false);
        }

        public void SetLeft(bool value) =>
			transform.localScale = (value)? Vector3.one.WithX(-1): Vector3.one;

		#endregion
	}
}
