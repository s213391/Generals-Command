using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    float timeSinceLastSecond;
    int framesSinceLastSecond;
    
    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        timeSinceLastSecond = 0.0f;
        framesSinceLastSecond = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastSecond += Time.deltaTime;
        framesSinceLastSecond++;

        if (timeSinceLastSecond >= 1.0f)
        {
            textMeshProUGUI.text = framesSinceLastSecond + " FPS";

            timeSinceLastSecond -= 1.0f;
            framesSinceLastSecond = 0;
        }
    }
}
