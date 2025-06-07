using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CardShow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private Image cardImage;

    private CardScriptableObject cardData;
    private UI_DeckBuilder deckBuilder;

    public static UI_CardShow instance;

    private CardScriptableObject currentCard;

    private void Awake()
    {
        instance = this;
    }

    public void SetupButton(CardScriptableObject cardToShow)
    {
        currentCard = cardToShow;
        nameText.text = cardToShow.cardName;
        manaText.text = cardToShow.manaCost.ToString();
        cardImage.sprite = cardToShow.character;

        cardData = cardToShow;
    }

    public CardScriptableObject GetCard()
    {
        return currentCard;
    }
}
