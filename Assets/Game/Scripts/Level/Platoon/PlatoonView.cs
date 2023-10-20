namespace Game
{
	using UnityEngine;

	public interface IPlatoonView
	{
		Transform Transform { get; }
		void SetPosition(Vector3 position);
	}

	public class PlatoonView : MonoBehaviour, IPlatoonView
	{
		public Transform Transform => transform;
		public void SetPosition(Vector3 position) => transform.position = position;
	}
}