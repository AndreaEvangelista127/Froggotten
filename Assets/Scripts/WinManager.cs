using UnityEngine;

public class WinManager : MonoBehaviour
{
    [Header("Win Animation Settings")]
    [SerializeField] private string _WinAnimationTrigger = "Win";
    [SerializeField] private Animator _winAnimator;

    [Header("Trophy Settings")]
    [SerializeField] private GameObject _trophyPrefab; // Prefab del trofeo
    [SerializeField] private Transform _trophySpawnPoint;

    private bool _hasAllTheFlies = false; // This should be set to true when the player collects all the required files

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerWinAnimation();
        }
    }

    private void TriggerWinAnimation()
    {
        if (_hasAllTheFlies && _winAnimator != null)
        {
            _winAnimator.SetTrigger(_WinAnimationTrigger);
        }
        else
        {
            Debug.Log("Player does not have all the required flies to win.");
        }
    }
}
