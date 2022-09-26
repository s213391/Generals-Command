using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTSModularSystem
{

    public class SnapPoint : MonoBehaviour
    {
        [SerializeField]
        private PlayerObjectData[] data;
        [HideInInspector]
        public Vector3 rendererCentre;

        public static List<SnapPoint> snapPoints { get; private set; }


        //adds self to the list of snap points
        private void Start()
        {
            if (snapPoints == null)
                snapPoints = new List<SnapPoint>();
            snapPoints.Add(this);

            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer)
                rendererCentre = renderer.bounds.center;
            else
                rendererCentre = transform.position;
        }


        //returns an array of all the names of stored data
        public List<string> GetData()
        {
            List<string> dataNames = new List<string>();
            for (int i = 0; i < data.Length; i++)
                dataNames.Add(data[i].name);
            return dataNames;
        }


        //returns all snap points that accept the given data name
        public static List<SnapPoint> GetCompatibleSnapPoints(string dataName)
        {
            List<SnapPoint> compatibleSnapPoints = new List<SnapPoint>();

            for (int i = 0; i < snapPoints.Count; i++)
            {
                //add relevant snappoints
                List<string> snapPointData = snapPoints[i].GetData();
                if (snapPointData.Contains(dataName))
                    compatibleSnapPoints.Add(snapPoints[i]);
            }

            return compatibleSnapPoints;
        }
    }
}
