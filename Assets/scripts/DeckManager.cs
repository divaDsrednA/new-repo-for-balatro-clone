using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public HandManager handManager;

    private List<CardData> deck = new List<CardData>();

    void Start()
    {
        CreateDeck();
        ShuffleDeck();
        DrawStartingHand(5);
    }

    void CreateDeck()
    {
        Suit[] suits = { Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades };

        foreach (Suit suit in suits)
        {
            for (int rank = 1; rank <= 13; rank++)
            {
                deck.Add(new CardData(rank, suit));
            }
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);

            CardData temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DrawStartingHand(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
        }
    }

    void DrawCard()
    {
        if (deck.Count == 0) return;

        CardData drawnCard = deck[0];
        deck.RemoveAt(0);

        handManager.AddCardToHand(drawnCard);
    }
}

