namespace Game.Platoon
{
	using Utilities;
	using Configs;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

	public class PlatoonBuilder : ControllerBase, IStartable
	{
		[Inject] private BattleFieldConfig _config;
		[Inject] private IPlatoon _platoon;
		[Inject] private IPlatoonView _view;
		[Inject] private Camera _camera;

		private Vector3 _viewOffset;

		public void Start()
		{
			float zSign = Mathf.Sign(_view.Transform.position.z);
			float platoonXOffset = zSign * _config.DefaultPlatoonSize.x / 2f * _config.PlatoonCellWidth;
			_viewOffset = _view.Transform.position.WithX(platoonXOffset);
			_view.SetPosition(_viewOffset);

			RectInt rect = new RectInt(Vector2Int.zero, _config.DefaultPlatoonSize);
			Map<PlatoonCell> map = new Map<PlatoonCell>(rect);

			foreach (Vector2Int cellPosition in map)
			{
				IPlatoonCellView cellView = GameObject.Instantiate(_config.PlatoonCellPrefab, _view.Transform);
				map[cellPosition] = new PlatoonCell(cellView, _camera);
				Vector3 worldPosition = GetCellWorldPosition(cellPosition);
				cellView.SetPosition(worldPosition);
			}
			
			_platoon.InitMap(map);
		}

		private Vector3 GetCellWorldPosition(Vector2Int localPosition)
		{
			Vector3 worldPosition = localPosition.x0y();
			Vector3 scaleBase = new Vector3(1, 0, -1);
			worldPosition.Scale(scaleBase * _config.PlatoonCellWidth);
			worldPosition = _view.Transform.localRotation * worldPosition;
			worldPosition += _viewOffset;

			return worldPosition;
		}
	}
}