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

        if (enemyAIType != AIType.placeFromDeck)
        {
            cardsInHand.Add(activeCards[0]);
            activeCards.RemoveAt(0);

            if (activeCards.Count == 0)
                SetupDeck();
        }

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(CardPointsController.instance.enemyCardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        if (enemyAIType == AIType.placeFromDeck || enemyAIType == AIType.handRandomPlace)
            PlaceRandom(cardPoints, ref randomPoint, ref selectedPoint);

        CardScriptableObject selectedCard = null;
        int iterations = 0;

        List<CardPlacePoint> prefferedPoints = new List<CardPlacePoint>();
        List<CardPlacePoint> secondaryPoints = new List<CardPlacePoint>();

        switch (enemyAIType)
        {

            case AIType.placeFromDeck:

                PlaceFromDeck(selectedPoint);

                break;

            case AIType.handRandomPlace:

                selectedCard = SelectedCardToPlay();

                iterations = 50;
                while (selectedCard != null && selectedPoint.activeCard == null && iterations > 0)
                {
                    PlayCard(selectedCard, selectedPoint);
                    selectedCard = SelectedCardToPlay();
                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.waitBetweenAttacks);

                    while (selectedPoint.activeCard != null && cardPoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPoints.Count);
                        selectedPoint = cardPoints[randomPoint];
                        cardPoints.RemoveAt(randomPoint);
                    }
                }

                break;

            case AIType.handDefensive:

                /*if (BattleController.instance.enemyHealth > BattleController.instance.enemyHealth/2)
                {
                    enemyAIType = AIType.handAttacking;
                    break;
                }*/

                selectedCard = SelectedCardToPlay();
                prefferedPoints.Clear();
                secondaryPoints.Clear();

                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerCardPoints[i].activeCard != null)
                            prefferedPoints.Add(cardPoints[i]);
                        else
                            secondaryPoints.Add(cardPoints[i]);
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && prefferedPoints.Count + secondaryPoints.Count > 0)
                {
                    if (prefferedPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, prefferedPoints.Count);
                        selectedPoint = prefferedPoints[selectPoint];

                        prefferedPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];

                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    //Check if we should play another
                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.waitBetweenAttacks);
                }
               

                    break;

            case AIType.handAttacking:

                selectedCard = SelectedCardToPlay();

                prefferedPoints.Clear();
                secondaryPoints.Clear();

                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerCardPoints[i].activeCard == null)
                            prefferedPoints.Add(cardPoints[i]);
                        else
                            secondaryPoints.Add(cardPoints[i]);
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && prefferedPoints.Count + secondaryPoints.Count > 0)
                {
                    if (prefferedPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, prefferedPoints.Count);
                        selectedPoint = prefferedPoints[selectPoint];

                        prefferedPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];

                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    //Check if we should play another
                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.waitBetweenAttacks);
                }


                break;
        }

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

    public void PlayCard(CardScriptableObject cardSO, CardPlacePoint placePoint)
    {
        Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
        newCard.cardSO = cardSO;
        
        newCard.SetupCard();
        newCard.MoveToPoint(placePoint.transform.position, placePoint.transform.rotation);

        placePoint.activeCard = newCard;
        newCard.assignedPlace = placePoint;

        cardsInHand.Remove(cardSO);

        BattleController.instance.SpendEnemyMana(cardSO.manaCost);
    }

    CardScriptableObject SelectedCardToPlay()
    {
        CardScriptableObject cardToPlay = null;

        List<CardScriptableObject> cardsToPlay = new List<CardScriptableObject>();
        foreach (CardScriptableObject card in cardsInHand)
        {
            if (card.manaCost <= BattleController.instance.enemyMana)
                cardsToPlay.Add(card);
        }
        
        if (cardsToPlay.Count > 0)
        {
            int selected = Random.Range(0, cardsToPlay.Count);

            cardToPlay = cardsToPlay[selected];
        }


        return cardToPlay;
    }
}
