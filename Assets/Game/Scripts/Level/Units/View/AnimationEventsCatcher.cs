namespace Game.Units
{
    using System.Linq;
    using UniRx;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class AnimationEventsCatcher : MonoBehaviour
    {
        private Animator _animator;

        private const string AttackClipName = "Attack";
        private const string DeathClipName = "Death";

        public readonly ReactiveCommand Hited = new ReactiveCommand();
        public readonly ReactiveCommand AttackAnimationCompleted = new ReactiveCommand();
        public readonly ReactiveCommand DeathAnimationCompleted = new ReactiveCommand();

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            AddCompleteAnimation(AttackClipName, "OnAttackAnimationCompleted");
            AddCompleteAnimation(DeathClipName, "OnDeathAnimationCompleted");
        }

        public void Hit() => Hited.Execute();

        public void OnAttackAnimationCompleted() => AttackAnimationCompleted.Execute();
        
        public void OnDeathAnimationCompleted() => DeathAnimationCompleted.Execute();

        private void AddCompleteAnimation(string clipName, string handlerName)
        {
            AnimationClip clip = _animator.runtimeAnimatorController.animationClips.Where(clip => clip.name == clipName).FirstOrDefault();

            if (clip != null)
            {
                AnimationEvent animationEndEvent = new AnimationEvent();
                animationEndEvent.time = clip.length;
                animationEndEvent.functionName = handlerName;
                animationEndEvent.stringParameter = clip.name;
                clip.AddEvent(animationEndEvent);
            }
        }
    }
}