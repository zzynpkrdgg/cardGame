using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public int currentHealth;
    public int AttackPower;
    public int manaCost;

    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text manaText;
    public TMP_Text nameText;
    public TMP_Text actionDescriptionText;
    public TMP_Text loreText;

    public Image characterArt;


    private void Start()
    {
        SetupCard();
    }

    private void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        AttackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;


        healthText.text = currentHealth.ToString();
        attackText.text = AttackPower.ToString();
        manaText.text = manaCost.ToString();

        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        characterArt.sprite = cardSO.character;
    }
}
