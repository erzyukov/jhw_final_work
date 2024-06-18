namespace Game.Analytics
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	//using GameAnalyticsSDK;
	using UnityEngine;

	public class ResourcesHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private IGameHero _gameHero;
		[Inject] private IGameEnergy _gameEnergy;
		
		private const string SoftCurrencyEventKey = "SoftCurrency";
		private const string PlayerExperienceEventKey = "Experience";
		private const string EnergyEventKey = "Energy";

		public void Initialize()
		{
			_gameCurrency.SoftCurrencyTransacted
				.Subscribe(OnSoftCurrencyTransacted)
				.AddTo(this);

			_gameHero.ExperienceTransacted
				.Subscribe(OnExperienceTransacted)
				.AddTo(this);

			_gameEnergy.EnergySpent
				.Subscribe(OnEnergySpent)
				.AddTo(this);
		}

		private void OnSoftCurrencyTransacted(CurrencyTransactionData<SoftTransaction> data)
		{
			//GAResourceFlowType flowType = data.Amount > 0 ? GAResourceFlowType.Source : GAResourceFlowType.Sink;
			//SendResourceEvent(flowType, SoftCurrencyEventKey, Mathf.Abs(data.Amount), data.Type.ToString(), data.Detail);
		}

		private void OnExperienceTransacted(ExperienceTransactionData data)
		{
			//SendResourceEvent(GAResourceFlowType.Source, PlayerExperienceEventKey, data.Amount, data.Type.ToString(), data.Detail);
		}

		private void OnEnergySpent(int spentAmount)
		{
			string itemType = "Battle";
			string itemId = "battle_start";
			//SendResourceEvent(GAResourceFlowType.Sink, EnergyEventKey, spentAmount, itemType, itemId);
		}
	}
}
