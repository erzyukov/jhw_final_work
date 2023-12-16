namespace Game.Configs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Energy", menuName = "Configs/Energy", order = (int)Config.Energy)]
    public class EnergyConfig : ScriptableObject
    {
        [SerializeField] private int _maxEnery;
        [SerializeField] private float _secondsToRestoreEnergyPoint;

        public int MaxEnery => _maxEnery;
        public  float SecondsToRestoreEnergyPoint => _secondsToRestoreEnergyPoint;
    }
}