using System.Collections;
using UnityEngine;

public class FanPlatform : MonoBehaviour
{
    [Header("Fan Settings")]
    [SerializeField] private float _windForce = 10f;
    [SerializeField] private float _activeDuration = 3f;
    [SerializeField] private float _inactiveDuration = 2f;
    [SerializeField] private Collider2D _windCollider;

    private Animator _fanAnimator;
    private bool _isActive = false;
    private ParticleSystem _fanParticle;

    private void Start()
    {
        _fanParticle = GetComponentInChildren<ParticleSystem>();
        _fanAnimator = GetComponentInChildren<Animator>();

        _fanParticle?.Stop(); // Ensure the particle system is stopped at the start

        if (_windCollider != null)
            _windCollider.enabled = false;

        StartCoroutine(FanCycle());
    }

    private IEnumerator FanCycle()
    {
        while(true) //becuse it's infinite loop, it will keep running until the object is destroyed
        {
            // Activate the fan
            _isActive = true;
            if (_fanAnimator != null)
            {
                _fanAnimator.SetBool("IsActive", true);
            }
            if (_fanParticle != null)
            {
                _fanParticle.Play();
            }
            _windCollider.enabled = true;
            yield return new WaitForSeconds(_activeDuration);

            // Deactivate the fan
            _isActive = false;
            if (_fanAnimator != null)
            {
                _fanAnimator.SetBool("IsActive", false);
            }
            if (_fanParticle != null)
            {
                _fanParticle.Stop();
            }
            _windCollider.enabled = false;
            yield return new WaitForSeconds(_inactiveDuration);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player == null)
            return;

        Vector2 windDirection = transform.up;

        player.SetWindForce(windDirection * _windForce);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player == null)
            return;

        player.SetWindForce(Vector2.zero);
    }
}

