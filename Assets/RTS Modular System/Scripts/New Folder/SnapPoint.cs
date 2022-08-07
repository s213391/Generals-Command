using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTSModularSystem
{

    public class SnapPoint : MonoBehaviour
    {
        [SerializeField]
        private PlayerObjectData[] data;

        public static List<SnapPoint> snapPoints { get; private set; }


        //adds self to the list of snap points
        private void Awake()
        {
            if (snapPoints == null)
                snapPoints = new List<SnapPoint>();
            snapPoints.Add(this);
        }


        //returns an array of all the names of stored data
        public List<string> getData()
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
                List<string> snapPointData = snapPoints[i].getData();
                if (snapPointData.Contains(dataName))
                    compatibleSnapPoints.Add(snapPoints[i]);
            }

            return compatibleSnapPoints;
        }
    }
}
