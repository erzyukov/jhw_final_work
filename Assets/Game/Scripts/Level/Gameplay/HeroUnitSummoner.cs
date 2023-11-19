namespace Game.Level
{
	using Game.Configs;
	using Game.Core;
	using Game.Field;
	using Game.Ui;
	using Game.Units;
	using Game.Utilities;
	using Zenject;

	public interface IHeroUnitSummoner
	{
		void Summon();
	}

	public class HeroUnitSummoner : ControllerBase, IHeroUnitSummoner, IInitializable
	{
		[Inject] private CurrencyConfig _currencyConfig;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private IUiHaveNeedOfMessage _haveNeedOfMessage;
		[Inject] private IUnitSpawner _unitSpawner;
		[Inject] private IFieldHeroFacade _fieldFacade;

		public void Initialize()
		{
			
		}

		public void Summon()
		{
			if (_fieldFacade.HasFreeSpace == false)
			{
				// TODO: show message: out of free space
				return;
			}

			int summonPrice = _currencyConfig.UnitSummonPrice;
			if (_gameCurrency.TrySpendSummonCurrency(summonPrice) == false)
			{
				_haveNeedOfMessage.ShowMessage(NeedMessage.SummonCurrency);
				return;
			}

			Species defaultSpecies = Species.HeroInfantryman;
			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(defaultSpecies);
			_fieldFacade.AddUnit(unit);
		}
	}
}
