namespace Game.Ui
{
    using Game.Utilities;
    using Zenject;
    using UniRx;
    using Game.Core;
    using Game.Profiles;
    using Game.Configs;
    using UnityEngine;

    public class UiLobby : ControllerBase, IInitializable
    {
        [Inject] private IUiLobbyScreen _lobbyScreen;
        [Inject] private IUiMessage _uiMessage;
        [Inject] private IGameLevel _gameLevel;
        [Inject] private IGameEnergy _gameEnergy;
        [Inject] private GameProfile _profile;
        [Inject] private LevelsConfig _levelsConfig;
        [Inject] private EnergyConfig _energyConfig;
        [Inject] private ILocalizator _localizator;
        [Inject] private IContinueLevelRequest _continueLevelRequest;

        private const string LevelTitlePrefixKey = "level";
        private const string lastWavePrefixKey = "uiLastWave";
        private const string playKey = "play";
        private const string pricePrefixKey = "pricePrefix";

        private string _levelTitlePrefix;

        private bool IsPlayEnergyFree => _profile.WaveNumber.Value != 0 || _profile.LevelNumber.Value < _energyConfig.FreeLevelTo;

        public void Initialize()
        {
            _levelTitlePrefix = _localizator.GetString(LevelTitlePrefixKey);

            _lobbyScreen.Opening
                .Subscribe(_ => OnScreenOpeningHandler())
                .AddTo(this);

            _lobbyScreen.PlayButtonClicked
                .Subscribe(_ => OnPlayButtonClickedHandler())
                .AddTo(this);
        }

        private void OnScreenOpeningHandler()
        {
            int levelIndex = _profile.LevelNumber.Value - 1;
            LevelConfig levelConfig = _levelsConfig.Levels[levelIndex];

            _lobbyScreen.SetTitle($"{_levelTitlePrefix} {levelConfig.Title}");

            _lobbyScreen.SetLastWaveActive(_profile.WaveNumber.Value != 0);
            string waveInfo = $"{_localizator.GetString(lastWavePrefixKey)} {_profile.WaveNumber.Value}/{levelConfig.Waves.Length}";
            _lobbyScreen.SetLastWaveValue(waveInfo);

            string playButtonTitle = _localizator.GetString(playKey)
                + (IsPlayEnergyFree == false ? $"\n{_localizator.GetString(pricePrefixKey)}{_energyConfig.LevelPrice}" : "");
            _lobbyScreen.SetPlayButtonText(playButtonTitle);
        }

        private void OnPlayButtonClickedHandler()
        {
            if (_profile.WaveNumber.Value == 0 || _profile.LevelNumber.Value < _energyConfig.FreeLevelTo)
                GoToLevel();
            else
                _continueLevelRequest.ShowRequest(() => GoToLevel(true), () => GoToLevel());
        }

        private void GoToLevel(bool resetWave = false)
        {
            if (resetWave)
                _profile.WaveNumber.Value = 0;

            if (IsPlayEnergyFree == false)
            {
                if (_gameEnergy.TryPayLevel())
                    _gameLevel.GoToLevel(_profile.LevelNumber.Value);
                else
                    _uiMessage.ShowMessage(UiMessage.NotEnoughEnergy);
            }
            else
            {
                _gameLevel.GoToLevel(_profile.LevelNumber.Value);
            }
        }
    }
}
