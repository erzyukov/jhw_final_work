namespace Game.Profiles
{
	using Game.Utilities;
	using UniRx;
	using YG;
	using Zenject;

	public class YandexProfileSaver : ControllerBase, IProfileSaver, IInitializable
	{
		public void Initialize()
		{
			Observable.FromEvent(
					x => YandexGame.GetDataEvent += x,
					x => YandexGame.GetDataEvent -= x
				)
				.Subscribe(_ => SaveSystemReady.Execute())
				.AddTo(this);
		}

		#region IProfileSaver

		public ReactiveCommand SaveSystemReady { get; } = new ReactiveCommand();

		public bool Load(out GameProfile data)
		{
			data = YandexGame.savesData.gameProfile;

			return data != null;
		}

		public void Save(GameProfile data)
		{
			YandexGame.savesData.gameProfile = data;
			YandexGame.SaveProgress();
		}

		#endregion
	}
}
