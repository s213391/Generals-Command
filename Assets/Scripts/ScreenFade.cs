using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade instance;
    public Image panel;
    
    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public static void In(float seconds)
    {
        instance.StartCoroutine(instance.Fade(true, seconds));
    }


    public static void Out(float seconds)
    {
        instance.StartCoroutine(instance.Fade(false, seconds));
    }


    IEnumerator Fade(bool fadeIn, float seconds)
    {
        float timer = 0.0f;
        panel.enabled = true;
        Color panelColour = panel.color;

        while (timer < seconds)
        {
            if (fadeIn)
                panelColour.a = 1 - timer / seconds;
            else
                panelColour.a = timer / seconds;

            panel.color = panelColour;
            yield return null;
        }

        if (fadeIn)
            panel.enabled = false;
        else
            panelColour.a = 1.0f;

        panel.color = panelColour;
    }
}
