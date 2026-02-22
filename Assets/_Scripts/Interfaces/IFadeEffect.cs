using UnityEngine;
using UnityEngine.UI;

public interface IFadeEffect
{
    void FadeIn(Image imageToFade, Color to, float time);
    void FadeOut(Image imageToFade, Color from, float time);
}
