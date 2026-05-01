using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cardPrefab;
    public HorizontalCardHolder cardHolder;

    private List<Card> cardsInHand = new List<Card>();

    public void AddCardToHand(CardData data)
    {
        if (cardHolder == null)
        {
            Debug.LogError("HandManager: cardHolder is not assigned.");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("HandManager: cardPrefab is not assigned.");
            return;
        }

        Transform emptySlot = GetEmptySlot();

        if (emptySlot == null)
        {
            Debug.LogWarning("HandManager: No empty slot available in HorizontalCardHolder.");
            return;
        }

        GameObject newCardObject = Instantiate(cardPrefab, emptySlot);
        newCardObject.transform.localPosition = Vector3.zero;
        newCardObject.transform.localRotation = Quaternion.identity;
        newCardObject.transform.localScale = Vector3.one;

        Card card = newCardObject.GetComponent<Card>();

        if (card == null)
        {
            Debug.LogError("HandManager: Card prefab does not contain a Card component.");
            Destroy(newCardObject);
            return;
        }

        card.SetData(data);
        cardsInHand.Add(card);

        cardHolder.RefreshCardList();
    }

    private Transform GetEmptySlot()
    {
        for (int i = 0; i < cardHolder.transform.childCount; i++)
        {
            Transform slot = cardHolder.transform.GetChild(i);

            if (slot.GetComponent<Card>() == null && slot.childCount == 0)
            {
                return slot;
            }
        }

        return null;
    }

    public void RemoveCardFromHand(Card card)
    {
        if (cardsInHand.Contains(card))
        {
            cardsInHand.Remove(card);
        }

        if (cardHolder != null)
        {
            cardHolder.RefreshCardList();
        }
    }

    public List<Card> GetCardsInHand()
    {
        return cardsInHand;
    }
}