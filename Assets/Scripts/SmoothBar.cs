using UnityEngine;
using UnityEngine.UI;
public class SmoothBar : MonoBehaviour
{
    private Image barImage;
    public float animationSpeed=2f;
    public float targetFill = 1f;
    private void Awake()
    {
        barImage = GetComponent<Image>();
    }

   
    void Update()
    {
        if (Mathf.Abs(barImage.fillAmount - targetFill) > 0.001f)
        {
            barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, targetFill, Time.deltaTime * animationSpeed);
        }
    }
    public void SetValue(float current, float max)
    {
        targetFill = Mathf.Clamp01(current / max);
    }
}
