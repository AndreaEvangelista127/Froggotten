using TMPro;
using UnityEngine;

public class HintZone : MonoBehaviour
{
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private string message = "Press [Space] to jump!";

    private void Start()
    {
        // Hide the hint at game start
        hintText.enabled = false;
        hintText.text = message;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        hintText.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        hintText.enabled = false;
    }
}
