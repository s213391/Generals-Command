using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationPanel : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public AudioSource audioSource;

    public float moveDuration;
    public Transform startPoint;
    public Transform endPoint;

    public float activeDuration;
    float activeTime;

    public bool active;
    public int notificationType;

    //turn panel off and move to 
    void Start()
    {
        transform.position = startPoint.position;
        gameObject.SetActive(false);
    }


    //updates panel text and moves it into position
    public void OpenPanel(int type, string text, AudioClip clip)
    {
        notificationType = type;
        textMeshProUGUI.text = text;

        activeTime = 0.0f;
        transform.position = startPoint.position;

        gameObject.SetActive(true);

        if (clip && audioSource)
            audioSource.PlayOneShot(clip);
    }


    //move the notification down then back up over its lifetime
    void Update()
    {
        activeTime += Time.deltaTime;
        if (activeTime >= activeDuration)
        {
            NotificationManager.instance.ClearNotification(this);
            gameObject.SetActive(false);
        }
        else if (activeTime < moveDuration)
            transform.position = Vector3.Lerp(startPoint.position, endPoint.position, activeTime / moveDuration);
        else if (activeTime > activeDuration - moveDuration)
            transform.position = Vector3.Lerp(startPoint.position, endPoint.position, (activeDuration - activeTime) / moveDuration);
    }
}
