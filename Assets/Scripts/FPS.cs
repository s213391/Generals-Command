using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    
    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = "FPS: " + (int)(1.0f / Time.deltaTime);
    }
}
