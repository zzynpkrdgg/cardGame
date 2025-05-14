using System.Collections.Generic;
using Mono.Cecil;
using NUnit.Framework;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    public MyLinkedList<Card> heldCard = new MyLinkedList<Card>();

    public Transform minPos, maxPos;
    public List<Vector3> cardPositions = new List<Vector3>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetCardPositionsInHand();
    }


    public void SetCardPositionsInHand()
    {
        cardPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;
        if (heldCard.Count > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCard.Count - 1);
        }

        SetCardToHand(distanceBetweenPoints);
        
    }

    private void SetCardToHand(Vector3 distanceBetweenPoints)
    {
        for (int i = 0; i < heldCard.Count; i++)
        {
            Vector3 targetPos = minPos.position + (distanceBetweenPoints * i);
            cardPositions.Add(targetPos);

            Card card = heldCard.GetAt(i);
          
            card.MoveToPoint(targetPos,minPos.rotation);
            
            card.inHand = true;
            card.handPosition = i;
        }
    }

    public void RemoveCardFromHand(Card cardToRemove)
    {
        Card cardInHand = heldCard.GetAt(cardToRemove.handPosition);
        if (cardInHand== cardToRemove)
        {
            heldCard.RemoveAt(cardToRemove.handPosition);
        }
        else
            Debug.LogError("Card position " + cardToRemove.handPosition + " is not the card being removed from hand");

        SetCardPositionsInHand();
    }
    public void AddCardToHand(Card cardToAdd)
    {
        heldCard.Add(cardToAdd);
        SetCardPositionsInHand();
    }
}
