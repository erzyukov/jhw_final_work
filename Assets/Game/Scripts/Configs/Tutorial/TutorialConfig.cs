namespace Game.Configs
{
	using Game.Tutorial;
	using Game.Units;
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Tutorial", menuName = "Configs/Tutorial", order = (int)Config.Tutorial)]
	public class TutorialConfig : ScriptableObject
	{
        // TODO: refact: BeginerTutorialMessage >> TutorialMessage<BeginnerStep>
        [Header("Beginer Tutorial")]
		[SerializeField] private List<BeginerTutorialMessage>		_beginerTutorialMessages = new();
		[SerializeField] private List<BeginerTurorialSummonData>	_beginerTurorialSummons = new();
		[SerializeField] private List<BeginerTurorialMergeData>		_beginerTurorialMerges = new();

        [Header("Upgrades Tutorial")]
        [SerializeField] private List<TutorialMessage<UpgradesStep>>	_upgradesTutorialMessages = new();

        [Header("Unlock Unit Tutorial")]
        [SerializeField] private List<TutorialMessage<UnlockUnitStep>>	_unlockUnitTutorialMessages = new();

        private Dictionary<BeginnerStep, string> _beginerMessages;
		private Dictionary<BeginnerStep, BeginerTurorialSummonData> _beginerSummons;
		private Dictionary<BeginnerStep, BeginerTurorialMergeData> _beginerMerges;
        private Dictionary<UpgradesStep, TutorialMessage<UpgradesStep>> _upgradesMessages;
        private Dictionary<UnlockUnitStep, TutorialMessage<UnlockUnitStep>> _unlockUnitMessages;
		
		public Dictionary<BeginnerStep, string> BeginerTutorialMessages 
			=> _beginerMessages;
		public Dictionary<BeginnerStep, BeginerTurorialSummonData> BeginerTurorialSummons
			=> _beginerSummons;
		public Dictionary<BeginnerStep, BeginerTurorialMergeData> BeginerTurorialMerges
			=> _beginerMerges;
        public Dictionary<UpgradesStep, TutorialMessage<UpgradesStep>> UpgradesTutorialMessages
			=> _upgradesMessages;
		public Dictionary<UnlockUnitStep, TutorialMessage<UnlockUnitStep>> UnlockStepTutorialMessages
			=> _unlockUnitMessages;


        public void Initialize()
		{
			_beginerMessages = new Dictionary<BeginnerStep, string>();
			foreach (var message in _beginerTutorialMessages)
				_beginerMessages.Add(message.Step, message.TranslationKey);

			_beginerSummons = new Dictionary<BeginnerStep, BeginerTurorialSummonData>();
			foreach (var data in _beginerTurorialSummons)
				_beginerSummons.Add(data.Step, data);

			_beginerMerges = new Dictionary<BeginnerStep, BeginerTurorialMergeData>();
			foreach (var data in _beginerTurorialMerges)
				_beginerMerges.Add(data.Step, data);

            _upgradesMessages = new Dictionary<UpgradesStep, TutorialMessage<UpgradesStep>>();
            foreach (var message in _upgradesTutorialMessages)
                _upgradesMessages.Add(message.Step, message);

            _unlockUnitMessages = new Dictionary<UnlockUnitStep, TutorialMessage<UnlockUnitStep>>();
            foreach (var message in _unlockUnitTutorialMessages)
                _unlockUnitMessages.Add(message.Step, message);
        }

		// TODO: switch to TutorialMessage<T>
		[Serializable]
		public struct BeginerTutorialMessage
		{
			public BeginnerStep Step;
			public string TranslationKey;
		}

		[Serializable]
		public struct BeginerTurorialSummonData
		{
			public BeginnerStep Step;
			public Species Species;
			public int GradeIndex;
			public int Power;
			public Vector2Int Position;
		}

		[Serializable]
		public struct BeginerTurorialMergeData
		{
			public BeginnerStep Step;
			public Vector2Int FromPosition;
			public Vector2Int ToPosition;
		}

        [Serializable]
        public struct TutorialMessage<T> where T : Enum
        {
            public T Step;
            public string TranslationKey;
			public DialogHintPlace Place;
		}
    }
}