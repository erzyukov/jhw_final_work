namespace Game.Core
{
	using UniRx;

	internal interface IApplicationPaused
	{
		BoolReactiveProperty IsApplicationPaused { get; }
	}
}
