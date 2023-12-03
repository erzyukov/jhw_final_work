namespace Game.Configs
{
	using Game.Tutorial;
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Tutorial", menuName = "Configs/Tutorial", order = (int)Config.Tutorial)]
	public class TutorialConfig : ScriptableObject
	{
		[SerializeField] private BeginerTutorialMessage[] _beginerTutorialMessages;

		private Dictionary<BeginnerStep, string> _beginerMessages;
		
		public Dictionary<BeginnerStep, string> BeginerTutorialMessages => _beginerMessages;

		public void Initialize()
		{
			_beginerMessages = new Dictionary<BeginnerStep, string>();
			foreach (var message in _beginerTutorialMessages)
				_beginerMessages.Add(message.Step, message.TranslationKey);
		}

		[Serializable]
		public struct BeginerTutorialMessage
		{
			public BeginnerStep Step;
			public string TranslationKey;
		}
	}
}