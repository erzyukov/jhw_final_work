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
	using UnityEngine;
	using Game.Units;

	public class BeginnerTutorial: BeginerTutorialFsmBase, IInitializable, IDisposable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameCycle _cycle;
		[Inject] private IUiTacticalStageHud _uiTacticalStageHud;
		[Inject] private IFingerHint _fingerHint;
		[Inject] private IDialogHint _dialogHint;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private TutorialConfig _config;
		[Inject] private ILocalizator _localizator;
		[Inject] private IHeroUnitSummoner _heroUnitSummoner;

		readonly private CompositeDisposable _disposable = new CompositeDisposable();
		private IDisposable _summonInterruptDisposable;

		public void Initialize()
		{
			_profile.Tutorial.BeginerStep
				.Where(step => step != State)
				.Subscribe(Transition)
				.AddTo(_disposable);

			_summonInterruptDisposable = _heroUnitSummoner.SummoningPaidUnit
				.Where(_ => _profile.Tutorial.BeginerStep.Value != BeginnerStep.Complete)
				.Subscribe(_ => OnSummoningPaidUnitHandler());
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
					if (_fieldHeroFacade.Units.Count == 1)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.SecondSummon;
					break;
				case BeginnerStep.SecondSummon:
					if (_fieldHeroFacade.Units.Count == 2)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.FirstBattle;
					break;
				case BeginnerStep.FirstBattle:
					if (_cycle.State.Value == GameState.BattleStage)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.PauseForFirstBattle;
					break;
				case BeginnerStep.PauseForFirstBattle:
					if (_cycle.State.Value == GameState.TacticalStage)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.ThirdSummon;
					break;
				case BeginnerStep.ThirdSummon:
					if (_fieldHeroFacade.Units.Count == 3)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.FourthSummon;
					break;
				case BeginnerStep.FourthSummon:
					if (_fieldHeroFacade.Units.Count == 4)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.FirstMerge;
					break;
				case BeginnerStep.FirstMerge:
					if (_fieldHeroFacade.Units.Count == 3)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.SecondMerge;
					break;
				case BeginnerStep.SecondMerge:
					if (_fieldHeroFacade.Units.Count == 2)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.SecondBattle;
					break;
				case BeginnerStep.SecondBattle:
					if (_cycle.State.Value == GameState.BattleStage)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.PauseForSecondBattle;
					break;
				case BeginnerStep.PauseForSecondBattle:
					if (_cycle.State.Value == GameState.TacticalStage)
						_profile.Tutorial.BeginerStep.Value = BeginnerStep.FifthSummon;
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
			_fingerHint.SetActive(false);
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
			_fingerHint.SetActive(false);
			_dialogHint.SetActive(false);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(true);
		}

		#endregion

		#region FirstBattle Step

		protected override void OnEnterFirstBattle()
		{
			_fingerHint.SetPosition(_uiTacticalStageHud.BattleButtonHintParameters.Point.position);
			_fingerHint.SetLeft(_uiTacticalStageHud.BattleButtonHintParameters.IsLeft);
			_fingerHint.SetActive(true);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(true);
			_uiTacticalStageHud.SetSummonButtonInteractable(false);
		}

		protected override void OnExitFirstBattle()
		{
			_fingerHint.SetActive(false);
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
			_fingerHint.SetActive(false);
		}

		#endregion

		#region FourthSummon Step

		protected override void OnEnterFourthSummon()
		{
			SetupSummonStep(false);
		}

		protected override void OnExitFourthSummon()
		{
			_fingerHint.SetActive(false);
			_summonInterruptDisposable.Dispose();
		}

		#endregion

		#region FirstMerge Step

		protected override void OnEnterFirstMerge()
		{
			Vector2Int position = new Vector2Int(0, 1);
			IUnitFacade unit = _fieldHeroFacade.Units
				.Where(unit => _fieldHeroFacade.GetCell(unit).Position == position)
				.FirstOrDefault();

			unit.SetDraggableActive(true);

			_uiTacticalStageHud.SetSummonButtonInteractable(false);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(false);

			ActivateDialogMessege();
		}

		protected override void OnExitFirstMerge()
		{
			_dialogHint.SetActive(false);
		}

		#endregion

		#region SecondMerge Step

		protected override void OnEnterSecondMerge()
		{
			Vector2Int position = new Vector2Int(1, 0);
			IUnitFacade unit = _fieldHeroFacade.Units
				.Where(unit => _fieldHeroFacade.GetCell(unit).Position == position)
				.FirstOrDefault();

			unit.SetDraggableActive(true);

			ActivateDialogMessege();
		}

		protected override void OnExitSecondMerge()
		{
			_dialogHint.SetActive(false);
		}

		#endregion

		private void SetupSummonStep(bool withDialog = true)
		{
			_fingerHint.SetPosition(_uiTacticalStageHud.SummonButtonHintParameters.Point.position);
			_fingerHint.SetLeft(_uiTacticalStageHud.SummonButtonHintParameters.IsLeft);
			_fingerHint.SetActive(true);

			if (withDialog)
				ActivateDialogMessege();

			_fieldHeroFacade.SetDraggableActive(false);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(false);
		}

		private void OnSummoningPaidUnitHandler()
		{
			if (_config.BeginerTurorialSummons.TryGetValue(State, out var data))
			{
				_heroUnitSummoner.InterruptPaidSummon();
				_heroUnitSummoner.Summon(data.Species, data.GradeIndex, data.Position);
			}
			else
			{
				throw new Exception($"Define unit for summon in tutorial step: {State.GetType()}.{State}!");
			}
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
