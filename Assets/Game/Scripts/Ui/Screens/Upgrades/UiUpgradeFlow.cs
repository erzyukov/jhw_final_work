namespace Game.Ui
{
	using Game.Units;
	using UniRx;

	public interface IUiUpgradeFlow
	{
		ReactiveProperty<Species> SelectedUnit { get; }
		ReactiveCommand<Species> UpgradeClicked { get; }
		ReactiveCommand<Species> AdClicked { get; }
	}

	public class UiUpgradeFlow : IUiUpgradeFlow
	{
		public ReactiveProperty<Species> SelectedUnit			{ get; } = new();
		public ReactiveCommand<Species> UpgradeClicked			{ get; } = new();
		public ReactiveCommand<Species> AdClicked				{ get; } = new();

	}
}
