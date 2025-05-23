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
            if (playerCardPoints[i].activeCard != null)
            {
                if (enemyCardPoints[i].activeCard != null)
                {
                    enemyCardPoints[i].activeCard.DamageCard(playerCardPoints[i].activeCard.attackPower);
                }
                else
                {
                    BattleController.instance.DamageEnemy(playerCardPoints[i].activeCard.attackPower);
                }
                playerCardPoints[i].activeCard.anim.SetTrigger("attack");
                yield return new WaitForSeconds(waitBetweenAttacks);
            }
        }
        CheckAssignedCards();

        BattleController.instance.AdvanceTurn();
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
            if (enemyCardPoints[i].activeCard != null)
            {
                if (playerCardPoints[i].activeCard != null)
                {
                    playerCardPoints[i].activeCard.DamageCard(enemyCardPoints[i].activeCard.attackPower);
                }
                else
                {
                    BattleController.instance.DamagePlayer(enemyCardPoints[i].activeCard.attackPower);
                }
                enemyCardPoints[i].activeCard.anim.SetTrigger("attack");
                yield return new WaitForSeconds(waitBetweenAttacks);
            }
        }
        CheckAssignedCards();

        BattleController.instance.AdvanceTurn();
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
}
