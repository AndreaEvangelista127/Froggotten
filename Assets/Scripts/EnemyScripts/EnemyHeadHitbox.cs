using UnityEngine;

public class EnemyHeadHitbox : MonoBehaviour, IBounceGiver
{
    [SerializeField] private float _bounceForce = 20f;

    public float BounceForce => _bounceForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {

            IDamageable damageable = GetComponentInParent<IDamageable>();

            if (damageable != null)
            {

                damageable.Die();
                BounceUtility.BouncePlayer(collision.gameObject, _bounceForce);
            }
        }
    }
}
