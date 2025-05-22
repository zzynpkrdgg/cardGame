using UnityEngine;
using TMPro;

public class UIDamageIndicator : MonoBehaviour
{
    private RectTransform myRect;

    public TMP_Text damageText;
    public float moveSpeed;
    public float lifeTime = 3f;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        myRect.anchoredPosition += new Vector2(0f, -moveSpeed * Time.deltaTime);
    }
}
