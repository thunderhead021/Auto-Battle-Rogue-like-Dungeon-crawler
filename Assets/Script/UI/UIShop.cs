using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    public List<UICard> allCards;
    public Text money;
    public UIBench uiBench;

    private EntitiesDatabaseSO cachedDb;
    private int refreshCost = 2;

    private void Start()
    {
        cachedDb = Gamemanager.Instance.entitiesDatabase;
        GenerateCard();
        PlayerData.Instance.OnUpdate += Refresh;
        Refresh();
    }

    public void GenerateCard()
    {
		for (int i = 0; i < allCards.Count; i++)
		{
			if (!allCards[i].gameObject.activeSelf)
				allCards[i].gameObject.SetActive(true);

			allCards[i].Setup(cachedDb.allEntities[Random.Range(0, cachedDb.allEntities.Count)], this);
		}
	}

    public void OnCardClick(UICard card, EntitiesDatabaseSO.EntityData cardData)
    {
        //We should check if we have the money!

        if (PlayerData.Instance.CanAfford(cardData.cost) && uiBench.getSlotsUsed() < 7)
        {
            PlayerData.Instance.SpendMoney(cardData.cost);
            card.gameObject.SetActive(false);
            uiBench.OnPawnBought(cardData);
        }
        else 
        {
            //need a msg to show on screen
            Debug.Log("Can't buy more pawn");
        }
    }

    public void OnRefreshClick()
    {
        //Decrease money 
        if (PlayerData.Instance.CanAfford(refreshCost))
        {
            PlayerData.Instance.SpendMoney(refreshCost);
            GenerateCard();
        }
    }

    void Refresh()
    {
        money.text = PlayerData.Instance.Money.ToString();
    }
}
