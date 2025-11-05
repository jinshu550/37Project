using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetUpSystem : Singleton<MatchSetUpSystem>
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private List<PerkData> perkData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {
        List<CardData> dataList = new List<CardData>();
        BackpackDataService dataService = new BackpackDataService();
        List<CardBasicInformation> data = dataService.GetCategoryData(BackpackUIManager.CategoryType.BackFunction);
        List<string> BattleCards = GameDataManager.Instance.GetBattleCardINames();
        for (int i = 0; i < BattleCards.Count; i++)
        {
            CardBasicInformation temp = data.Find(card => card.cardName == BattleCards[i]);
            if (temp != null)
            {
                dataList.Add(temp.card);
            }
        }

        heroData.FunctionDeck = dataList;
        HeroSystem.Instance.SetUp(heroData);
        CardSystem.Instance.SetUp(heroData.Deck, heroData.FunctionDeck);
        EnemySystem.Instance.SetUp(StaticUtility.CurrentEnemyData);
        foreach (var item in perkData)
        {
            PerkSystem.Instance.AddPerk(new Perk(item));
        }
        DrawCardsGA drawCardsGA = new(3);
        ActionSystem.Instance.Perform(drawCardsGA);
        SoundSystem.Instance.SetUp();
    }

}
