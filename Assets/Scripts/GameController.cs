using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIController uiController;
    [SerializeField] private GridManager grid;
    [SerializeField] private Button spawnButton;
    [SerializeField] private GameObject winPopup;

    [Header("UI Refs (Specific)")]
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private float timePerEnergy = 120f;
    [SerializeField] private int targetLevel = 6;

    [Header("Player Data")]
    public int CurrentCoins = 1000;
    public int CurrentEnergy = 25;
    public int MaxEnergy = 50;

    private float energyTimer;

    private void Start()
    {
        energyTimer = timePerEnergy;

        spawnButton.onClick.AddListener(SpawnItem);
        grid.OnMerge += HandleMerge;

        if (winPopup != null) winPopup.SetActive(false);

        UpdateGoalUI();
        UpdateUI();
    }

    private void Update()
    {
        if (CurrentEnergy < MaxEnergy)
        {
            energyTimer -= Time.deltaTime;

            if (energyTimer <= 0)
            {
                AddEnergy(1);
                energyTimer = timePerEnergy;
            }

            if (timerText != null)
            {
                System.TimeSpan time = System.TimeSpan.FromSeconds(energyTimer);
                timerText.text = string.Format("Next Energy: {0:D2}:{1:D2}", time.Minutes, time.Seconds);
            }
        }
        else
        {
            if (timerText != null) timerText.text = "Energy Full";
        }
    }

    // --- GAMEPLAY ACTIONS ---

    private void SpawnItem()
    {
        if (CurrentEnergy <= 0) return;

        if (grid.TrySpawnItem(1))
        {
            AddEnergy(-1);
        }
    }

    private void HandleMerge(int newLevel)
    {
        if (newLevel == targetLevel)
        {
            Debug.Log("GOAL REACHED!");
            if (winPopup != null) winPopup.SetActive(true);
            spawnButton.interactable = false;
        }
    }

    // --- SHOP & ECONOMY ACTIONS ---

    public bool TrySpendCoins(int amount)
    {
        if (CurrentCoins >= amount)
        {
            CurrentCoins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        CurrentCoins += amount;
        UpdateUI();
    }

    public void AddEnergy(int amount)
    {
        CurrentEnergy += amount;
        if (CurrentEnergy > MaxEnergy) CurrentEnergy = MaxEnergy;

        UpdateUI();
    }

    // --- HELPER METHODS ---

    private void UpdateUI()
    {
        if (uiController != null)
        {
            uiController.UpdateDisplay(CurrentEnergy, MaxEnergy, CurrentCoins);
        }

        if (spawnButton != null)
            spawnButton.interactable = CurrentEnergy > 0;
    }

    private void UpdateGoalUI()
    {
        if (goalText != null)
            goalText.text = $"Goal: Create Level {targetLevel}!";
    }
}