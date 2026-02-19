using UnityEngine;

public class ParallaxBackgrounds : MonoBehaviour
{
    [SerializeField] private Transform[] _backgrounds = new Transform[5];

    private float[] _speeds = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

    private Transform _camera;
    private Vector3 _previousCameraPosition;

    private void Start()
    {
        _camera = Camera.main.transform;
        _previousCameraPosition = _camera.position;
    }

    private void LateUpdate()
    {
        float deltaX = _camera.position.x - _previousCameraPosition.x;

        for (int i = 0; i < _backgrounds.Length; i++)
        {
            if (_backgrounds[i] == null) continue;
            _backgrounds[i].position += new Vector3(deltaX * _speeds[i], 0f, 0f);
        }

        _previousCameraPosition = _camera.position;
    }
}
