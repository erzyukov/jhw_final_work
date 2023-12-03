namespace Game.Profiles
{
	using System;
	using Game.Tutorial;
	using UniRx;

	[Serializable]
	public class TutorialProfile
	{
		public ReactiveProperty<BeginnerStep> BeginerStep = new ReactiveProperty<BeginnerStep>();// BeginerTutorial.Step.Complete
	}
}

