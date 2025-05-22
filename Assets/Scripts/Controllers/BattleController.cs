using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    public int startingMana = 1;
    public int maxMana = 10;
    public int playerMana;
    public int startingCardsAmount = 5;
    public int playerHealth;
    public int enemyHealth;

    private int currentPlayerMaxMana;

    public enum TurnOrder {playerActive, playerCardAttacks, enemyActive, enemyCardAttacks}
    public MyQueue<TurnOrder> turnQueue = new MyQueue<TurnOrder>();
    public TurnOrder currentPhase;

    public Transform discardPoint;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetupQueue();
        currentPlayerMaxMana = startingMana;
        FillPlayerMana();
        SetupHealthBars();

        DeckController.Instance.DrawMultipleCards(startingCardsAmount);
    }

    private static void SetupHealthBars()
    {
        UIController.instance.UpdatePlayerHealth();
        UIController.instance.UpdateEnemyHealth();
    }

    private void SetupQueue()
    {
        turnQueue.Enqueue(TurnOrder.playerActive);
        turnQueue.Enqueue(TurnOrder.playerCardAttacks);
        turnQueue.Enqueue(TurnOrder.enemyActive);
        turnQueue.Enqueue(TurnOrder.enemyCardAttacks);

        currentPhase = turnQueue.Dequeue();
        HandlePhase(currentPhase);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            AdvanceTurn();
    }

    public void AdvanceTurn()
    {
        turnQueue.Enqueue(currentPhase);

        currentPhase = turnQueue.Dequeue();

        HandlePhase(currentPhase);
    }

    public void FillPlayerMana()
    {
        playerMana = currentPlayerMaxMana;
        UIController.instance.SetManaText(playerMana);
    }

    private void HandlePhase(TurnOrder phase)
    {
        switch (phase)
        {
            case TurnOrder.playerActive:
                UIController.instance.endTurnButton.SetActive(true);
                UIController.instance.drawButton.SetActive(true);

                if (currentPlayerMaxMana < maxMana)
                    currentPlayerMaxMana++;

                FillPlayerMana();
                DeckController.Instance.DrawCardToHand();
                break;

            case TurnOrder.playerCardAttacks:

                CardPointsController.instance.PlayerAttack();
                break;

            case TurnOrder.enemyActive:
                Debug.Log("Skipping enemy active");
                AdvanceTurn();
                break;

            case TurnOrder.enemyCardAttacks:
                ResetTurnQueue();
                CardPointsController.instance.EnemyAttack();
                break;
        }
    }
    private void ResetTurnQueue()
    {
        turnQueue = new MyQueue<TurnOrder>();
        turnQueue.Enqueue(TurnOrder.playerActive);
        turnQueue.Enqueue(TurnOrder.playerCardAttacks);
        turnQueue.Enqueue(TurnOrder.enemyActive);
        turnQueue.Enqueue(TurnOrder.enemyCardAttacks);
    }

    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana -= amountToSpend;

        if (playerMana < 0)
            playerMana = 0;

        UIController.instance.SetManaText(playerMana);
    }

    public void EndPlayerTurn()
    {
        AdvanceTurn();
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawButton.SetActive(false);
    }

    public void DamagePlayer(int damageAmount)
    {
        if (playerHealth > 0)
        {
            playerHealth -= damageAmount;

            if (playerHealth <= 0)
            {
                //End Battle
            }
            SetupHealthBars();
            DamageIndicatorPlayer(damageAmount);
        }
    }

    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0)
        {
            enemyHealth -= damageAmount;
            if (enemyHealth <= 0)
            {
                //end battle
            }
            SetupHealthBars();
            DamageIndicatorEnemy(damageAmount);
        }
    }

    private static void DamageIndicatorEnemy(int damageAmount)
    {
        UIDamageIndicator damageClone = Instantiate(UIController.instance.enemyDamage, UIController.instance.enemyDamage.transform.parent);
        damageClone.damageText.text = damageAmount.ToString();
        damageClone.gameObject.SetActive(true);
    }

    private static void DamageIndicatorPlayer(int damageAmount)
    {
        UIDamageIndicator damageClone = Instantiate(UIController.instance.playerDamage, UIController.instance.playerDamage.transform.parent);
        damageClone.damageText.text = damageAmount.ToString();
        damageClone.gameObject.SetActive(true);
    }
}
