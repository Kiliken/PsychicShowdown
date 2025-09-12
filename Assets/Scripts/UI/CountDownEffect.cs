using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountDownEffect : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] frames;

    private float frameDuration;
    private Vector3 initialScale;

    private void Start()
    {
        frameDuration = 1f / 10f;
        initialScale = targetImage.rectTransform.localScale;
        StartCoroutine(PlayCountdown());
    }

    private IEnumerator PlayCountdown()
    {
        float timer;
        int frameIndex;

        int totalFramesFirstPart = 10 * 3;
        for (int count = 0; count < totalFramesFirstPart; count++)
        {
            timer = 0f;
            frameIndex = count % 10;
            targetImage.sprite = frames[frameIndex];
            while (timer < frameDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        for (int i = 10; i < 17; i++)
        {
            timer = 0f;
            targetImage.sprite = frames[i];

            float t = (i - 9) / 5f;
            Vector3 targetScale = Vector3.Lerp(initialScale, initialScale * 0.3f, t);
            targetImage.rectTransform.localScale = targetScale;

            while (timer < frameDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        targetImage.sprite = frames[17];
        targetImage.rectTransform.localScale = initialScale * 0.3f;
    }
}
