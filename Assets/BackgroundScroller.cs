using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed = 0.1f;

    private Material _material;
    private float _offset;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        _offset += _scrollSpeed * Time.deltaTime;
        _material.SetTextureOffset("_MainTex", new Vector2(0f, _offset));
    }
}
