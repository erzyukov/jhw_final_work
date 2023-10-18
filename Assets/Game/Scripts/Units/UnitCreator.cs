namespace Game.Units
{
	using Ui;
	using Utilities;
	using VContainer;
	using VContainer.Unity;
	using UnityEngine;
	using UniRx;

	public class UnitCreator : ControllerBase, IStartable
	{
		[Inject] IHudUnitPanel _hudUnitPanel;

		public void Start()
		{
			_hudUnitPanel.UnitSelectButtonPressed
				.Subscribe(CreateUnit)
				.AddTo(this);
		}

		private void CreateUnit(Unit.Type type)
		{
			Debug.LogWarning($"type: {type}");
		}
	}
}