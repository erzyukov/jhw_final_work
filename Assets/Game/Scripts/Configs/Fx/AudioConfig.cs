namespace Game.Configs
{
	using UnityEngine;
	using Game.Units;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;
	using Game.Core;
	using System;
	using Game.Weapon;

	[CreateAssetMenu(fileName = "Audio", menuName = "Configs/Audio", order = (int)Config.Audio)]
	public class AudioConfig : SerializedScriptableObject
	{
		[Header("UI")]
		[SerializeField] private float _defaultUiVolume;
		[SerializeField] private AudioClip _uiButtonClick;

		[Header("Music")]
		[SerializeField] private float _defaultMusicVolume;
		[SerializeField] private float _fadeDuration;

		[Header("Sound")]
		[SerializeField] private float _defaultSoundVolume;

		[Header("Units")]
		[SerializeField] private AudioClip _unitMerge;
		[SerializeField] private Dictionary<Species, AudioClip> _shoot = new();
		[SerializeField] private Dictionary<ProjectileType, AudioClip> _projectile = new();
		
		[Header("Game")]
		[SerializeField] private Dictionary<GameState, GameStateAudioClip> _gameStateThemes = new();

		public float DefaultUiVolume => _defaultUiVolume;
		public AudioClip UiButtonClick => _uiButtonClick;
		public float DefaultSoundVolume => _defaultSoundVolume;
		public float DefaultMusicVolume => _defaultMusicVolume;
		public float FadeDuration => _fadeDuration;
		public AudioClip UnitMerge => _unitMerge;

		public AudioClip GetShootClip( Species species )
		{
			if (_shoot.TryGetValue( species, out AudioClip value ))
				return value;
			
			return null;
		}

		public AudioClip GetProjectileClip( ProjectileType type )
		{
			if (_projectile.TryGetValue( type, out AudioClip value ))
				return value;
			
			return null;
		}

		public AudioClip GetGameStateTheme( GameState state, out float volume )
		{
			if (_gameStateThemes.TryGetValue( state, out GameStateAudioClip value ))
			{
				volume = value.Volume;
				return value.AudioClip;
			}

			volume = 0;
			return null;
		}

		[Serializable]
		public struct GameStateAudioClip
		{
			public float Volume;
			public AudioClip AudioClip;
		}
	}
}
