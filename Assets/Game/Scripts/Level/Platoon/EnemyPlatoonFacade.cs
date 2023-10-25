namespace Game.Platoon
{
    public class EnemyPlatoonFacade : PlatoonFacade
	{
		private void Start()
		{
			BattleSimulator.RegisterEnemyPlatoonFacade(this);
		}
	}
}
