using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cardPrefab;
    public HorizontalCardHolder cardHolder;

    private readonly List<Card> cardsInHand = new List<Card>();

    public void AddCardToHand(CardData data)
    {
        if (data == null)
            return;

        if (cardPrefab == null || cardHolder == null)
        {
            Debug.LogError("HandManager: cardPrefab or cardHolder is not assigned.");
            return;
        }

        Transform emptySlot = GetEmptySlot();

        if (emptySlot == null)
        {
            Debug.LogWarning("HandManager: No empty hand slot available.");
            return;
        }

        GameObject newCardObject = Instantiate(cardPrefab, emptySlot);

        newCardObject.transform.localPosition = Vector3.zero;
        newCardObject.transform.localRotation = Quaternion.identity;
        newCardObject.transform.localScale = Vector3.one;

        Card card = newCardObject.GetComponent<Card>();

        if (card == null)
        {
            Debug.LogError("HandManager: Card prefab has no Card component.");
            Destroy(newCardObject);
            return;
        }

        card.SetData(data);

        if (!cardsInHand.Contains(card))
            cardsInHand.Add(card);

        RefreshHand();
    }

    public void AddCardToHandObject(Card card)
    {
        if (card == null || cardHolder == null)
            return;

        Transform emptySlot = GetEmptySlot();

        if (emptySlot == null)
        {
            Debug.LogWarning("HandManager: No empty hand slot available.");
            return;
        }

        card.transform.SetParent(emptySlot, false);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = Vector3.one;

        card.Deselect();

        if (!cardsInHand.Contains(card))
            cardsInHand.Add(card);

        RefreshHand();
    }

    public void RemoveCardFromHand(Card card)
    {
        if (card == null)
            return;

        cardsInHand.Remove(card);

        RefreshHand();
    }

    public List<Card> GetCardsInHand()
    {
        return cardsInHand;
    }

    private Transform GetEmptySlot()
    {
        for (int i = 0; i < cardHolder.transform.childCount; i++)
        {
            Transform slot = cardHolder.transform.GetChild(i);

            if (slot.childCount == 0)
                return slot;
        }

        return null;
    }

    private void RefreshHand()
    {
        cardsInHand.RemoveAll(card => card == null);

        if (cardHolder != null)
            cardHolder.RefreshCardList();
    }
}