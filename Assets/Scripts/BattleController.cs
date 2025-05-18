using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    public int startingMana = 1;
    public int maxMana = 10;
    public int playerMana;
    public int startingCardsAmount = 5;

    private int currentPlayerMaxMana;

    public enum TurnOrder {playerActive, playerCardAttacks, enemyActive, enemyCardAttacks}
    public MyQueue<TurnOrder> turnQueue = new MyQueue<TurnOrder>();
    public TurnOrder currentPhase;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetupQueue();
        currentPlayerMaxMana = startingMana;
        FillPlayerMana();

        DeckController.Instance.DrawMultipleCards(startingCardsAmount);
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
                AdvanceTurn();
                break;

            case TurnOrder.enemyActive:
                Debug.Log("Skipping enemy active");
                AdvanceTurn();
                break;

            case TurnOrder.enemyCardAttacks:
                Debug.Log("Skipping enemy attack");
                AdvanceTurn();
                ResetTurnQueue();
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
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawButton.SetActive(false);
        AdvanceTurn();
    }
}
