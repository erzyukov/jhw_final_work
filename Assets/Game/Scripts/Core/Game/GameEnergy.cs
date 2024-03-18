namespace Game.Core
{
    using Game.Utilities;
    using Game.Profiles;
    using Game.Configs;
    using Zenject;
    using UniRx;
    using System;
    using UnityEngine;
	using Game.Ui;
	using UnityEngine.UI;
	using Game.Managers;

	public interface IGameEnergy
    {
        FloatReactiveProperty EnergyRatio { get; }
        IntReactiveProperty SecondsToRestoreOnePoint { get; }
		ReactiveCommand<int> EnergySpent { get; }
        bool TryPayLevel();
    }

    public class GameEnergy : ControllerBase, IGameEnergy, IInitializable, ITickable
    {
        [Inject] private GameProfile _profile;
        [Inject] private IGameCycle _gameCycle;
        [Inject] private IGameProfileManager _gameProfileManager;
        [Inject] private EnergyConfig _config;
		[Inject] private IAdsManager _adsManager;

		private EnergyProfile Energy => _profile.Energy;

		private float _startRestoreTime;
        private int _secondsToOnePointPassed;

        public void Initialize()
        {
            SecondsToRestoreOnePoint.Value = _config.SecondsToRestoreOnePoint;

            Energy.Amount
                .Subscribe(OnEnergyValueChanged)
                .AddTo(this);

            _gameCycle.State
                .Where(state => state == GameState.Lobby)
                .Subscribe(state => UpdateRatio())
                .AddTo(this);

            SecondsToRestoreOnePoint
                .Where(value => value == 0)
                .Subscribe(_ => Save())
                .AddTo(this);

			_adsManager.OnCompleted[ ERewardedType.AddEnergy ]
				.Subscribe( _ => OnEnergyAdsReward() )
				.AddTo( this );

            if (Energy.Amount.Value < _config.MaxEnery)
            {
                TimeSpan passedFromLastSave = DateTime.Now.Subtract(Energy.LastEnergyChange);
                int passedSectondsFromLastSave = Mathf.FloorToInt((float)passedFromLastSave.TotalSeconds);
                _startRestoreTime = Time.unscaledTime - passedSectondsFromLastSave;
            }
        }

        public void Tick()
        {
            if (Energy.Amount.Value >= _config.MaxEnery)
                return;

            float passedTime = Time.unscaledTime - _startRestoreTime;
            int passedSeconds = Mathf.FloorToInt(passedTime);
            _startRestoreTime = Time.unscaledTime - (passedTime - passedSeconds);

            int restoredPoints = (_secondsToOnePointPassed + passedSeconds) / _config.SecondsToRestoreOnePoint;
            int remainSecondsToRestore = (_secondsToOnePointPassed + passedSeconds) % _config.SecondsToRestoreOnePoint;

            if (restoredPoints > 0)
            {
                Energy.Amount.Value = Mathf.Min(Energy.Amount.Value + restoredPoints, _config.MaxEnery);
                Save();

                if (Energy.Amount.Value >= _config.MaxEnery)
                {
                    _startRestoreTime = 0;
                    _secondsToOnePointPassed = 0;
                }
            }

            _secondsToOnePointPassed = remainSecondsToRestore;
            SecondsToRestoreOnePoint.Value = _config.SecondsToRestoreOnePoint - remainSecondsToRestore;
        }

        #region IGameEnergy

        public FloatReactiveProperty EnergyRatio { get; } = new FloatReactiveProperty();

        public IntReactiveProperty SecondsToRestoreOnePoint { get; } = new IntReactiveProperty();

		public ReactiveCommand<int> EnergySpent { get; } = new ReactiveCommand<int>();

		public bool TryPayLevel()
        {
            if (Energy.Amount.Value < _config.LevelPrice)
                return false;

            Energy.Amount.Value -= _config.LevelPrice;
            Save();

			EnergySpent.Execute(_config.LevelPrice);

			return true;
        }

        #endregion

        private void OnEnergyValueChanged(int value)
        {
            UpdateRatio();

            if (Energy.Amount.Value < _config.MaxEnery)
                StartEnergyRestore();
        }

        private void StartEnergyRestore()
        {
            _startRestoreTime = Time.unscaledTime;
            _secondsToOnePointPassed = 0;
        }

        private void UpdateRatio() =>
            EnergyRatio.SetValueAndForceNotify((float)Energy.Amount.Value / _config.MaxEnery);

		private void OnEnergyAdsReward()
		{
			Energy.Amount.Value += _config.LevelPrice;
			Save();
		}

        private void Save()
        {
			Energy.LastEnergyChange = DateTime.Now;
            _gameProfileManager.Save();
        }
    }
}