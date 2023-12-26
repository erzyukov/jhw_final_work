namespace Game.Profiles
{
	using System;
	using Game.Tutorial;
	using UniRx;

	[Serializable]
	public class TutorialProfile
	{
		public ReactiveProperty<BeginnerStep> BeginnerStep = new ReactiveProperty<BeginnerStep>();// Game.Tutorial.BeginnerStep.Complete
		public ReactiveProperty<UpgradesStep> UpgradesStep = new ReactiveProperty<UpgradesStep>();// Game.Tutorial.UpgradesStep.Complete
    }
}

