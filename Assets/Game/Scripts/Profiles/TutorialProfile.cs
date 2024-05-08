namespace Game.Profiles
{
	using System;
	using Game.Tutorial;
	using UniRx;

	[Serializable]
	public class TutorialProfile
	{
		public ReactiveProperty<BeginnerStep>		BeginnerStep		= new();
		public ReactiveProperty<UpgradesStep>		UpgradesStep		= new();
		public ReactiveProperty<UnlockUnitStep>		UnlockUnitStep		= new();

		public BoolReactiveProperty					IsBattleHintComplete	= new();
    }
}

