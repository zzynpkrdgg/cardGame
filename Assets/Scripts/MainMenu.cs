using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public string battleSelectedscene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
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
