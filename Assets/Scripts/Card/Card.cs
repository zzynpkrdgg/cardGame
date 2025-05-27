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

    public bool canRevive;
    public bool isStunned;
    public int stunDuration = 0;
    public bool canOverwhelm;


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

        if (cardSO.cardsSkill == CardScriptableObject.cardSkills.invincible)
            canRevive = true;
    }

    private void Update()
    {
        HandleMove();
        PickCards();

        if (attackPower < 0)
            attackPower = 0;
    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;
        canOverwhelm = cardSO.hasOverwhelm;

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

                    ActivateCardEffect();
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

        if (cardSO.cardsSkill == CardScriptableObject.cardSkills.allenTheAlien)
        {
            attackPower += 4;
            currentHealth += 2;
            anim.SetTrigger("jump");
        }

        if (currentHealth <= 0)
        {
            if (cardSO.cardsSkill == CardScriptableObject.cardSkills.invincible && canRevive == true)
            {
                currentHealth = 1;
                canRevive = false;
                UpdateCardDisplay();
                return;
            }

            if (cardSO.cardsSkill == CardScriptableObject.cardSkills.drawCardOnDeath)
                DeckController.Instance.DrawCardToHand();

            if (cardSO.cardsSkill == CardScriptableObject.cardSkills.atomEve && assignedPlace.isPlayerPoint)
                CardPointsController.instance.PlayerKaiFlameEveryone(cardSO.buffValue);
            else if (cardSO.cardsSkill == CardScriptableObject.cardSkills.atomEve && assignedPlace.isPlayerPoint == false)
                CardPointsController.instance.EnemyKaiFlameEveryone(cardSO.buffValue);

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

    public void ActivateCardEffect()
    {
        switch (cardSO.cardsSkill)
        {
            case CardScriptableObject.cardSkills.drawCardOnPlay:
                if (assignedPlace.isPlayerPoint)
                    DeckController.Instance.DrawCardToHand();
                else
                    EnemyController.instance.EnemyDrawCard();
                break;

            case CardScriptableObject.cardSkills.buffAllies:
                BuffAllies(cardSO.buffValue);
                break;

            case CardScriptableObject.cardSkills.kai:
                if (assignedPlace.isPlayerPoint)
                    CardPointsController.instance.PlayerKaiFlameEveryone(cardSO.buffValue);
                else
                    CardPointsController.instance.PlayerKaiFlameEveryone(cardSO.buffValue);
                break;

            case CardScriptableObject.cardSkills.gunter:
                if (assignedPlace.isPlayerPoint)
                    CardPointsController.instance.PlayerGunterSkill();
                else
                    CardPointsController.instance.EnemyGunterSkill();
                break;

            case CardScriptableObject.cardSkills.gumball:
                if (assignedPlace.isPlayerPoint)
                    CardPointsController.instance.PlayerGumball();
                else
                    CardPointsController.instance.EnemyGumball();
                break;

            case CardScriptableObject.cardSkills.spiderman:
                if (assignedPlace.isPlayerPoint)
                    CardPointsController.instance.PlayerSpidermanSkill();
                else
                    CardPointsController.instance.EnemySpidermanSkill();
                break;

            case CardScriptableObject.cardSkills.naruto:
                if (assignedPlace.isPlayerPoint)
                    CardPointsController.instance.PlayerNaruto();
                else
                    CardPointsController.instance.EnemyNaruto();
                break;

            case CardScriptableObject.cardSkills.idaho:
                if(assignedPlace.isPlayerPoint)
                    CardPointsController.instance.IdahoSkill();
                break;

            case CardScriptableObject.cardSkills.finn:
                if (assignedPlace.isPlayerPoint)
                    DeckController.Instance.DrawBMOToHand();
                break;

            case CardScriptableObject.cardSkills.healYourself:
                HealYourself(cardSO.buffValue);
                break;

            case CardScriptableObject.cardSkills.captainK:
                RandomStats();
                break;

            case CardScriptableObject.cardSkills.ben10:
                if (assignedPlace.isPlayerPoint)
                    DeckController.Instance.DrawAlienToHand();
                break;

            case CardScriptableObject.cardSkills.flapjack:
                if (assignedPlace.isPlayerPoint)
                    CardPointsController.instance.FlapjackSkill();
                break;

            case CardScriptableObject.cardSkills.rick:
                RickBuff();
                break;

            case CardScriptableObject.cardSkills.mordecai:
                if (assignedPlace.isPlayerPoint)
                    CardPointsController.instance.PlayerMordecai();
                else
                    CardPointsController.instance.EnemyMordecai();
                break;

            case CardScriptableObject.cardSkills.none:
                default:
                break;
        }
    }

    private void BuffAllies(int buffVal)
    {
        bool isPlayerSide = assignedPlace.isPlayerPoint;

        CardPlacePoint[] allies;

        if (isPlayerSide)
            allies = CardPointsController.instance.playerCardPoints;
        else
            allies = CardPointsController.instance.enemyCardPoints;

        foreach (var allyPoint in allies)
        {
            if (allyPoint.activeCard != null)
            {
                allyPoint.activeCard.attackPower += buffVal;
                allyPoint.activeCard.currentHealth += buffVal;
                allyPoint.activeCard.UpdateCardDisplay();
            }
        }
    }

    public void HealYourself(int healVal)
    {
        bool isPlayerSide = assignedPlace.isPlayerPoint;

        CardPlacePoint[] allies;

        if (isPlayerSide)
            allies = CardPointsController.instance.playerCardPoints;
        else
            allies = CardPointsController.instance.enemyCardPoints;

        foreach (var allyPoint in allies)
        {
            if (allyPoint.activeCard != null && isPlayerSide)
            {
                BattleController.instance.playerHealth += healVal;
                if (BattleController.instance.playerHealth > 50)
                    BattleController.instance.playerHealth = 50;
                UIController.instance.UpdatePlayerHealth();
            }
            if (allyPoint.activeCard != null && isPlayerSide == false)
            {
                BattleController.instance.enemyHealth += healVal;
                if (BattleController.instance.playerHealth > 50)
                    BattleController.instance.playerHealth = 50;
                UIController.instance.UpdateEnemyHealth();
            }
        }
    }

    public void RandomStats()
    {
        currentHealth = Random.Range(1, 7);
        attackPower = Random.Range(1, 7);
        anim.SetTrigger("jump");
        UpdateCardDisplay();
    }

    public void RickBuff()
    {
        bool isPlayerSide = assignedPlace.isPlayerPoint;

        CardPlacePoint[] allies;

        if (isPlayerSide)
            allies = CardPointsController.instance.playerCardPoints;
        else
            allies = CardPointsController.instance.enemyCardPoints;

        foreach (var allyPoint in allies)
        {
            if (allyPoint.activeCard != null && isPlayerSide && allyPoint.activeCard.cardSO.cardsSkill != CardScriptableObject.cardSkills.rick)
            {
                allyPoint.activeCard.canOverwhelm = true;
            }
            if (allyPoint.activeCard != null && isPlayerSide == false && allyPoint.activeCard.cardSO.cardsSkill != CardScriptableObject.cardSkills.rick)
            {
                allyPoint.activeCard.canOverwhelm = true;
            }
        }
    }
}
