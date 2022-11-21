using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem.BasicCombat;

public class GameMusic : MonoBehaviour
{
    public AudioSource calmMusic;
    public AudioSource combatMusic;

    public bool inCombat;

    [Range(0,1)]
    public float combatTransitionValue = 0.0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        calmMusic.volume = 1;
        combatMusic.volume = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (inCombat)
            combatTransitionValue += Time.deltaTime;
        else
            combatTransitionValue -= Time.deltaTime;

        combatTransitionValue = Mathf.Clamp(combatTransitionValue, 0.0f, 1.0f);

        calmMusic.volume = 1 - combatTransitionValue;
        combatMusic.volume = combatTransitionValue;
    }
}
