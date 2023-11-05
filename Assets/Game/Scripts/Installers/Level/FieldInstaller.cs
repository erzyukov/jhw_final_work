namespace Game.Installers
{
	using Configs;
	using Field;
	using VContainer;
	using VContainer.Unity;

	public class FieldInstaller : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			builder.Register(container =>
				{
					BattleFieldConfig config = container.Resolve<BattleFieldConfig>();
					return config.FieldCellView;
				},
				Lifetime.Singleton
			);

			FieldView view = GetComponent<FieldView>();
			builder.RegisterComponent(view).AsImplementedInterfaces();

			builder.Register<Field<FieldCell>>(Lifetime.Scoped).AsImplementedInterfaces();
			builder.Register<FieldBuilder>(Lifetime.Scoped).AsImplementedInterfaces();
		}
	}
}