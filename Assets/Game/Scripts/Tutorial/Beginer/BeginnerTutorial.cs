namespace Game.Tutorial
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System;
	using Game.Core;
	using UnityEngine;
	using Game.Ui;
	using Game.Field;
	using Game.Configs;

	public class BeginnerTutorial: BeginerTutorialFsmBase, IInitializable, IDisposable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameLevel _level;
		[Inject] private IGameCycle _cycle;
		[Inject] private IUiTacticalStageHud _uiTacticalStageHud;
		[Inject] private IFingerHint _fingerHint;
		[Inject] private IDialogHint _dialogHint;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private TutorialConfig _config;
		[Inject] private ILocalizator _localizator;

		readonly private CompositeDisposable _disposable = new CompositeDisposable();

		public void Initialize()
		{
			_profile.Tutorial.BeginerStep
				.Where(step => step != State)
				.Subscribe(Transition)
				.AddTo(_disposable);
		}

		public virtual void Dispose() => _disposable.Dispose();

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

		private void SetupSummonStep()
		{
			_fingerHint.SetPosition(_uiTacticalStageHud.SummonButtonHintParameters.Point.position);
			_fingerHint.SetActive(true);

			if (_config.BeginerTutorialMessages.TryGetValue(State, out string key))
			{
				_dialogHint.SetMessage(_localizator.GetString(key));
				_dialogHint.SetActive(true);
			}

			_fieldHeroFacade.SetDraggableActive(false);
			_uiTacticalStageHud.SetStartBattleButtonInteractable(false);
		}

	}
}
