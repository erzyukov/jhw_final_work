namespace Game.Profiles
{
	using UniRx;

	public interface IProfileSaver
	{
		ReactiveCommand SaveSystemReady { get; }
		bool Load(out GameProfile data);
		void Save(GameProfile data);
	}
}
