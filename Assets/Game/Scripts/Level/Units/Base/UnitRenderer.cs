namespace Game.Units
{
	using UnityEngine;
	using Zenject;

	public interface IUnitRenderer
	{
		Transform Transform { get; }
		Renderer Renderer { get; }
		Transform RendererTransform { get; }
        Animator Animator { get; }
        AnimationEventsCatcher AnimationEventsCatcher { get; }
    }

	public class UnitRenderer : MonoBehaviour, IUnitRenderer
	{
		[SerializeField] Renderer _renderer;
		[SerializeField] Animator _animator;
		[SerializeField] AnimationEventsCatcher _animationEventsCatcher;

		private void Awake()
		{
			RendererTransform = _renderer.transform;
		}

		#region IUnitModel

		public Transform Transform => transform;

		public Renderer Renderer => _renderer;

		public Transform RendererTransform { get; private set; }

        public Animator Animator => _animator;

        public AnimationEventsCatcher AnimationEventsCatcher => _animationEventsCatcher;

        #endregion

        public class Factory : PlaceholderFactory<Object, IUnitRenderer>
		{
		}
	}
}