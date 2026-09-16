using UnityEngine;

public static class BounceUtility 
{
    
    public static void BouncePlayer(GameObject target, float bounceForce)
    {
        Rigidbody2D playerRb = target.GetComponent<Rigidbody2D>();
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f); // Reset vertical velocity
            playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }

        if (playerMovement != null)
        {
            playerMovement.ResetJumps(); // Reset jumps to allow for double jump after bounce
        }
    }
}
