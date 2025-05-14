using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int currentHealth;
    public int AttackPower;
    public int manaCost;

    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text manaText;

    private void Start()
    {
        healthText.text = currentHealth.ToString();
        attackText.text = AttackPower.ToString();
        manaText.text = manaCost.ToString();

    }


}
