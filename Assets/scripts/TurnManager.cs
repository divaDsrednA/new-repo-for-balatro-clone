using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public HandManager handManager;

    private List<CardData> playedCards = new List<CardData>();

    public void PlayCard(Card card)
    {
        if (card == null || card.data == null)
            return;

        playedCards.Add(card.data);

        handManager.RemoveCardFromHand(card);

        Destroy(card.gameObject);
    }

    public void EndTurn()
    {
        foreach (CardData data in playedCards)
        {
            handManager.AddCardToHand(data);
        }

        playedCards.Clear();
    }
}