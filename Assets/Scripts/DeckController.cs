using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DeckController : MonoBehaviour

{
    public static DeckController Instance;
    private void Awake()
    {
        Instance = this;
    }
    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    [SerializeField] private MyStack<CardScriptableObject> activeCards= new MyStack<CardScriptableObject>();
    public Card cardToSpawn;
    public void SetUpDeck()
    {
        activeCards =new MyStack<CardScriptableObject>();
        List<CardScriptableObject> tempDeck= new List<CardScriptableObject>();
       tempDeck.AddRange(deckToUse);
        Shuffle(tempDeck);
        for (int i = 0; i < deckToUse.Count; i++)
        {
            activeCards.Push(tempDeck[i]);
            Debug.Log("card eklendi!");
        }

    }
    public void DrawCardToHand()
    {
        if (activeCards.IsEmpty())
        {
            SetUpDeck();
        }
        if (!activeCards.IsEmpty())
        {
            Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
            newCard.cardSO = activeCards.Pop();
            newCard.SetupCard();
            HandController.instance.AddCardToHand(newCard);
        }

    }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        SetUpDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DrawCardToHand();
        }
    }
    public void Shuffle(List<CardScriptableObject> list) 
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand=Random.Range(i,list.Count);
            var temp= list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
