using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    public Animator myKitty;
    public RectTransform kittyTransform;
    public float moveSpeed = 200f;
    public float endXPosition = 1500f;
    public GameObject winPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
