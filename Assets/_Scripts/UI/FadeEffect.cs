using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour, IFadeEffect
{
    private Color _clearColor = Color.clear;

    public void FadeIn(Image imageToFade, Color to, float time)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(imageToFade, _clearColor, to, time));
    }

    public void FadeOut(Image imageToFade, Color from, float time)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(imageToFade, from, _clearColor, time));
    }

    private IEnumerator Fade(Image image, Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            image.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
}
