using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public string battleSelectedscene;
    
    void Start()
    {

    }

    
    void Update()
    {

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
