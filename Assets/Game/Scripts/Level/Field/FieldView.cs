namespace Game.Field
{
	using UnityEngine;

	public interface IFieldView
	{
		Transform Transform { get; }
		void SetPosition(Vector3 position);
	}

	public class FieldView : MonoBehaviour, IFieldView
	{
		public Transform Transform => transform;
		public void SetPosition(Vector3 position) => transform.position = position;
	}
}