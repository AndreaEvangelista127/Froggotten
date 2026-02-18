using UnityEngine;

public class VFXConfetti : MonoBehaviour
{
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
