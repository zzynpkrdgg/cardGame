using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class UI_DeckBuilder : MonoBehaviour
{
    [SerializeField] private List<CardScriptableObject> playableCards = new List<CardScriptableObject>();
    [SerializeField] private UI_CardButton buttonPrefab;
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private TMP_Text deckCountText;
    private List<UI_CardButton> buttonPool = new List<UI_CardButton>();
    private bool listedByMana = true;

    public List<CardScriptableObject> selectedCards = new List<CardScriptableObject>();
    public static UI_DeckBuilder instance;
    public int maxCardOnDeck = 25;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SortCardsByManaAndName();
        CreateButtons();
    }
    private void HandleCountText()
    {
        if (selectedCards.Count < 10)
            deckCountText.text = "Deck: 0" + selectedCards.Count + "/" + maxCardOnDeck;
        else
            deckCountText.text = "Deck: " + selectedCards.Count + "/" + maxCardOnDeck;
    }

    private void CreateButtons()
    {
        if (buttonPool.Count == 0)
        {
            for (int i = 0; i < playableCards.Count; i++)
            {
                UI_CardButton newButton = Instantiate(buttonPrefab, buttonsParent);
                buttonPool.Add(newButton);
            }
        }

        for (int i = 0; i < playableCards.Count; i++)
        {
            buttonPool[i].SetupButton(playableCards[i], i, this);
        }
    }

    public void HandleSorting()
    {
        listedByMana = !listedByMana;

        if (listedByMana)
            SortCardsByManaAndName();
        else
            SortCardsByName();

        CreateButtons();
    }

    private void SortCardsByManaAndName()
    {
        playableCards = playableCards.OrderBy(card => card.manaCost)
                           .ThenBy(card => card.cardName)
                           .ToList();
    }
    private void SortCardsByName()
    {
        playableCards = playableCards.OrderBy(card => card.cardName)
                           .ToList();
    }

    public void AddCardToDeck(CardScriptableObject card)
    {
        if (!selectedCards.Contains(card) && selectedCards.Count < maxCardOnDeck)
        {
            selectedCards.Add(card);
            HandleCountText();
        }
    }

    public void RemoveCardFromDeck(CardScriptableObject card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            HandleCountText();
        }
    }
}