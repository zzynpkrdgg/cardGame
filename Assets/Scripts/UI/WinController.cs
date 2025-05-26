using UnityEngine;
using System.Collections;

public class WinController : MonoBehaviour
{
    public Animator myKitty;
    public RectTransform kittyTransform;
    public float moveSpeed = 200f;
    public float endXPosition = 1500f;
    public GameObject winPanel;

    public static WinController instance;

    private void Awake()
    {
        instance = this;
    }

    public void showKitty()
    {
        
        myKitty.Play("kittyy");
        StartCoroutine(MoveKittyAndHide());

    }
    private IEnumerator MoveKittyAndHide()
    {
        while (kittyTransform.anchoredPosition.x < endXPosition)
        {
            kittyTransform.anchoredPosition += new Vector2(moveSpeed * Time.deltaTime, 0f);
            yield return null;
        }


        kittyTransform.gameObject.SetActive(false);
    }
}
