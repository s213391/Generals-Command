using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSModularSystem.FogOfWar
{

    public class FogSampler : MonoBehaviour
    {
        [SerializeField]
        private RenderTexture fogTexture;
        [SerializeField]
        private Projector fogProjector;
        private Texture2D readableTexture;

        private Vector2 fogBottomLeft;
        private float fogDimension;
        private Color fogColour = new Color(0, 0, 0, 0);
        private bool firstCallThisFrame = true;



        //get the bottom left point of the fog to match the (0,0) point of a texture
        void Start()
        {
            Vector3 fogPos = fogProjector.transform.position;
            fogDimension = fogProjector.orthographicSize;
            fogBottomLeft = new Vector2(fogPos.x, fogPos.z) - new Vector2(fogDimension, fogDimension);

            //set up the texture2d that the rendertexture will read into every frame
            readableTexture = new Texture2D(fogTexture.width, fogTexture.height, TextureFormat.ARGB32, false);
        }


        //return true if the given point is currently in the vision of an owned object
        public bool IsVisible(Vector3 objectPosition)
        {
            //get the coordinates of the object relative to the fog, 
            Vector2 fogPosition = new Vector2(objectPosition.x, objectPosition.z) - fogBottomLeft;
            fogPosition *= fogTexture.width / (2.0f * fogDimension);

            //read rendered texture into a texture2d that can be sampled
            if (firstCallThisFrame)
            {
                RenderTexture.active = fogTexture;
                readableTexture.ReadPixels(new Rect(0, 0, fogTexture.width, fogTexture.height), 0, 0);
            }

            return readableTexture.GetPixel((int)fogPosition.x, (int)fogPosition.y) != fogColour;
        }
    }
}
