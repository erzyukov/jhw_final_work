namespace Game.Field
{
	using Configs;
	using Utilities;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

	public class FieldBuilder: IStartable
	{
		[Inject] private FieldType _fieldType;
		[Inject] private BattleFieldConfig _config;
		[Inject] private FieldCellView _cellViewPrefab;
		[Inject] private IField<FieldCell> _field;
		[Inject] private IFieldView _view;

		private Vector3 _viewOffset;

		public void Start()
		{
			float zSign = Mathf.Sign(_view.Transform.position.z);
			float fieldXOffset = zSign * (_config.TeamFieldSize.x - 1) / 2f * _config.FieldCellWidth;
			_viewOffset = _view.Transform.position.WithX(fieldXOffset);
			_viewOffset = _viewOffset.WithZ(_viewOffset.z + zSign * _config.FieldCellWidth * 0.5f);
			_view.SetPosition(_viewOffset);

			RectInt rect = new RectInt(Vector2Int.zero, _config.TeamFieldSize);
			Map<FieldCell> map = new Map<FieldCell>(rect);

			foreach (Vector2Int cellPosition in map)
			{
				IFieldCellView cellView = GameObject.Instantiate(_cellViewPrefab, _view.Transform);
				cellView.SetColor(GetCellColor(cellPosition));
				map[cellPosition] = new FieldCell(cellView, cellPosition);
				Vector3 worldPosition = GetCellWorldPosition(cellPosition);
				cellView.SetPosition(worldPosition);
			}

			_field.InitMap(map);
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

		private Color GetCellColor(Vector2Int cellPosition)
		{
			bool isEven = (cellPosition.x + cellPosition.y) % 2 == 0;
			Color color = _fieldType == FieldType.Hero
				? (isEven) ? _config.HeroEvenCollor : _config.HeroOddCollor
				:  (isEven) ? _config.EnemyEvenCollor : _config.EnemyOddCollor;
			return color;
		}
	}
}