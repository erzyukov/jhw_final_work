namespace Game.Ui
{
	using UniRx;

	public interface IUiLobbyFlow
	{
		ReactiveCommand Loaded { get; }
		ReactiveCommand PlayButtonClicked { get; }
		IntReactiveProperty SelectedLevelIndex { get; }
		BoolReactiveProperty IsStartAvailable { get; }
		BoolReactiveProperty IsSelectLevelAvailable { get; }
		BoolReactiveProperty IsNextStageAvailable { get; }
	}

	public class UiLobbyFlow : IUiLobbyFlow
	{
		public ReactiveCommand			Loaded					{ get; } = new();
		public ReactiveCommand			PlayButtonClicked		{ get; } = new();
		public IntReactiveProperty		SelectedLevelIndex		{ get; } = new();
		public BoolReactiveProperty		IsStartAvailable		{ get; } = new();
		public BoolReactiveProperty		IsSelectLevelAvailable	{ get; } = new();
		public BoolReactiveProperty		IsNextStageAvailable	{ get; } = new();
	}
}
