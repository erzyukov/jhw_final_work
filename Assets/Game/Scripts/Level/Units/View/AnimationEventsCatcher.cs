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
        private int AttackHash = Animator.StringToHash(AttackClipName);

        public readonly ReactiveCommand Hited = new ReactiveCommand();
        public readonly ReactiveCommand AttackAnimationCompleted = new ReactiveCommand();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            AnimationClip clip = _animator.runtimeAnimatorController.animationClips.Where(clip => clip.name == AttackClipName).FirstOrDefault();

            if (clip != null)
            {
                AnimationEvent animationEndEvent = new AnimationEvent();
                animationEndEvent.time = clip.length;
                animationEndEvent.functionName = "OnAttackAnimationCompleted";
                animationEndEvent.stringParameter = clip.name;
                clip.AddEvent(animationEndEvent);
            }
        }

        public void Hit() => Hited.Execute();

        public void OnAttackAnimationCompleted() => AttackAnimationCompleted.Execute();

        public bool IsPlayingAttack => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == AttackHash;
    }
}