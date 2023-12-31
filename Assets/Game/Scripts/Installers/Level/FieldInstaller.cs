﻿namespace Game.Installers
{
	using Game.Configs;
	using Game.Field;
	using Game.Dev;
	using UnityEngine;
	using Zenject;
	
	public class FieldInstaller : MonoInstaller
	{
		[Inject] private BattleFieldConfig _battleFieldConfig;

		[SerializeField] private FieldType _fieldType;

		public override void InstallBindings()
		{
			WebGLDebug.Log($"[Project] FieldInstaller: Configure: {_fieldType}");

			Container.BindInstance(_fieldType);
			Container.BindInstance(_battleFieldConfig.FieldCellView);

			Container
				.BindInterfacesTo<FieldView>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<Field<FieldCell>>()
				.AsSingle();

			Container
				.BindInterfacesTo<FieldBuilder>()
				.AsSingle();

			Container
				.BindInterfacesTo<FieldEvents>()
				.AsSingle();

			Container
				.BindInterfacesTo<FieldUnits>()
				.AsSingle();
		}
	}
}