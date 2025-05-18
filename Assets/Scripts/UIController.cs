using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] private TMP_Text playerManaText;
    [SerializeField] private GameObject manaWarning;
    [SerializeField] private float manaWarnTimer = 1.5f;
    public GameObject drawButton, endTurnButton;

    private float counterManaWarning;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        ManaWarningTimer();
    }

    private void ManaWarningTimer()
    {
        if (counterManaWarning > 0)
        {
            counterManaWarning -= Time.deltaTime;

            if (counterManaWarning <= 0)
                manaWarning.SetActive(false);
        }
    }

    public void SetManaText(int manaAmount)
    {
        playerManaText.text = "Mana: " + manaAmount;
    }

    public void DrawCardButton()
    {
        DeckController.Instance.DrawCardForMana(2);
    }

    public void ShowManaWarning()
    {
        manaWarning.SetActive(true);
        counterManaWarning = manaWarnTimer;
    }

    public void EndTurnButton()
    {
        BattleController.instance.EndPlayerTurn();
    }
}
