namespace Game
{
	using Game.Units;

    public struct Rewarded
    {
        public ERewardedType Type;
        public Species? UpgradeUnit;

        public Rewarded(ERewardedType type)
        {
            this = default;
            Type = type;
        }

        public Rewarded(Species species)
        {
            this = default;
            UpgradeUnit = species;
        }
    }
}
