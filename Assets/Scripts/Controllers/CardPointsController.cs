using System.Collections;
using System.Drawing;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{
    public static CardPointsController instance;
    public CardPlacePoint[] playerCardPoints, enemyCardPoints;
    public float waitBetweenAttacks = .3f;

    private void Awake()
    {
        instance = this;
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
                                enemyCardPoints[j].activeCard.DamageCard(5);
                            }
                        }
                        BattleController.instance.DamageEnemy(5);
                    }
                    else
                    {
                        for (int j = 0; j < enemyCardPoints.Length; j++)
                        {
                            if (enemyCardPoints[j].activeCard != null)
                            {
                                enemyCardPoints[j].activeCard.DamageCard(5);
                            }
                        }
                        BattleController.instance.DamageEnemy(5);
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
                    BattleController.instance.DamageEnemy(10);

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
            BattleController.instance.playerHealth += 2;
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
                                playerCardPoints[j].activeCard.DamageCard(5);
                            }
                        }
                        BattleController.instance.DamagePlayer(5);
                    }
                    else
                    {
                        for (int j = 0; j < playerCardPoints.Length; j++)
                        {
                            if (playerCardPoints[j].activeCard != null)
                            {
                                playerCardPoints[j].activeCard.DamageCard(5);
                            }
                        }
                        BattleController.instance.DamagePlayer(5);
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
                    BattleController.instance.DamagePlayer(10);

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
            BattleController.instance.enemyHealth += 2;
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

    public void PlayerKaiFlameEveryone()
    {
        for (int j = 0; j < enemyCardPoints.Length; j++)
        {
            if (enemyCardPoints[j].activeCard != null)
            {
                enemyCardPoints[j].activeCard.DamageCard(2);
            }
        }
    }

    public void EnemyKaiFlameEveryone()
    {
        Debug.Log("EnemyKaiFlameEveryone called");
        Debug.Log("playerCardPoints length: " + playerCardPoints.Length);

        for (int j = 0; j < playerCardPoints.Length; j++)
        {
            if (playerCardPoints[j].activeCard != null)
            {
                Debug.Log($"Damaging player card at point {j}");
                playerCardPoints[j].activeCard.DamageCard(2);
            }
            else
            {
                Debug.Log($"No active card at playerCardPoints[{j}]");
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
                else
                    Debug.Log("No Lloyd Here");
            }
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

}
