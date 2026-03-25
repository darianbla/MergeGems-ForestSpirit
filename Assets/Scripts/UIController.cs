using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Top Bar Text")]
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI coinsText;

    public void UpdateDisplay(int currentEnergy, int maxEnergy, int currentCoins)
    {
        if (energyText != null)
            energyText.text = $"{currentEnergy}/{maxEnergy}";

        if (coinsText != null)
            coinsText.text = currentCoins.ToString("N0");
    }
}