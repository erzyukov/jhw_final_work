namespace Game.Units
{
	using Game.Core;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUnitView
	{
		GameObject GameObject { get; }
		void SetParent(Transform transform);
		void SetIgnoreRaycast(bool value);
		void SetHealthRatio(float value);
		void SetCooldownRatio(float value);
	}

	public class UnitView : MonoBehaviour, IUnitView
	{
		[SerializeField] private Slider _health;
		[SerializeField] private Slider _cooldown;

		private List<GameObject> _unitGameObjects;

		public GameObject GameObject => gameObject;

		private void Start()
		{
			_unitGameObjects = new List<GameObject>{ GameObject };

			foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
				_unitGameObjects.Add(child.gameObject);
		}

		public void SetParent(Transform transform) =>
			this.transform.SetParent(transform, false);

		public void SetIgnoreRaycast(bool value)
		{
			int layer = value ? Constans.IgnoreRaycastLayer : Constans.DefaultLayer;

			foreach (GameObject item in _unitGameObjects)
				item.gameObject.layer = layer;
		}

		public void SetHealthRatio(float value) => _health.value = value;
		public void SetCooldownRatio(float value) => _cooldown.value = value;
	}
}
