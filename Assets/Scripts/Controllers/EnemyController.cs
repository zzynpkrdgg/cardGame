using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;
    public Transform cardSpawnPoint;

    public enum AIType { placeFromDeck, handRandomPlace, handDefensive, handAttacking}
    public AIType enemyAIType;

    private List<CardScriptableObject> cardsInHand = new List<CardScriptableObject>();
    public int startHandSize;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetupDeck();
        if(enemyAIType != AIType.placeFromDeck)
            SetupHand();
    }

    public void SetupDeck()
    {
        activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++;
        }
    }

    public void StartAction()
    {
        StartCoroutine(EnemyActionCo());
    }

    IEnumerator EnemyActionCo()
    {

        if (activeCards.Count == 0)
            SetupDeck();

        yield return new WaitForSeconds(.5f);

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(CardPointsController.instance.enemyCardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        if (enemyAIType == AIType.placeFromDeck || enemyAIType == AIType.handRandomPlace)
            PlaceRandom(cardPoints, ref randomPoint, ref selectedPoint);

        SetupAIType(selectedPoint);

        yield return new WaitForSeconds(.7f);

        BattleController.instance.AdvanceTurn();
    }

    private static void PlaceRandom(List<CardPlacePoint> cardPoints, ref int randomPoint, ref CardPlacePoint selectedPoint)
    {
        cardPoints.Remove(selectedPoint);

        while (selectedPoint.activeCard != null && cardPoints.Count > 0)
        {
            randomPoint = Random.Range(0, cardPoints.Count);
            selectedPoint = cardPoints[randomPoint];
            cardPoints.RemoveAt(randomPoint);
        }
    }

    private void SetupAIType(CardPlacePoint selectedPoint)
    {
        switch (enemyAIType)
        {

            case AIType.placeFromDeck:

                PlaceFromDeck(selectedPoint);
                break;
            case AIType.handRandomPlace:
                break;

            case AIType.handDefensive:
                break;

            case AIType.handAttacking:
                break;
        }
    }

    private void PlaceFromDeck(CardPlacePoint selectedPoint)
    {
        if (selectedPoint.activeCard == null)
        {
            Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
            newCard.cardSO = activeCards[0];
            activeCards.RemoveAt(0);
            newCard.SetupCard();
            newCard.MoveToPoint(selectedPoint.transform.position, selectedPoint.transform.rotation);

            selectedPoint.activeCard = newCard;
            newCard.assignedPlace = selectedPoint;
        }
    }

    private void SetupHand()
    {
        for (int i = 0; i < startHandSize; i++)
        {
            if (activeCards.Count == 0)
                SetupDeck();

            cardsInHand.Add(activeCards[0]);
            activeCards.RemoveAt(0);
        }
    }
}
