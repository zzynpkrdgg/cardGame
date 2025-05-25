using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;
    public Animator anim;
    private HandController theHC;

    public int currentHealth;
    public int attackPower;
    public int manaCost;

    public bool isPlayer;

    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text manaText;
    public TMP_Text nameText;
    public TMP_Text actionDescriptionText;
    public TMP_Text loreText;
    public Image characterArt;

    [Header("Move Effects")]
    public float moveSpeed = 5f; public float rotateSpeed = 540f;
    public bool inHand;
    public int handPosition;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    private bool isSelected;
    private Collider theCol;
    private bool justPressed;

    public LayerMask whatIsDesktop;
    public LayerMask whatIsPlacement;
    public CardPlacePoint assignedPlace;


    private void Awake()
    {
        theCol = GetComponent<Collider>();
    }

    [System.Obsolete]
    private void Start()
    {
        if (targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }

        SetupCard();

        theHC = FindObjectOfType<HandController>();
    }

    private void Update()
    {
        HandleMove();
        PickCards();
    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;

        UpdateCardDisplay();

        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        characterArt.sprite = cardSO.character;
    }

    private void PickCards()
    {
        if (isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
                MoveToPoint(hit.point + new Vector3(0f, 2f, 0f), Quaternion.identity);

            if (Input.GetMouseButtonDown(1) && BattleController.instance.battleEnded == false)
                ReturnToHand();

            if (Input.GetMouseButtonDown(0) && justPressed == false && BattleController.instance.battleEnded == false)
            {
                hit = PlaceCard(ray);
            }
        }

        justPressed = false;
    }

    private RaycastHit PlaceCard(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement) && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
        {
            CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

            if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
            {
                if (BattleController.instance.playerMana >= manaCost)
                {
                    selectedPoint.activeCard = this;
                    assignedPlace = selectedPoint;

                    MoveToPoint(selectedPoint.transform.position, Quaternion.identity);

                    inHand = false;
                    isSelected = false;

                    theHC.RemoveCardFromHand(this);

                    BattleController.instance.SpendPlayerMana(manaCost);
                }
                else
                {
                    ReturnToHand();
                    UIController.instance.ShowManaWarning();
                }
            }
            else
                ReturnToHand();
        }
        else
            ReturnToHand();
        return hit;
    }

    private void HandleMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }

    public void MoveToPoint(Vector3 pointToMove, Quaternion rotToMatch)
    {
        targetPoint = pointToMove;
        targetRot = rotToMatch;
    }

    private void OnMouseOver()
    {
        if (inHand && isPlayer && BattleController.instance.battleEnded == false)
            MoveToPoint(theHC.cardPositions[handPosition] + new Vector3(0f, 1f, .5f), Quaternion.identity);
    }

    private void OnMouseExit()
    {
        if (inHand && isPlayer && BattleController.instance.battleEnded == false)
            MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
    }

    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer && BattleController.instance.battleEnded == false) 
        {
            isSelected = true;
            theCol.enabled = false;

            justPressed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;

        MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
    }

    public void DamageCard(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            assignedPlace.activeCard = null;
            MoveToPoint(BattleController.instance.discardPoint.position, BattleController.instance.discardPoint.rotation);
            anim.SetTrigger("jump");
            Destroy(gameObject, 5f);
        }

        anim.SetTrigger("hurt");

        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        manaText.text = manaCost.ToString();
    }
}
