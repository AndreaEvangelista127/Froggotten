using UnityEngine;
using UnityEngine.UI;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private RawImage[] _parallaxLayers;

    [SerializeField] private float[] _speeds;



    private void Update()
    {
        MoveLayer();
    }

    /// <summary>
    /// Scrolls each parallax layer horizontally by modifying its UV rect offset.
    /// Each layer moves at its own speed, and the UV resets to 0 when it reaches 1
    /// to create a seamless loop effect.
    /// </summary>
    private void MoveLayer()
    {
        for(int i = 0; i < _parallaxLayers.Length; i++)
        {
            if (_parallaxLayers[i] != null && i < _speeds.Length) 
            {
                Rect uv = _parallaxLayers[i].uvRect;  //current UV rect
                uv.x += _speeds[i] * Time.deltaTime * 0.01f; //Move the rect of the layer based on speed and time

                if (uv.x >= 1f) uv.x = 0f; //when we reach the end of the texture, reset to 0

                _parallaxLayers[i].uvRect = uv; // Apply the modified UV rect back to the RawImage
            }
        }
    }
}
