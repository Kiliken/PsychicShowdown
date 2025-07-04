using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarBG : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;
    private Color originalColor;
    private Vector3 originalPosition;

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 10f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        originalColor = image.color;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void TakeDamage()
    {
        StopAllCoroutines(); // In case multiple hits happen
        StartCoroutine(FlashAndShake());
    }

    private IEnumerator FlashAndShake()
    {
        // Turn red
        image.color = Color.red;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            rectTransform.anchoredPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset position and color
        rectTransform.anchoredPosition = originalPosition;
        image.color = originalColor;
    }
}
