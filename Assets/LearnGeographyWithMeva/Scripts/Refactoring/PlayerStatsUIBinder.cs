using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStatsUIBinder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private Slider xpSlider;

    private void Start()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.SetUI(levelText, xpText, xpSlider);
        }
    }
}