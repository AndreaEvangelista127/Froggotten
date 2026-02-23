using UnityEngine;

public class ParallaxBackgrounds : MonoBehaviour
{
    [SerializeField] private Transform[] _backgrounds = new Transform[5];

    // Each index corresponds to a background layer, slower values for distant layers
    private float[] _speeds = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

    private Transform _camera;
    private Vector3 _previousCameraPosition;

    private void Start()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("ParallaxBackgrounds: Main camera not found!");
            return;
        }

        _camera = Camera.main.transform;
        _previousCameraPosition = _camera.position;
    }

    /// <summary>
    /// Moves each background layer horizontally based on camera movement and its assigned speed,
    /// creating a depth illusion through differential scrolling.
    /// </summary>
    private void LateUpdate() // LateUpdate used to ensure the camera has moved before we adjust the backgrounds
    {
        if (_camera == null) return;

        // How much the camera moved this frame
        float deltaX = _camera.position.x - _previousCameraPosition.x;

        // First background in the array would move slow cuz is the first value (the lowest one) in the array
        for (int i = 0; i < _backgrounds.Length; i++)
        {
            if (_backgrounds[i] == null) continue;
            _backgrounds[i].position += new Vector3(deltaX * _speeds[i], 0f, 0f);
        }

        _previousCameraPosition = _camera.position;
    }
}
