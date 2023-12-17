namespace Game.Configs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Energy", menuName = "Configs/Energy", order = (int)Config.Energy)]
    public class EnergyConfig : ScriptableObject
    {
        [SerializeField] private int _maxEnery;
        [SerializeField] private int _levelPrice;
        [SerializeField] private int _secondsToRestoreOnePoint;

        public int MaxEnery => _maxEnery;
        public int LevelPrice => _levelPrice;
        public int SecondsToRestoreOnePoint => _secondsToRestoreOnePoint;
    }
}