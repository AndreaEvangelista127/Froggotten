using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fly"))
        {
            Collectibles collectible = collision.GetComponent<Collectibles>();
            if (collectible != null)
            {
                collectible.Collect();
            }
        }
    }
}
