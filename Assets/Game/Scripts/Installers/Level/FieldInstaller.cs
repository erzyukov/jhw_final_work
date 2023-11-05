namespace Game.Installers
{
	using Configs;
	using Field;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

	public class FieldInstaller : LifetimeScope
	{
		[SerializeField] private FieldType _fieldType;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.Register(container =>
				{
					BattleFieldConfig config = container.Resolve<BattleFieldConfig>();
					return config.FieldCellView;
				},
				Lifetime.Singleton
			);

			builder.RegisterInstance(_fieldType);

			FieldView view = GetComponent<FieldView>();
			builder.RegisterComponent(view).AsImplementedInterfaces();

			builder.Register<Field<FieldCell>>(Lifetime.Scoped).AsImplementedInterfaces();
			builder.Register<FieldBuilder>(Lifetime.Scoped).AsImplementedInterfaces();
		}
	}
}