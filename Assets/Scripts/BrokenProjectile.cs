using UnityEngine;

public class BrokenProjectilePiece : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector2 initialVelocity = new Vector2(2, 3);
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Settings")]
    [SerializeField] private float lifetime = 2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (rb != null)
        {
            // Applica velocità iniziale
            rb.linearVelocity = initialVelocity;

            // Applica rotazione
            rb.angularVelocity = rotationSpeed;
        }

        // Auto-distruggi dopo tot secondi
        Destroy(gameObject, lifetime);
    }
}