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
		[SerializeField] private List<BeginerTutorialMessage> _beginerTutorialMessages = new List<BeginerTutorialMessage>();
		[SerializeField] private List<BeginerTurorialSummonData> _beginerTurorialSummons = new List<BeginerTurorialSummonData>();

		private Dictionary<BeginnerStep, string> _beginerMessages;
		private Dictionary<BeginnerStep, BeginerTurorialSummonData> _beginerSummons;
		
		public Dictionary<BeginnerStep, string> BeginerTutorialMessages => _beginerMessages;
		public Dictionary<BeginnerStep, BeginerTurorialSummonData> BeginerTurorialSummons => _beginerSummons;

		public void Initialize()
		{
			_beginerMessages = new Dictionary<BeginnerStep, string>();
			foreach (var message in _beginerTutorialMessages)
				_beginerMessages.Add(message.Step, message.TranslationKey);

			_beginerSummons = new Dictionary<BeginnerStep, BeginerTurorialSummonData>();
			foreach (var data in _beginerTurorialSummons)
				_beginerSummons.Add(data.Step, data);
		}

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
			public Vector2Int Position;
		}
	}
}