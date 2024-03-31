namespace Game.Iap
{
	using System;
	using UniRx;
	using Zenject;

	public class IapCoreListener : IInitializable, IDisposable
	{
		[Inject] protected IIapCore IapCore;
		[Inject] protected IIapCoreFacade IapCoreFacade;
		[Inject] protected IIapConfig IapConfig;

		protected CompositeDisposable LifetimeDisposable = new();

		public virtual void Initialize() {}

		public void Dispose() => LifetimeDisposable.Dispose();

	}
}
