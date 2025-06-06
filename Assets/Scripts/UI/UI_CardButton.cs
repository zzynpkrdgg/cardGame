using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CardButton : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private Image cardImage;

    private int cardID;
    private CardScriptableObject cardData;
    private UI_DeckBuilder deckBuilder;
    private bool isSelected = false;

    public void SetupButton(CardScriptableObject cardToShow, int ID, UI_DeckBuilder builder)
    {
        nameText.text = cardToShow.cardName;
        manaText.text = cardToShow.manaCost.ToString();
        cardImage.sprite = cardToShow.character;

        cardData = cardToShow;
        cardID = ID;
        deckBuilder = builder;
    }

    public void OnClick()
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            deckBuilder.AddCardToDeck(cardData);
        }
        else
        {
            deckBuilder.RemoveCardFromDeck(cardData);
        }
    }
}