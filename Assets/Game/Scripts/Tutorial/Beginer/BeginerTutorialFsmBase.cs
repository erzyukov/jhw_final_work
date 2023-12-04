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
			AddOnEnterAction(BeginnerStep.ThirdSummon, OnEnterThirdSummon);
			AddOnExitAction(BeginnerStep.ThirdSummon, OnExitThirdSummon);
			AddOnEnterAction(BeginnerStep.FourthSummon, OnEnterFourthSummon);
			AddOnExitAction(BeginnerStep.FourthSummon, OnExitFourthSummon);
			AddOnEnterAction(BeginnerStep.FirstMerge, OnEnterFirstMerge);
			AddOnExitAction(BeginnerStep.FirstMerge, OnExitFirstMerge);
			AddOnEnterAction(BeginnerStep.SecondMerge, OnEnterSecondMerge);
			AddOnExitAction(BeginnerStep.SecondMerge, OnExitSecondMerge);
			AddOnEnterAction(BeginnerStep.SecondBattle, OnEnterSecondBattle);
			AddOnExitAction(BeginnerStep.SecondBattle, OnExitSecondBattle);
			AddOnEnterAction(BeginnerStep.LastSummon, OnEnterSixthSummon);
			AddOnExitAction(BeginnerStep.LastSummon, OnExitSixthSummon);
			AddOnEnterAction(BeginnerStep.ThirdBattle, OnEnterThirdBattle);
			AddOnExitAction(BeginnerStep.ThirdBattle, OnExitThirdBattle);
		}

		protected override BeginnerStep DefaultState => BeginnerStep.None;

		protected abstract void OnEnterFirstSummon();
		protected abstract void OnExitFirstSummon();
		protected abstract void OnEnterSecondSummon();
		protected abstract void OnExitSecondSummon();
		protected abstract void OnEnterFirstBattle();
		protected abstract void OnExitFirstBattle();
		protected abstract void OnEnterThirdSummon();
		protected abstract void OnExitThirdSummon();
		protected abstract void OnEnterFourthSummon();
		protected abstract void OnExitFourthSummon();
		protected abstract void OnEnterFirstMerge();
		protected abstract void OnExitFirstMerge();
		protected abstract void OnEnterSecondMerge();
		protected abstract void OnExitSecondMerge();
		protected abstract void OnEnterSecondBattle();
		protected abstract void OnExitSecondBattle();
		protected abstract void OnEnterSixthSummon();
		protected abstract void OnExitSixthSummon();
		protected abstract void OnEnterThirdBattle();
		protected abstract void OnExitThirdBattle();
	}
}