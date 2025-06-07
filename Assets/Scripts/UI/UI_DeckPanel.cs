using System.Collections.Generic;
using UnityEngine;

public class UI_DeckPanel : MonoBehaviour
{
    [SerializeField] private UI_CardShow panelPrefab;
    [SerializeField] private Transform panelParent;

    public static UI_DeckPanel instance;

    private List<UI_CardShow> activePanels = new List<UI_CardShow>();

    private void Awake()
    {
        instance = this;
    }

    public void ShowPanel(CardScriptableObject cardToShow)
    {
        UI_CardShow newShow = Instantiate(panelPrefab, panelParent);
        newShow.SetupButton(cardToShow);
        activePanels.Add(newShow);
    }

    public void RemovePanel(CardScriptableObject cardToRemove)
    {
        UI_CardShow panelToRemove = activePanels.Find(panel => panel.GetCard() == cardToRemove);

        if (panelToRemove != null)
        {
            activePanels.Remove(panelToRemove);
            Destroy(panelToRemove.gameObject);
        }
    }
}