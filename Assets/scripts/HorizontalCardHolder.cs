using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalCardHolder : MonoBehaviour
{
    [Header("Action Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button discardButton;

    [SerializeField] private Card selectedCard;
    [SerializeField] private Card hoveredCard;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int cardsToSpawn = 5;
    [SerializeField] private bool tweenCardReturn = true;

    private RectTransform rect;
    public List<Card> cards = new List<Card>();
    private bool isCrossing = false;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Create empty slots only if none exist yet
        if (transform.childCount == 0)
        {
            for (int i = 0; i < cardsToSpawn; i++)
            {
                Instantiate(slotPrefab, transform);
            }
        }

        RefreshCardList();
        HookupButtons();
        UpdateActionButtons();

        StartCoroutine(UpdateVisualIndexesNextFrame());
    }

    private void HookupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(PlaySelectedCards);
            playButton.onClick.AddListener(PlaySelectedCards);
        }

        if (discardButton != null)
        {
            discardButton.onClick.RemoveListener(DiscardSelectedCards);
            discardButton.onClick.AddListener(DiscardSelectedCards);
        }
    }

    private IEnumerator UpdateVisualIndexesNextFrame()
    {
        yield return null;

        foreach (Card card in cards)
        {
            if (card != null && card.cardVisual != null)
            {
                card.cardVisual.UpdateIndex(transform.childCount);
            }
        }
    }

    public void RefreshCardList()
    {
        cards = GetComponentsInChildren<Card>().ToList();

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];

            // remove first so we don't stack duplicate listeners
            card.PointerEnterEvent.RemoveListener(CardPointerEnter);
            card.PointerExitEvent.RemoveListener(CardPointerExit);
            card.BeginDragEvent.RemoveListener(BeginDrag);
            card.EndDragEvent.RemoveListener(EndDrag);
            card.SelectEvent.RemoveListener(OnCardSelectionChanged);

            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.SelectEvent.AddListener(OnCardSelectionChanged);

            card.name = i.ToString();
        }

        UpdateActionButtons();
    }

    private void OnCardSelectionChanged(Card card, bool isSelected)
    {
        UpdateActionButtons();
    }

    private void UpdateActionButtons()
    {
        bool hasSelectedCards = cards.Any(card => card != null && card.selected);

        if (playButton != null)
            playButton.interactable = hasSelectedCards;

        if (discardButton != null)
            discardButton.interactable = hasSelectedCards;
    }

    private List<Card> GetSelectedCards()
    {
        return cards.Where(card => card != null && card.selected).ToList();
    }

    public void PlaySelectedCards()
    {
        List<Card> selectedCards = GetSelectedCards();

        if (selectedCards.Count == 0)
            return;

        foreach (Card card in selectedCards)
        {
            if (card == null)
                continue;

            cards.Remove(card);

            if (card.transform.parent != null)
                Destroy(card.transform.parent.gameObject);
            else
                Destroy(card.gameObject);
        }

        UpdateActionButtons();
    }

    public void DiscardSelectedCards()
    {
        List<Card> selectedCards = GetSelectedCards();

        if (selectedCards.Count == 0)
            return;

        foreach (Card card in selectedCards)
        {
            if (card == null)
                continue;

            cards.Remove(card);

            if (card.transform.parent != null)
                Destroy(card.transform.parent.gameObject);
            else
                Destroy(card.gameObject);
        }

        UpdateActionButtons();
    }

    private void BeginDrag(Card card)
    {
        selectedCard = card;
    }

    private void EndDrag(Card card)
    {
        if (selectedCard == null)
            return;

        selectedCard.transform.DOLocalMove(
            selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero,
            tweenCardReturn ? 0.15f : 0f
        ).SetEase(Ease.OutBack);

        // force layout refresh
        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;
    }

    private void CardPointerEnter(Card card)
    {
        hoveredCard = card;
    }

    private void CardPointerExit(Card card)
    {
        if (hoveredCard == card)
            hoveredCard = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (hoveredCard != null)
            {
                cards.Remove(hoveredCard);

                if (hoveredCard.transform.parent != null)
                    Destroy(hoveredCard.transform.parent.gameObject);
                else
                    Destroy(hoveredCard.gameObject);

                hoveredCard = null;
                UpdateActionButtons();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in cards)
            {
                if (card != null)
                    card.Deselect();
            }

            UpdateActionButtons();
        }

        if (selectedCard == null || isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null || cards[i] == selectedCard)
                continue;

            if (selectedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (selectedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    private void Swap(int index)
    {
        if (selectedCard == null || index < 0 || index >= cards.Count || cards[index] == null)
            return;

        isCrossing = true;

        Transform focusedParent = selectedCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition =
            cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;

        selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cards[index].cardVisual != null)
        {
            bool swapIsRight = cards[index].ParentIndex() > selectedCard.ParentIndex();
            cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);
        }

        foreach (Card card in cards)
        {
            if (card != null && card.cardVisual != null)
                card.cardVisual.UpdateIndex(transform.childCount);
        }
    }
}