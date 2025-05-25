using System;
using System.Collections;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    [Header("Mana Setup")]
    public int startingMana = 1;
    public int maxMana = 10;
    public int playerMana;
    public int enemyMana;

    [Header("Battle Setup")]
    public int startingCardsAmount = 5;
    public int playerHealth;
    public int enemyHealth;
    public bool battleEnded;

    [Header("Details")]
    public float resultScreenTimer;
    public enum TurnOrder {playerActive, playerCardAttacks, enemyActive, enemyCardAttacks}
    public MyQueue<TurnOrder> turnQueue = new MyQueue<TurnOrder>();
    public TurnOrder currentPhase;

    public Transform discardPoint;

    private int currentPlayerMaxMana, currentEnemyMaxMana;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetupQueue();
        currentPlayerMaxMana = startingMana;
        currentEnemyMaxMana = 0;
        FillPlayerMana();
        FillEnemyMana();
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
        SetupAggression();
    }

    private void SetupAggression()
    {
        if (enemyHealth < 25 || currentEnemyMaxMana < 5)
            EnemyController.instance.enemyAIType = EnemyController.AIType.handDefensive;
        else if (enemyHealth >= 25 && currentEnemyMaxMana >= 5)
            EnemyController.instance.enemyAIType = EnemyController.AIType.handAttacking;
    }

    public void AdvanceTurn()
    {
        if (battleEnded == false)
        {
            turnQueue.Enqueue(currentPhase);

            currentPhase = turnQueue.Dequeue();

            HandlePhase(currentPhase);

            if (CardPointsController.instance.PlayerHasLloyd())
                CardPointsController.instance.PlayerLloydSkill();
            if (CardPointsController.instance.EnemyHasLloyd())
                CardPointsController.instance.EnemyLloydSkill();
        }
    }

    public void FillPlayerMana()
    {
        playerMana = currentPlayerMaxMana;
        UIController.instance.SetManaText(playerMana);
    }

    public void FillEnemyMana()
    {
        enemyMana = currentEnemyMaxMana;
        UIController.instance.SetEnemyManaText(enemyMana);
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
                if (currentEnemyMaxMana < maxMana)
                    currentEnemyMaxMana++;

                FillEnemyMana();
                EnemyController.instance.StartAction();
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

    public void SpendEnemyMana(int amountToSpend)
    {
        enemyMana -= amountToSpend;

        if (enemyMana < 0)
            enemyMana = 0;

        UIController.instance.SetEnemyManaText(enemyMana);
    }

    public void EndPlayerTurn()
    {
        AdvanceTurn();
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawButton.SetActive(false);
    }

    public void DamagePlayer(int damageAmount)
    {
        if (playerHealth > 0 || battleEnded == false)
        {
            playerHealth -= damageAmount;

            if (playerHealth <= 0)
            {
                EndBattle();
            }
            SetupHealthBars();
            DamageIndicatorPlayer(damageAmount);
        }
    }

    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0 || battleEnded == false)
        {
            enemyHealth -= damageAmount;
            if (enemyHealth <= 0)
            {
                EndBattle();
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

    private void EndBattle()
    {
        battleEnded = true;

        if (enemyHealth <= 0)
            UIController.instance.resultText.text = "YOU WON!";
        else
            UIController.instance.resultText.text = "YOU LOST";

        StartCoroutine(ShowResultCo());
    }

    IEnumerator ShowResultCo()
    {
        yield return new WaitForSeconds(resultScreenTimer);

        UIController.instance.endScreen.SetActive(true);
    }
}
