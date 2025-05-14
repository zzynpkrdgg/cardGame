using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] private TMP_Text playerManaText;

    private void Awake()
    {
        instance = this;
    }

    public void SetManaText(int manaAmount)
    {
        playerManaText.text = "Mana: " + manaAmount;
    }

}
