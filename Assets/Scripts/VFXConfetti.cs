using UnityEngine;

public class VFXConfetti : MonoBehaviour
{
    /// <summary>
    /// Spawns a confetti particle effect at the given position, plays it, and destroys it after 6 seconds.
    /// </summary>
    /// <param name="prefab">The confetti GameObject prefab to instantiate.</param>
    /// <param name="position">The world position where the confetti will be spawned.</param>
    public static void SpawnConfettiWithTimer(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        GameObject confetti = Object.Instantiate(prefab, position, Quaternion.identity);

        ParticleSystem ps = confetti.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        Object.Destroy(confetti, 6f);
    }


}
