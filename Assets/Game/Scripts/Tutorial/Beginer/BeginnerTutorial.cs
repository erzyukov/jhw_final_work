namespace Game.Tutorial
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System;
	using Game.Core;
	using Game.Ui;
	using Game.Field;
	using Game.Configs;
	using Game.Level;
	using System.Linq;
	using Game.Units;

	public class BeginnerTutorial : BeginerTutorialFsmBase, IInitializable, IDisposable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameCycle _cycle;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IUiTacticalStageHud _uiTacticalStageHud;
		[Inject] private IFingerHint _fingerHint;
		[Inject] private IFingerSlideHint _fingerSlideHint;
		[Inject] private IDialogHint _dialogHint;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private TutorialConfig _config;
		[Inject] private ILocalizator _localizator;
		[Inject] private IHeroUnitSummoner _heroUnitSummoner;
		[Inject] private CurrencyConfig _currencyConfig;
		[Inject] private IGameProfileManager _gameProfileManager;

		readonly private CompositeDisposable _disposable = new CompositeDisposable();
		private IDisposable _summonInterruptDisposable;
		bool _isUnitSummoned;

		public void Initialize()
		{
			Observable.CombineLatest(
					_profile.Tutorial.BeginnerStep,
					_gameLevel.LevelLoading,
					(step, _) => (step, _)
				)
				.Where(v => v.step != State)
				.Subscribe(v => OnBeginnerStepChanged(v.step))
				.AddTo(_disposable);

			_summonInterruptDisposable = _heroUnitSummoner.SummoningPaidUnit
				.Where(_ => _profile.Tutorial.BeginnerStep.Value != BeginnerStep.Complete)
				.Subscribe(_ => OnSummoningPaidUnitHandler());

			_fieldHeroFacade.Units.ObserveCountChanged()
				.Where(_ => State == BeginnerStep.LastSummon)
				.Subscribe(_ => _uiTacticalStageHud.SetStartBattleButtonInteractable(false))
				.AddTo(_disposable);
		}

		public virtual void Dispose()
		{
			_summonInterruptDisposable?.Dispose();
			_disposable.Dispose();
		}

		protected override void StateTransitions()
		{
			switch (State)
			{
				case BeginnerStep.FirstSummon:
					if (_isUnitSummoned)
					{
						_isUnitSummoned = false;
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.SecondSummon;
					}
					break;

				case BeginnerStep.SecondSummon:
					if (_isUnitSummoned)
					{
						_isUnitSummoned = false;
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.FirstBattle;
					}
					break;

				case BeginnerStep.FirstBattle:
					if (_cycle.State.Value == GameState.BattleStage)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.PauseForFirstBattle;
					break;

				case BeginnerStep.PauseForFirstBattle:
					if (_cycle.State.Value == GameState.TacticalStage)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.ThirdSummon;
					break;

				case BeginnerStep.ThirdSummon:
					if (_isUnitSummoned)
					{
						_isUnitSummoned = false;
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.FourthSummon;
					}
					break;

				case BeginnerStep.FourthSummon:
					if (_isUnitSummoned)
					{
						_isUnitSummoned = false;
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.FirstMerge;
					}
					break;

				case BeginnerStep.FirstMerge:
					if (_fieldHeroFacade.Units.Count == 3)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.SecondMerge;
					break;

				case BeginnerStep.SecondMerge:
					if (_fieldHeroFacade.Units.Count == 2)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.SecondBattle;
					break;

				case BeginnerStep.SecondBattle:
					if (_cycle.State.Value == GameState.BattleStage)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.PauseForSecondBattle;
					break;

				case BeginnerStep.PauseForSecondBattle:
					if (_cycle.State.Value == GameState.TacticalStage)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.LastSummon;
					break;

				case BeginnerStep.LastSummon:
					_isUnitSummoned = false;

					if (_profile.SummonCurrency.Value < _currencyConfig.UnitSummonPrice)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.ThirdBattle;
					break;

				case BeginnerStep.ThirdBattle:
					if (_cycle.State.Value == GameState.BattleStage)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.PauseForThirdBattle;
					break;

				case BeginnerStep.PauseForThirdBattle:
					if (_cycle.State.Value == GameState.WinBattle)
						_profile.Tutorial.BeginnerStep.Value = BeginnerStep.Complete;
					break;
			}
		}

		#region FirstSummon Step

		protected override void OnEnterFirstSummon()
		{
			SetupSummonStep();
		}

		protected override void OnExitFirstSummon()
		{
			_fingerHint.Hide();
			_dialogHint.SetActive(false);
		}

		#endregion

		#region SecondSummon Step

		protected override void OnEnterSecondSummon()
		{
			SetupSummonStep();
		}

		protected override void OnExitSecondSummon()
		{
			_fingerHint.Hide();
			_dialogHint.SetActive(false);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(true);
		}

		#endregion

		#region FirstBattle Step

		protected override void OnEnterFirstBattle()
		{
			SetupBattleStep();
		}

		protected override void OnExitFirstBattle()
		{
			_fingerHint.Hide();
			_uiTacticalStageHud.SetSummonButtonInteractable(true);
		}

		#endregion

		#region ThirdSummon Step

		protected override void OnEnterThirdSummon()
		{
			SetupSummonStep(false);
		}

		protected override void OnExitThirdSummon()
		{
			_fingerHint.Hide();
		}

		#endregion

		#region FourthSummon Step

		protected override void OnEnterFourthSummon()
		{
			SetupSummonStep(false);
		}

		protected override void OnExitFourthSummon()
		{
			_fingerHint.Hide();
		}

		#endregion

		#region FirstMerge Step

		protected override void OnEnterFirstMerge()
		{
			SetupMergetep();
		}

		protected override void OnExitFirstMerge()
		{
			SetupExitMergeStep();
		}

		#endregion

		#region SecondMerge Step

		protected override void OnEnterSecondMerge()
		{
			SetupMergetep();
		}

		protected override void OnExitSecondMerge()
		{
			SetupExitMergeStep();
		}

		#endregion

		#region SecondBattle Step

		protected override void OnEnterSecondBattle()
		{
			SetupBattleStep();
		}

		protected override void OnExitSecondBattle()
		{
			_fingerHint.Hide();
			_uiTacticalStageHud.SetSummonButtonInteractable(true);
		}

		#endregion

		#region LastSummon Step

		protected override void OnEnterSixthSummon()
		{
			SetupSummonStep(false);
			_fieldHeroFacade.SetDraggableActive(true);
		}

		protected override void OnExitSixthSummon()
		{
			_fingerHint.Hide();
			_summonInterruptDisposable.Dispose();
		}

		#endregion

		#region ThirdBattle

		protected override void OnEnterThirdBattle()
		{
			SetupBattleStep();
			ActivateDialogMessege();
		}

		protected override void OnExitThirdBattle()
		{
			_fingerHint.Hide();
			_uiTacticalStageHud.SetSummonButtonInteractable(true);
			_dialogHint.SetActive(false);
		}

		#endregion

		private void OnBeginnerStepChanged(BeginnerStep step)
		{
			_gameProfileManager.Save();
			Transition(step);

			if (step == BeginnerStep.Complete)
				Dispose();
		}

		private void SetupBattleStep()
		{
			_fingerHint.Show(FingerPlace.TacticalStageStart);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(true);
			_uiTacticalStageHud.SetSummonButtonInteractable(false);
		}

		private void SetupSummonStep(bool withDialog = true)
		{
			_fingerHint.Show(FingerPlace.TacticalStageSummon);

			if (withDialog)
				ActivateDialogMessege();

			_fieldHeroFacade.SetDraggableActive(false);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(false);
		}

		private void SetupMergetep()
		{
			if (_config.BeginerTurorialMerges.TryGetValue(State, out var data) == false)
				return;

			IFieldCell fromCell = _fieldHeroFacade.GetCell(data.FromPosition);
			IFieldCell targetCell = _fieldHeroFacade.GetCell(data.ToPosition);

			fromCell.Select();
			targetCell.Select();

			IUnitFacade unit = fromCell.Unit;

			unit.SetDraggableActive(true);

			_fingerSlideHint.SetActive(true);
			_fingerSlideHint.SetPositions(fromCell.WorldPosition, targetCell.WorldPosition);
			_fingerSlideHint.PlayAnimation();

			_uiTacticalStageHud.SetSummonButtonInteractable(false);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(false);

			ActivateDialogMessege();
		}

		private void SetupExitMergeStep()
		{
			_fingerSlideHint.SetActive(false);
			_dialogHint.SetActive(false);

			if (_config.BeginerTurorialMerges.TryGetValue(State, out var data) == false)
				return;

			IFieldCell fromCell = _fieldHeroFacade.GetCell(data.FromPosition);
			fromCell.Deselect();
		}

		private void OnSummoningPaidUnitHandler()
		{
			_isUnitSummoned = true;

			if (_config.BeginerTurorialSummons.TryGetValue(State, out var data))
			{
				_heroUnitSummoner.InterruptPaidSummon();
				_heroUnitSummoner.Summon(data.Species, data.GradeIndex, data.Power, data.Position);
			}

			_uiTacticalStageHud.SetStartBattleButtonInteractable(false);
		}

		private void ActivateDialogMessege()
		{
			if (_config.BeginerTutorialMessages.TryGetValue(State, out string key))
			{
				_dialogHint.SetMessage(_localizator.GetString(key));
				_dialogHint.SetActive(true);
			}
		}
	}
}
