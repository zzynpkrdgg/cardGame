using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public string battleSelectedscene;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject deckMenu;

    public void GoToDeckManager()
    {
        mainMenu.SetActive(false);
        deckMenu.SetActive(true);
    }

    public void ReturnBackToMainMenu()
    {
        mainMenu.SetActive(true);
        deckMenu.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(battleSelectedscene);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quitting game...");
    }
}
