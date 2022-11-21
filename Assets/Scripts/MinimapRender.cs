using UnityEngine;
using UnityEngine.UI;

public class MinimapRender : MonoBehaviour
{
    public Camera renderCamera;
    public RawImage minimapBackground;

    Texture2D mapRender;

    private void LateUpdate()
    {
        if (renderCamera)
        {
            CamCapture();
            enabled = false;
            minimapBackground.texture = mapRender;
        }
    }


    void CamCapture()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderCamera.targetTexture;

        renderCamera.Render();

        mapRender = new Texture2D(renderCamera.targetTexture.width, renderCamera.targetTexture.height, TextureFormat.RGB24, false, true);
        mapRender.ReadPixels(new Rect(0, 0, renderCamera.targetTexture.width, renderCamera.targetTexture.height), 0, 0);
        mapRender.Apply();
        RenderTexture.active = currentRT;
    }
}