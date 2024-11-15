﻿namespace Game.Gameplay
{
	using Game.Configs;
	using Game.Core;
	using Game.Field;
	using Game.Units;
	using Game.Utilities;
	using Game.Weapon;
	using Sirenix.Utilities;
	using System;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class BattleStageHandler : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle			_gameCycle;
		[Inject] private IGameLevel			_gameLevel;
		[Inject] private IGameCurrency		_gameCurrency;
		[Inject] private IGameHero			_gameHero;
		[Inject] private IFieldHeroFacade	_fieldHeroFacade;
		[Inject] private IFieldEnemyFacade	_fieldEnemyFacade;
		[Inject] private TimingsConfig		_timingsConfig;
		[Inject] private UnitsConfig		_unitsConfig;
		[Inject] private IFieldFacade[]		_fields;
		[Inject] private IGameAudio			_gameAudio;
		[Inject] private IGameplayEvents	_gameplayEvents;
		[Inject] private IBattleEvents		_battleEvents;

		private const float		LoseAdditionalDelay = 1f;

		private IDisposable		_loseDelayTimer;

		public void Initialize()
		{
			_gameCycle.State
				.Where( state => state == GameState.BattleStage )
				.Subscribe( _ => OnBattleStageHandler() )
				.AddTo( this );

			_fieldEnemyFacade.Units.ObserveCountChanged()
				.Subscribe( OnEnemyUnitsCountChanged )
				.AddTo( this );

			_fieldHeroFacade.AliveUnitsCount
				.Where( _ => _gameCycle.State.Value == GameState.BattleStage )
				.Subscribe( OnHeroUnitsCountChanged )
				.AddTo( this );

			_fieldEnemyFacade.Events.UnitDying
				.Subscribe( OnEnemyUnitDyingHandler )
				.AddTo( this );

			_fields.ForEach( field => 
				field.Events.UnitAttacking
					.Subscribe( unit => _gameAudio.PlayUnitShoot( unit.Species ) )
					.AddTo( this )
			);

			_battleEvents.DamageApplyed
				.Where( d => d.ProjectileType != ProjectileType.None )
				.Subscribe( d => _gameAudio.PlayProjectileHit(d.ProjectileType) )
				.AddTo( this );
		}

		public override void Dispose()
		{
			base.Dispose();
			_loseDelayTimer?.Dispose();
		}

		private void OnEnemyUnitDyingHandler(IUnitFacade unit)
		{
			if (_unitsConfig.Units.TryGetValue(unit.Species, out UnitConfig unitConfig) == false)
				return;

			int reward = Mathf.CeilToInt(unitConfig.SoftReward + unitConfig.SoftRewardPowerMultiplier * unit.Power);
			_gameCurrency.AddLevelSoftCurrency(reward);

			int experience = Mathf.CeilToInt(unitConfig.Experience + unitConfig.ExperiencePowerMultiplier * unit.Power);
			_gameHero.AddLevelHeroExperience(experience);
		}

		private void OnHeroUnitsCountChanged(int count)
		{
			if (count == 0 && _gameCycle.State.Value == GameState.BattleStage)
			{
				float showLoseDelay		= LoseAdditionalDelay + _timingsConfig.WaveTransitionDelay;
				_loseDelayTimer			= Observable.Timer( TimeSpan.FromSeconds( showLoseDelay ))
					.Subscribe( _ =>
					{
						_gameplayEvents.BattleLost.Execute( CreateBattlefieldData() );
						_gameCycle.SetState(GameState.LoseBattle);
					} )
					.AddTo(this);
			}
		}

		private void OnEnemyUnitsCountChanged(int count)
		{
			if (count == 0)
			{
				Observable.Timer(TimeSpan.FromSeconds(_timingsConfig.WaveTransitionDelay))
					.Subscribe(_ =>
					{
						_loseDelayTimer?.Dispose();
						_gameplayEvents.BattleWon.Execute(CreateBattlefieldData());
						_gameCycle.SetState(GameState.CompleteWave);
						_gameLevel.GoToNextWave();
					})
					.AddTo(this);
			}
		}

		private void OnBattleStageHandler()
		{
			_fieldHeroFacade.SetFieldRenderEnabled(false);
			_fieldHeroFacade.SetDraggableActive(false);
			_fieldEnemyFacade.SetFieldRenderEnabled(false);

			foreach (var unit in _fieldHeroFacade.Units)
				unit.EnterBattle();

			foreach (var unit in _fieldEnemyFacade.Units)
				unit.EnterBattle();

			_gameplayEvents.BattleStarted.Execute(CreateBattlefieldData());
		}

		private BattlefieldData CreateBattlefieldData() => new BattlefieldData
		{
			HeroField = _fieldHeroFacade,
			EnemyField = _fieldEnemyFacade
		};
	}
}