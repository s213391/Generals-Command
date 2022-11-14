using UnityEngine;

namespace RTSModularSystem.BasicCombat
{
    //displays all health bars on the canvas it is attached to
    public class HealthBarManager : MonoBehaviour
    {
        public GameObject prefab;
        public static HealthBarManager instance;


        //set up singleton
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //create a health bar and display it on the canvas
        public HealthBar AddHealthBar(Attackable health)
        {
            GameObject go = Instantiate(prefab);
            go.transform.SetParent(transform);
            HealthBar hb = go.GetComponent<HealthBar>();
            hb.attackable = health;

            return hb;
        }
    }
}
