namespace Game.Platoon
{
    public class HeroPlatoonFacade : PlatoonFacade
	{
		private void Start()
		{
			BattleSimulator.RegisterHeroPlatoonFacade(this);
		}
	}
}