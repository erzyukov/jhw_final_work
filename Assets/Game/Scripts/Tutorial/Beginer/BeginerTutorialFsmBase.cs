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
			AddOnEnterAction(BeginnerStep.FirstBattle, OnEnterFirstBattle);
			AddOnExitAction(BeginnerStep.FirstBattle, OnExitFirstBattle);
		}

		protected override BeginnerStep DefaultState => BeginnerStep.None;

		protected abstract void OnEnterFirstSummon();
		protected abstract void OnExitFirstSummon();
		protected abstract void OnEnterSecondSummon();
		protected abstract void OnExitSecondSummon();
		protected abstract void OnEnterFirstBattle();
		protected abstract void OnExitFirstBattle();
	}
}