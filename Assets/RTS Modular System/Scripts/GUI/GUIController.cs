using UnityEngine;
using UnityEngine.Events;
using RTSModularSystem.Selection;

namespace RTSModularSystem
{
    public class GUIController : MonoBehaviour
    {
        public static GUIController instance;

        private static GUIResources guiResources;
        private static GUIActionBar guiBuildings;
        private static GUISelected guiSelected;

        private static GameObject deviceGUICanvas;

        public UnityEvent onGUIInitialise;

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
                /*if (SystemInfo.deviceType == DeviceType.Desktop)
                    deviceGUICanvas = instance.transform.GetChild(0).gameObject;
                else*/

                //only mobile supported right now
                    deviceGUICanvas = instance.transform.GetChild(1).gameObject;
                deviceGUICanvas.SetActive(true);

                //initialise all components of the gui
                guiResources = deviceGUICanvas.GetComponentInChildren<GUIResources>(true);
                guiResources.Init();
                guiBuildings = deviceGUICanvas.GetComponentInChildren<GUIActionBar>(true);
                guiBuildings.Init();
                guiSelected = deviceGUICanvas.GetComponentInChildren<GUISelected>(true);
                guiSelected.Init();
                instance.onGUIInitialise.Invoke();
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
            if (guiResources.isActiveAndEnabled)
                guiResources.OnUpdate();
        }
    }
}
