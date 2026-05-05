using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    public HandManager handManager;
    public Transform playArea;

    [Header("Play Area Settings")]
    public float cardSpacing = 130f;
    public float returnDelay = 1.5f;

    private readonly List<Card> playedCards = new List<Card>();

    public void PlayCard(Card card)
    {
        if (card == null)
            return;

        if (playedCards.Count >= 5)
        {
            Debug.Log("You can only play 5 cards max.");
            return;
        }

        playedCards.Add(card);

        if (handManager != null)
            handManager.RemoveCardFromHand(card);

        card.transform.SetParent(playArea, false);

        SpreadCardsHorizontally();
    }

    public void EndTurn()
    {
        StartCoroutine(ReturnPlayedCardsToHand());
    }

    private void SpreadCardsHorizontally()
    {
        int count = playedCards.Count;

        for (int i = 0; i < count; i++)
        {
            Card card = playedCards[i];

            if (card == null)
                continue;

            float xPosition = (i - (count - 1) / 2f) * cardSpacing;

            card.transform.localPosition = new Vector3(xPosition, 0f, 0f);
            card.transform.localRotation = Quaternion.identity;
            card.transform.localScale = Vector3.one;
        }
    }

   private IEnumerator ReturnPlayedCardsToHand()
{
    yield return new WaitForSeconds(returnDelay);

    foreach (Card card in playedCards)
    {
        if (card == null)
            continue;

        if (handManager != null)
        {
            handManager.AddCardToHandObject(card);
        }
    }

    playedCards.Clear();
}
}