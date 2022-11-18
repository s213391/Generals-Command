using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenDeathTimer : MonoBehaviour
{
    public void TimerEnd()
    {
        SuddenDeath.Begin();
    }
}
