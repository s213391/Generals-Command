using UnityEngine;
using UnityEngine.UI;
using DS_Selection;

namespace RTSModularSystem
{
    public class GUIController : MonoBehaviour
    {
        public static GUIController instance;

        private static GUIResources guiResources;
        private static GUIActionBar guiBuildings;
        private static GUISelected guiSelected;

        //Ensure only one GUIController exists
        public void Init()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //sets gui active once local player connects
        public static void PlayerConnect()
        {
            if (instance != null)
            {
                //set gui active based on platform
                if (SystemInfo.deviceType == DeviceType.Desktop)
                    instance.transform.GetChild(0).gameObject.SetActive(true);
                else
                    instance.transform.GetChild(1).gameObject.SetActive(true);

                //initialise all components of the gui
                guiResources = instance.GetComponentInChildren<GUIResources>();
                guiResources.Init();
                guiBuildings = instance.GetComponentInChildren<GUIActionBar>();
                guiBuildings.Init();
                guiSelected = instance.GetComponentInChildren<GUISelected>();
                guiSelected.Init();
            }
        }


        //sets gui inactive once local player disconnects
        public static void PlayerDisconnect()
        {
            if (instance != null)
                instance.transform.GetChild(0).gameObject.SetActive(false);
        }


        //update all components of the gui
        public static void OnUpdate()
        {
            guiResources.OnUpdate();
        }
    }
}
