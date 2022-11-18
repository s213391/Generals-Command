using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPColourChanger : MonoBehaviour
{
    [SerializeField]
    Color colour1 = Color.white;
    [SerializeField]
    Color colour2 = Color.black;
    [SerializeField]
    float colour1Duration = 0.5f;
    [SerializeField]
    float transition1Duration = 0.5f;
    [SerializeField]
    float colour2Duration = 0.5f;
    [SerializeField]
    float transition2Duration = 0.5f;

    TextMeshProUGUI textMeshProUGUI;
    float timer;
    Color tempColour;
    
    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        StartCoroutine(ChangeColour());
    }

    
    IEnumerator ChangeColour()
    {
        while (true)
        {
            timer = 0.0f;
            textMeshProUGUI.color = colour1;
            yield return new WaitForSeconds(colour1Duration);

            while (timer < transition1Duration)
            {
                if (!isActiveAndEnabled)
                    yield return null;

                timer += Time.deltaTime;

                tempColour = textMeshProUGUI.color;
                tempColour.r = Mathf.Lerp(colour1.r, colour2.r, timer / transition1Duration);
                tempColour.g = Mathf.Lerp(colour1.g, colour2.g, timer / transition1Duration);
                tempColour.b = Mathf.Lerp(colour1.b, colour2.b, timer / transition1Duration);
                textMeshProUGUI.color = tempColour;

                yield return null;
            }

            timer = 0.0f;
            textMeshProUGUI.color = colour2;
            yield return new WaitForSeconds(colour2Duration);

            while (timer < transition2Duration)
            {
                if (!isActiveAndEnabled)
                    yield return null;

                timer += Time.deltaTime;

                tempColour = textMeshProUGUI.color;
                tempColour.r = Mathf.Lerp(colour2.r, colour1.r, timer / transition2Duration);
                tempColour.g = Mathf.Lerp(colour2.g, colour1.g, timer / transition2Duration);
                tempColour.b = Mathf.Lerp(colour2.b, colour1.b, timer / transition2Duration);
                textMeshProUGUI.color = tempColour;

                yield return null;
            }
        }
    }


    //resets the timer
    public void Reset()
    {
        timer = 0.0f;
    }
}
