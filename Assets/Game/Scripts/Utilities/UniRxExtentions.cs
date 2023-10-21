namespace Game.Utilities
{
	using System;
	using UniRx;

	public class ControllerBase : IDisposable
	{
		readonly CompositeDisposable _disposable = new CompositeDisposable();

		void IDisposable.Dispose() => _disposable.Dispose();

		public void AddDisposable(IDisposable item)
		{
			_disposable.Add(item);
		}
	}

	public static class DisposableExtensions
	{
		public static T AddTo<T>(this T disposable, ControllerBase controller) where T : IDisposable
		{
			controller.AddDisposable(disposable);

			return disposable;
		}
	}
}