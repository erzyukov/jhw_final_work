namespace Game.Ui
{
	using Game.Units;
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;

	public interface IUiUpgradeFlow
	{
		ReactiveProperty<Species> SelectedUnit { get; }
		ReactiveCollection<Species> SelectDisabled { get; }
		ReactiveCollection<Species> UpgradeDisabled { get; }
		ReactiveCommand<Species> UpgradeClicked { get; }
		ReactiveCommand<Species> AdClicked { get; }

		Dictionary<Species, GameObject> SelectButtons { get; }
		Dictionary<Species, GameObject> UpgradeButtons { get; }
	}

	public class UiUpgradeFlow : IUiUpgradeFlow
	{
		public ReactiveProperty<Species> SelectedUnit			{ get; } = new(Species.HeroInfantryman);
		public ReactiveCollection<Species> SelectDisabled		{ get; } = new();
		public ReactiveCollection<Species> UpgradeDisabled		{ get; } = new();
		public ReactiveCommand<Species> UpgradeClicked			{ get; } = new();
		public ReactiveCommand<Species> AdClicked				{ get; } = new();

		public Dictionary<Species, GameObject> SelectButtons	{ get; } = new();
		public Dictionary<Species, GameObject> UpgradeButtons	{ get; } = new();
	}
}
