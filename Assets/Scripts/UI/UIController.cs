using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] private TMP_Text playerManaText;
    [SerializeField] private TMP_Text enemyManaText;
    [SerializeField] private GameObject manaWarning;
    [SerializeField] private float manaWarnTimer = 1.5f;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text enemyHealthText;
    public GameObject endScreen;
    public GameObject drawButton, endTurnButton;
    public UIDamageIndicator playerDamage, enemyDamage;
    //public TMP_Text resultText;
    //public Image resultImage;
    public GameObject winPanel;
    public GameObject losePanel;
    public string selectedMainMenu;
    public string selectedBattleMenu;
    private float counterManaWarning;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        ManaWarningTimer();
    }

    private void ManaWarningTimer()
    {
        if (counterManaWarning > 0)
        {
            counterManaWarning -= Time.deltaTime;

            if (counterManaWarning <= 0)
                manaWarning.SetActive(false);
        }
    }

    public void SetManaText(int manaAmount)
    {
        playerManaText.text = manaAmount.ToString();
    }

    public void SetEnemyManaText(int manaAmount)
    {
        enemyManaText.text = manaAmount.ToString();
    }

    public void DrawCardButton()
    {
        DeckController.Instance.DrawCardForMana(2);
    }

    public void ShowManaWarning()
    {
        manaWarning.SetActive(true);
        counterManaWarning = manaWarnTimer;
    }

    public void EndTurnButton()
    {
        BattleController.instance.EndPlayerTurn();
    }

    public void UpdatePlayerHealth()
    {
        playerHealthText.text = "Player: " + BattleController.instance.playerHealth;
    }

    public void UpdateEnemyHealth()
    {
        enemyHealthText.text = "Enemy: " + BattleController.instance.enemyHealth;
    }
    public void showWinPanel()
    {
        winPanel.SetActive(true);
        // myKitty.Play("kittyy");
        // StartCoroutine(MoveKittyAndHide());
        WinController.instance.showKitty();
    }
   
    public void showLosePanel()
    {
        losePanel.SetActive(true);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(selectedMainMenu);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(selectedBattleMenu);
    }
}
