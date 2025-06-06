using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{
    public static CardPointsController instance;
    public CardPlacePoint[] playerCardPoints, enemyCardPoints;
    public float waitBetweenAttacks = .3f;

    private Card card;

    private void Awake()
    {
        instance = this;
        card = GetComponent<Card>();
    }

    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCo());
    }

    IEnumerator PlayerAttackCo()
    {
        yield return new WaitForSeconds(waitBetweenAttacks);

        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            var attacker = playerCardPoints[i].activeCard;

            if (attacker != null)
            {
                if (attacker.canOverwhelm)
                {
                    int tempHealth;
                    int overwhelmDamage;

                    if (enemyCardPoints[i].activeCard != null)
                    {
                        tempHealth = enemyCardPoints[i].activeCard.currentHealth;
                        overwhelmDamage = attacker.cardSO.attackPower;
                        overwhelmDamage -= tempHealth;
                        BattleController.instance.DamageEnemy(overwhelmDamage);
                    }
                }

                if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.attackAllEnemies)
                {
                    if (i < enemyCardPoints.Length && enemyCardPoints[i].activeCard != null)
                    {
                        for (int j = 0; j < enemyCardPoints.Length; j++)
                        {
                            if (enemyCardPoints[j].activeCard != null)
                            {
                                enemyCardPoints[j].activeCard.DamageCard(attacker.attackPower);
                            }
                        }
                    }
                    else
                    {
                        BattleController.instance.DamageEnemy(attacker.attackPower);
                        for (int j = 0; j < enemyCardPoints.Length; j++)
                        {
                            if (enemyCardPoints[j].activeCard != null)
                            {
                                enemyCardPoints[j].activeCard.DamageCard(attacker.attackPower);
                            }
                        }
                    }
                }

                else if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.babyDucks)
                {
                    if (i < enemyCardPoints.Length && enemyCardPoints[i].activeCard != null)
                    {
                        for (int j = 0; j < enemyCardPoints.Length; j++)
                        {
                            if (enemyCardPoints[j].activeCard != null)
                            {
                                enemyCardPoints[j].activeCard.DamageCard(attacker.cardSO.buffValue);
                            }
                        }
                        BattleController.instance.DamageEnemy(attacker.cardSO.buffValue);
                    }
                    else
                    {
                        for (int j = 0; j < enemyCardPoints.Length; j++)
                        {
                            if (enemyCardPoints[j].activeCard != null)
                            {
                                enemyCardPoints[j].activeCard.DamageCard(attacker.cardSO.buffValue);
                            }
                        }
                        BattleController.instance.DamageEnemy(attacker.cardSO.buffValue);
                        BattleController.instance.DamageEnemy(attacker.attackPower);
                    }
                }
                else
                {
                    if (i < enemyCardPoints.Length && enemyCardPoints[i].activeCard != null)
                    {
                        enemyCardPoints[i].activeCard.DamageCard(attacker.attackPower);
                        LifeStealPlayer(attacker);
                    }
                    else
                    {
                        BattleController.instance.DamageEnemy(attacker.attackPower);
                        LifeStealPlayer(attacker);
                    }
                }


                if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.drawCardOnAttack)
                    DeckController.Instance.DrawCardToHand();

                if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.omniman)
                    BattleController.instance.DamageEnemy(attacker.cardSO.buffValue);

                if (attacker.anim != null)
                    attacker.anim.SetTrigger("attack");

                yield return new WaitForSeconds(waitBetweenAttacks);
            }
        }

        CheckAssignedCards();
        BattleController.instance.AdvanceTurn();
    }

    private static void LifeStealPlayer(Card attacker)
    {
        if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.lifeSteal)
        {
            BattleController.instance.playerHealth += attacker.attackPower;
            if (BattleController.instance.playerHealth > 50)
                BattleController.instance.playerHealth = 50;
            UIController.instance.UpdatePlayerHealth();
        }
    }

    public void EnemyAttack()
    {
        StartCoroutine(EnemyAttackCo());
    }

    IEnumerator EnemyAttackCo()
    {
        yield return new WaitForSeconds(waitBetweenAttacks);

        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            var attacker = enemyCardPoints[i].activeCard;

            if (attacker != null)
            {
                if (attacker.canOverwhelm)
                {
                    int tempHealth;
                    int overwhelmDamage;

                    if (playerCardPoints[i].activeCard != null)
                    {
                        tempHealth = playerCardPoints[i].activeCard.currentHealth;
                        overwhelmDamage = attacker.cardSO.attackPower;
                        overwhelmDamage -= tempHealth;
                        BattleController.instance.DamagePlayer(overwhelmDamage);
                    }
                }

                if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.attackAllEnemies)
                {
                    if (i < playerCardPoints.Length && playerCardPoints[i].activeCard != null)
                    {
                        for (int j = 0; j < playerCardPoints.Length; j++)
                        {
                            if (playerCardPoints[j].activeCard != null)
                            {
                                playerCardPoints[j].activeCard.DamageCard(attacker.attackPower);
                            }
                        }
                    }
                    else
                    {
                        BattleController.instance.DamagePlayer(attacker.attackPower);
                        for (int j = 0; j < playerCardPoints.Length; j++)
                        {
                            if (playerCardPoints[j].activeCard != null)
                            {
                                playerCardPoints[j].activeCard.DamageCard(attacker.attackPower);
                            }
                        }
                    }
                }
                else if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.babyDucks)
                {
                    if (i < playerCardPoints.Length && playerCardPoints[i].activeCard != null)
                    {
                        for (int j = 0; j < playerCardPoints.Length; j++)
                        {
                            if (playerCardPoints[j].activeCard != null)
                            {
                                playerCardPoints[j].activeCard.DamageCard(attacker.cardSO.buffValue);
                            }
                        }
                        BattleController.instance.DamagePlayer(attacker.cardSO.buffValue);
                    }
                    else
                    {
                        for (int j = 0; j < playerCardPoints.Length; j++)
                        {
                            if (playerCardPoints[j].activeCard != null)
                            {
                                playerCardPoints[j].activeCard.DamageCard(attacker.cardSO.buffValue);
                            }
                        }
                        BattleController.instance.DamagePlayer(attacker.cardSO.buffValue);
                        BattleController.instance.DamagePlayer(attacker.attackPower);
                    }
                }
                else
                {
                    if (i < playerCardPoints.Length && playerCardPoints[i].activeCard != null)
                    {
                        playerCardPoints[i].activeCard.DamageCard(attacker.attackPower);
                        LifeStealEnemy(attacker);
                    }
                    else
                    {
                        BattleController.instance.DamagePlayer(attacker.attackPower);
                        LifeStealEnemy(attacker);
                    }
                }

                if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.drawCardOnAttack)
                    EnemyController.instance.EnemyDrawCard();

                if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.omniman)
                    BattleController.instance.DamagePlayer(attacker.cardSO.buffValue);

                if (attacker.anim != null)
                    attacker.anim.SetTrigger("attack");

                yield return new WaitForSeconds(waitBetweenAttacks);
            }
        }

        CheckAssignedCards();
        BattleController.instance.AdvanceTurn();
    }

    private static void LifeStealEnemy(Card attacker)
    {
        if (attacker.cardSO.cardsSkill == CardScriptableObject.cardSkills.lifeSteal)
        {
            BattleController.instance.enemyHealth += attacker.attackPower;
            if (BattleController.instance.enemyHealth > 50)
                BattleController.instance.enemyHealth = 50;
            UIController.instance.UpdateEnemyHealth();
        }
    }

    public void CheckAssignedCards()
    {
        foreach (CardPlacePoint point in enemyCardPoints)
        {
            if (point.activeCard != null)
            {
                if (point.activeCard.currentHealth <= 0)
                    point.activeCard = null;
            }
        }

        foreach (CardPlacePoint point in playerCardPoints)
        {
            if (point.activeCard != null)
            {
                if (point.activeCard.currentHealth <= 0)
                    point.activeCard = null;
            }
        }
    }

    public void PlayerKaiFlameEveryone(int damageAmount)
    {
        for (int j = 0; j < enemyCardPoints.Length; j++)
        {
            if (enemyCardPoints[j].activeCard != null)
                enemyCardPoints[j].activeCard.DamageCard(damageAmount);
        }
    }

    public void EnemyKaiFlameEveryone(int damageAmount)
    {
        for (int j = 0; j < playerCardPoints.Length; j++)
        {
            if (playerCardPoints[j].activeCard != null)
            {
                playerCardPoints[j].activeCard.DamageCard(damageAmount);
            }
        }
    }

    public void PlayerLloydSkill()
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null)
            {
                if (playerCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.lloyd)
                {
                    playerCardPoints[i].activeCard.attackPower += BattleController.instance.playerMana;
                    playerCardPoints[i].activeCard.anim.SetTrigger("jump");
                }
            }
        }
        if (card != null)
        {
            card.UpdateCardDisplay();
        }
    }

    public void EnemyLloydSkill()
    {
        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard != null)
            {
                if (enemyCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.lloyd)
                {
                    enemyCardPoints[i].activeCard.attackPower += BattleController.instance.enemyMana;
                    enemyCardPoints[i].activeCard.anim.SetTrigger("jump");
                }
            }
        }
        if (card != null)
        {
            card.UpdateCardDisplay();
        }
    }

    public bool EnemyHasLloyd()
    {
        bool isLloyd = false;
        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard != null)
            {
                if (enemyCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.lloyd)
                {
                    isLloyd = true;
                }
            }
        }
        return isLloyd;
    }

    public bool PlayerHasLloyd()
    {
        bool isLloyd = false;
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null)
            {
                if (playerCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.lloyd)
                {
                    isLloyd = true;
                }
            }
        }
        return isLloyd;
    }

    public void PlayerGunterSkill()
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null &&
                playerCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.gunter)
            {
                if (i < enemyCardPoints.Length && enemyCardPoints[i].activeCard != null)
                {
                    enemyCardPoints[i].activeCard.DamageCard(1);
                    playerCardPoints[i].activeCard.DamageCard(1);
                }
            }
        }
    }

    public void EnemyGunterSkill()
    {
        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard != null && enemyCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.gunter)
            {
                if (i < playerCardPoints.Length && playerCardPoints[i].activeCard != null)
                {
                    playerCardPoints[i].activeCard.DamageCard(1);
                    enemyCardPoints[i].activeCard.DamageCard(1);
                }
            }
        }
    }

    public void PlayerGumball()
    {
        List<Card> validTargets = new List<Card>();

        for (int j = 0; j < enemyCardPoints.Length; j++)
        {
            if (enemyCardPoints[j].activeCard != null)
                validTargets.Add(enemyCardPoints[j].activeCard);
        }

        if (validTargets.Count > 0)
        {
            int randIndex = Random.Range(0, validTargets.Count);
            Card selectedTarget = validTargets[randIndex];

            selectedTarget.DamageCard(2);
        }
        else
            return;
    }

    public void EnemyGumball()
    {
        List<Card> validTargets = new List<Card>();

        for (int j = 0; j < playerCardPoints.Length; j++)
        {
            if (playerCardPoints[j].activeCard != null)
                validTargets.Add(playerCardPoints[j].activeCard);
        }

        if (validTargets.Count > 0)
        {
            int randIndex = Random.Range(0, validTargets.Count);
            Card selectedTarget = validTargets[randIndex];

            selectedTarget.DamageCard(2);
        }
        else
            return;
    }

    public void PlayerSpidermanSkill()
    {
        Card highestPowerEnemy = null;

        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            Card enemyCard = enemyCardPoints[i].activeCard;

            if (enemyCard != null)
            {
                enemyCard.DamageCard(1);

                if (highestPowerEnemy == null || enemyCard.attackPower > highestPowerEnemy.attackPower)
                {
                    highestPowerEnemy = enemyCard;
                }
            }
        }

        if (highestPowerEnemy != null)
        {
            highestPowerEnemy.attackPower -= 5;
            if (highestPowerEnemy.attackPower < 0)
                highestPowerEnemy.attackPower = 0;

            highestPowerEnemy.UpdateCardDisplay();
        }
    }

    public void EnemySpidermanSkill()
    {
        Card highestPowerPlayer = null;

        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            Card playerCard = playerCardPoints[i].activeCard;

            if (playerCard != null)
            {
                playerCard.DamageCard(1);

                if (highestPowerPlayer == null || playerCard.attackPower > highestPowerPlayer.attackPower)
                {
                    highestPowerPlayer = playerCard;
                }
            }
        }

        if (highestPowerPlayer != null)
        {
            highestPowerPlayer.attackPower -= 5;
            if (highestPowerPlayer.attackPower < 0)
                highestPowerPlayer.attackPower = 0;
            highestPowerPlayer.UpdateCardDisplay();
        }
    }

    public void PlayerAnais(int healAmount)
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            Card playerCard = playerCardPoints[i].activeCard;

            if (playerCard != null)
            {
                playerCard.currentHealth += healAmount;
                playerCard.UpdateCardDisplay();
            }
        }
    }

    public void EnemyAnais(int healAmount)
    {
        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            Card enemyCard = enemyCardPoints[i].activeCard;

            if (enemyCard != null)
            {
                enemyCard.currentHealth += healAmount;
                enemyCard.UpdateCardDisplay();
            }
        }
    }

    public bool EnemyHasAnais()
    {
        bool isAnais = false;
        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard != null)
            {
                if (enemyCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.anais)
                {
                    isAnais = true;
                }
            }
        }
        return isAnais;
    }

    public bool PlayerHasAnais()
    {
        bool isAnais = false;
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null)
            {
                if (playerCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.anais)
                {
                    isAnais = true;
                }
            }
        }
        return isAnais;
    }

    public void PlayerNaruto()
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            Card playerCard = playerCardPoints[i].activeCard;

            if (playerCard != null && playerCard.cardSO.cardsSkill != CardScriptableObject.cardSkills.naruto)
            {
                playerCard.DamageCard(500);
            }
        }
        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            Card enemyCard = enemyCardPoints[i].activeCard;

            if (enemyCard != null)
            {
                enemyCard.DamageCard(500);
            }
        }
    }

    public void EnemyNaruto()
    {
        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            Card enemyCard = enemyCardPoints[i].activeCard;

            if (enemyCard != null && enemyCard.cardSO.cardsSkill != CardScriptableObject.cardSkills.naruto)
            {
                enemyCard.DamageCard(500);
            }
        }
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            Card playerCard = playerCardPoints[i].activeCard;

            if (playerCard != null)
            {
                playerCard.DamageCard(500);
            }
        }
    }

    public void IdahoSkill()
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null &&
                playerCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.idaho)
            {
                if (i < enemyCardPoints.Length && enemyCardPoints[i].activeCard != null)
                {
                    enemyCardPoints[i].activeCard.attackPower -= 1;
                    playerCardPoints[i].activeCard.DamageCard(1);
                    enemyCardPoints[i].activeCard.UpdateCardDisplay();
                }
            }
        }
    }

    public void PlayerBMO()
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            Card playerCard = playerCardPoints[i].activeCard;

            if (playerCard != null && playerCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.finn)
            {
                playerCard.attackPower += 4;
                playerCard.UpdateCardDisplay();
            }

            if (playerCard != null && playerCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.bmo)
            {
                playerCard.attackPower += 1;
                playerCard.UpdateCardDisplay();
            }
        }
    }

    public bool PlayerHasBmo()
    {
        bool isBmo = false;
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null)
            {
                if (playerCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.bmo)
                {
                    isBmo = true;
                }
            }
        }
        return isBmo;
    }

    public void FlapjackSkill()
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null &&
                playerCardPoints[i].activeCard.cardSO.cardsSkill == CardScriptableObject.cardSkills.flapjack)
            {
                List<Card> validTargets = new List<Card>();

                for (int j = 0; j < enemyCardPoints.Length; j++)
                {
                    if (playerCardPoints[j].activeCard != null && playerCardPoints[j].activeCard.cardSO.cardsSkill != CardScriptableObject.cardSkills.flapjack)
                        validTargets.Add(playerCardPoints[j].activeCard);
                }

                if (validTargets.Count > 0)
                {
                    int randIndex = Random.Range(0, validTargets.Count);
                    Card selectedTarget = validTargets[randIndex];

                    selectedTarget.currentHealth += 2;
                    selectedTarget.UpdateCardDisplay();
                }
                playerCardPoints[i].activeCard.DamageCard(1);
            }
        }
    }

    public void PlayerMordecai()
    {
        List<Card> validTargets = new List<Card>();

        for (int j = 0; j < enemyCardPoints.Length; j++)
        {
            if (enemyCardPoints[j].activeCard != null)
                validTargets.Add(enemyCardPoints[j].activeCard);
        }

        if (validTargets.Count > 0)
        {
            for (int k = 0; k < 3; k++)
            {
                int randIndex = Random.Range(0, validTargets.Count);
                Card selectedTarget = validTargets[randIndex];
                selectedTarget.DamageCard(1);

                validTargets.RemoveAt(randIndex);

                if (validTargets.Count == 0)
                    break;
            }
        }
        else
            return;
    }

    public void EnemyMordecai()
    {
        List<Card> validTargets = new List<Card>();

        for (int j = 0; j < playerCardPoints.Length; j++)
        {
            if (playerCardPoints[j].activeCard != null)
                validTargets.Add(playerCardPoints[j].activeCard);
        }

        if (validTargets.Count > 0)
        {
            for (int k = 0; k < 3; k++)
            {
                int randIndex = Random.Range(0, validTargets.Count);
                Card selectedTarget = validTargets[randIndex];
                selectedTarget.DamageCard(1);

                validTargets.RemoveAt(randIndex);

                if (validTargets.Count == 0)
                    break;
            }
        }
        else
            return;
    }
}
