namespace Game.Field
{
	using Game.Dev;
	using UnityEngine;

	public interface IFieldView
	{
		Transform Transform { get; }
		void SetPosition(Vector3 position);
	}

	public class FieldView : MonoBehaviour, IFieldView
	{
		public Transform Transform => transform;

		private void Start()
		{
			WebGLDebug.Log($"[Project] FieldView: Start: monobehaviour");
		}

		public void SetPosition(Vector3 position) => transform.position = position;
	}
}