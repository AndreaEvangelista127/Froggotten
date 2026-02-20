using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyCounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CollectiblesManager _collectiblesManager;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _flyCountText;
    [SerializeField] private Image _flyImage;

    private void Update()
    {
        if (_collectiblesManager == null || _flyCountText == null) return;

        int collected = _collectiblesManager.GetCollectedFlies();
        int total = _collectiblesManager.GetTotalFlies();
        _flyCountText.text = $"{collected}/{total}";
    }
}
