using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DeckController : MonoBehaviour

{
    public static DeckController Instance;
    private UI_DeckBuilder db;

    [SerializeField] private bool randomizeDeck;
    private List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private MyStack<CardScriptableObject> activeCards= new MyStack<CardScriptableObject>();
    [SerializeField] private float waitForDrawing = .25f;
    public CardScriptableObject bmoCard;

    public List<CardScriptableObject> alienDeck = new List<CardScriptableObject>();
    [SerializeField] private MyStack<CardScriptableObject> activeAliens = new MyStack<CardScriptableObject>();

    public Card cardToSpawn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TransferDeck();
        SetUpDeck();
        SetUpAlienDeck();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            DrawAlienToHand();
    }

    private void TransferDeck()
    {
         deckToUse = UI_DeckBuilder.instance.selectedCards;
    }

    public void SetUpDeck()
    {
       activeCards = new MyStack<CardScriptableObject>();
       List<CardScriptableObject> tempDeck= new List<CardScriptableObject>();
       tempDeck.AddRange(deckToUse);

        if(randomizeDeck)
            Shuffle(tempDeck);

        for (int i = 0; i < deckToUse.Count; i++)
        {
            activeCards.Push(tempDeck[i]);
        }

    }

    public void SetUpAlienDeck()
    {
        activeAliens = new MyStack<CardScriptableObject>();
        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(alienDeck);

        if (randomizeDeck)
            Shuffle(tempDeck);

        for (int i = 0; i < alienDeck.Count; i++)
        {
            activeAliens.Push(tempDeck[i]);
        }
    }

    public void DrawCardToHand()
    {
        if (HandController.instance.heldCard.Count < 10)
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
        else
            Debug.Log("Max Hand");
    }

    public void DrawBMOToHand()
    {
        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        newCard.cardSO = bmoCard;
        newCard.SetupCard();
        HandController.instance.AddBMOToHand(newCard);
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

    public void DrawAlienToHand()
    {
        if (HandController.instance.heldCard.Count < 10)
        {
            if (activeCards.IsEmpty())
            {
                SetUpAlienDeck();
            }
            if (!activeCards.IsEmpty())
            {
                Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
                newCard.cardSO = activeAliens.Pop();
                newCard.SetupCard();
                HandController.instance.AddCardToHand(newCard);
            }
        }
        else
            Debug.Log("Max Hand");
    }

    public void DrawCardForMana(int manaCost)
    {
        if (HandController.instance.heldCard.Count < 10)
        {
            if (BattleController.instance.playerMana >= manaCost)
            {
                BattleController.instance.SpendPlayerMana(manaCost);
                DrawCardToHand();
            }
            else
                UIController.instance.ShowManaWarning();
        }
        else
            Debug.Log("Max Hand");
    }

    public void DrawMultipleCards(int amountToDraw)
    {
        StartCoroutine(DrawMultipleCo(amountToDraw));
    }

    IEnumerator DrawMultipleCo(int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand();
            yield return new WaitForSeconds(waitForDrawing);
        }
    }
}
