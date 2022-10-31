using UnityEngine;
using UnityEngine.UI;

public class CurrentlyBuildingIcon : MonoBehaviour
{
    public Image meter;
    private GameObject rootGameObject;
    public Image icon;

    private float duration;
    private float elapsedDuration;
    private float heightOffset;
    private bool currentlyActive;


    //set up the meter
    public void Start()
    {
        rootGameObject = transform.root.gameObject;
        heightOffset = GetComponent<RectTransform>().anchoredPosition3D.y;
        if (meter)
        {
            meter.type = Image.Type.Filled;
            meter.fillMethod = Image.FillMethod.Horizontal;
        }

        currentlyActive = false;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }


    //sets the icon and starts increasing the meter
    public void NewProduction(Sprite currentProduction, float buildTimer)
    {
        icon.sprite = currentProduction;
        duration = buildTimer;
        elapsedDuration = 0.0f;

        currentlyActive = true;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);
    }


    //update position and meter
    private void Update()
    {
        if (!currentlyActive)
            return;

        transform.position = rootGameObject.transform.position + Vector3.up * heightOffset;
        transform.forward = Camera.main.transform.forward;

        // scale the meter
        elapsedDuration += Time.deltaTime;
        meter.fillAmount = Mathf.Clamp01(elapsedDuration / duration);

        //hide when timer complete
        if (elapsedDuration >= duration)
        {
            currentlyActive = false;
            for (int i=0;i<transform.childCount;i++)
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}

