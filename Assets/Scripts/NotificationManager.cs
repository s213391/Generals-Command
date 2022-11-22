using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem.BasicCombat;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance { get; private set; }

    public NotificationPanel[] notificationPanels = new NotificationPanel[2];
    public GameObject nuclearCountdownTimer;
    public string[] notificationTexts = new string[4];
    public AudioClip[] notificationSounds = new AudioClip[4];
    bool[] activeNotificationTypes = new bool[4];


    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


    //creates a notification of the given type
    public void RequestNotification(int notificationType, string unitName = "")
    {
        //only one notification of a type allowed at once
        if (activeNotificationTypes[notificationType])
            return;

        int index = AssignNotificationPanel(notificationType);
        if (index == -1)
            return;

        activeNotificationTypes[notificationType] = true;

        if (notificationType == 3)
        {
            if (!CombatManager.inCombat)
                notificationPanels[index].OpenPanel(3, $"Your {unitName} is under attack", notificationSounds[3]);
        }
        else
            notificationPanels[index].OpenPanel(notificationType, notificationTexts[notificationType], notificationSounds[notificationType]);
    }


    //clears a notification
    public void ClearNotification(NotificationPanel panel)
    {
        activeNotificationTypes[panel.notificationType] = false;
    }


    //returns the best notification panel for this notification
    int AssignNotificationPanel(int notificationType)
    {
        if (!notificationPanels[0].active)
            return 0;
        if (!notificationPanels[1].active)
            return 1;

        if (notificationPanels[0].notificationType > notificationType)
        {
            ClearNotification(notificationPanels[0]);
            return 0;
        }
        if (notificationPanels[1].notificationType > notificationType)
        {
            ClearNotification(notificationPanels[1]);
            return 1;
        }

        return -1;
    }
}
