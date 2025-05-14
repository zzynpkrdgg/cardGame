using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    public int startingMana = 1;
    public int maxMana = 10;
    public int playerMana;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerMana = startingMana;
        UIController.instance.SetManaText(playerMana);
    }

    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana -= amountToSpend;

        if (playerMana < 0)
            playerMana = 0;

        UIController.instance.SetManaText(playerMana);
    }
}
