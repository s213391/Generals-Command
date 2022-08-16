using UnityEngine;
using TMPro;

public class VersionNumber : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI textGUI = GetComponent<TextMeshProUGUI>();
        textGUI.text = "Version: " + Application.version;
    }
}
