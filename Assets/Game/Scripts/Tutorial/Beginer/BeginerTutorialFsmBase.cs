namespace Game.Tutorial
{
	using Game.Utilities;
	using Zenject;

	public abstract class BeginerTutorialFsmBase : SimpleFsmBase<BeginnerStep>, ITickable
	{
		protected BeginerTutorialFsmBase()
		{
			AddOnEnterAction(BeginnerStep.FirstSummon, OnEnterFirstSummon);
			AddOnExitAction(BeginnerStep.FirstSummon, OnExitFirstSummon);
			AddOnEnterAction(BeginnerStep.SecondSummon, OnEnterSecondSummon);
			AddOnExitAction(BeginnerStep.SecondSummon, OnExitSecondSummon);
		}

		protected override BeginnerStep DefaultState => BeginnerStep.None;

		protected abstract void OnEnterFirstSummon();
		protected abstract void OnExitFirstSummon();
		protected abstract void OnEnterSecondSummon();
		protected abstract void OnExitSecondSummon();
	}
}