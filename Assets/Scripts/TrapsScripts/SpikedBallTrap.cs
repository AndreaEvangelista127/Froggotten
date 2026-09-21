using UnityEngine;

public class SpikedBallTrap : TrapBase
{
    [SerializeField] private GameObject _chainpathOrigin;
    [SerializeField] private GameObject _chainPrefab;
    [SerializeField] private float _chainSpacing = 0.5f;
    [SerializeField] private float _rotationSpeed = 100f;

    private void Awake()
    {
        if (_chainpathOrigin != null)
        {
            CreateChainPath();
        }
    }

    private void CreateChainPath()
    {
        // Calculate the distance between the trap and the chain path origin
        float distance = Vector2.Distance(transform.position, _chainpathOrigin.transform.position);
        int numberOfChains = Mathf.CeilToInt(distance / _chainSpacing);
        // Calculate the direction from the trap to the chain path origin
        Vector2 direction = (_chainpathOrigin.transform.position - transform.position).normalized;

        for (int i = 0; i < numberOfChains; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + direction * _chainSpacing * i;
            GameObject chainSegment = Instantiate(_chainPrefab, spawnPosition, Quaternion.identity);
            chainSegment.transform.SetParent(_chainpathOrigin.transform);
        }
    }

    private void Update()
    {
        // Rotate the spiked ball around its own axis
        gameObject.transform.RotateAround(_chainpathOrigin.transform.position, Vector3.forward, _rotationSpeed * Time.deltaTime);

    }

}
