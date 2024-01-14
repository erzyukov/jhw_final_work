namespace Game.Units
{
	using System;
	using UnityEngine;
	using Zenject;
	using TMPro;
	using DG.Tweening;
	using Game.Utilities;

	public class DamageFx : MonoBehaviour, IPoolable<Vector3, int, Color, IMemoryPool>, IDisposable
	{
		[SerializeField] private TextMeshPro _textMeshPro;
		private IMemoryPool _pool;

		public void Dispose()
		{
			_pool.Despawn(this);
		}

		public void OnDespawned()
		{
			_pool = null;
			_textMeshPro.text = "";
		}

		public void OnSpawned(Vector3 position, int damage, Color color, IMemoryPool pool)
		{
			Vector3 offset = UnityEngine.Random.insideUnitCircle.x0y() * 0.1f;

			transform.position = position + offset;
			_textMeshPro.text = damage.ToString();
			_textMeshPro.color = color;
			_pool = pool;

			float target = transform.position.y + 0.8f;
			transform.DOMoveY(target, 1)
				.SetEase(Ease.OutCubic)
				.OnComplete(() => Dispose());
		}

		public class Factory : PlaceholderFactory<Vector3, int, Color, DamageFx> { }
	}
}