namespace Game.Platoon
{
	using Core;
	using Configs;
	using Utilities;
	using UnityEngine;
	using Zenject;

	public class PlatoonBuilder : ControllerBase, IInitializable
	{
		[Inject] private BattleFieldConfig _config;
		[Inject] private PlatoonCellView _cellViewPrefab;
		[Inject] private IPlatoon _platoon;
		[Inject] private IPlatoonView _view;
		[Inject] private Camera _camera;
		[Inject] private BattleFieldConfig _battleFieldConfig;


		private Vector3 _viewOffset;

		public void Initialize()
		{
			float zSign = Mathf.Sign(_view.Transform.position.z);
			float platoonXOffset = zSign * _battleFieldConfig.TeamFieldSize.x / 2f * _config.FieldCellWidth;
			_viewOffset = _view.Transform.position.WithX(platoonXOffset);
			_view.SetPosition(_viewOffset);

			RectInt rect = new RectInt(Vector2Int.zero, _battleFieldConfig.TeamFieldSize);
			Map<PlatoonCell> map = new Map<PlatoonCell>(rect);

			foreach (Vector2Int cellPosition in map)
			{
				IPlatoonCellView cellView = GameObject.Instantiate(_cellViewPrefab, _view.Transform);
				map[cellPosition] = new PlatoonCell(cellView, _camera, cellPosition);
				Vector3 worldPosition = GetCellWorldPosition(cellPosition);
				cellView.SetPosition(worldPosition);
			}
			
			_platoon.InitMap(map);
		}

		private Vector3 GetCellWorldPosition(Vector2Int localPosition)
		{
			Vector3 worldPosition = localPosition.x0y();
			Vector3 scaleBase = new Vector3(1, 0, -1);
			worldPosition.Scale(scaleBase * _config.FieldCellWidth);
			worldPosition = _view.Transform.localRotation * worldPosition;
			worldPosition += _viewOffset;

			return worldPosition;
		}
	}
}