using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopController : MonoBehaviour
{
    [Header("Main Refs")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform shopContainer;
    [SerializeField] private GameController gameController;
    [SerializeField] private Button closeButton;

    [Header("Button Animation")]
    [SerializeField] private Button openShopButton;
    [SerializeField] private float punchDuration = 0.15f;
    [SerializeField] private float punchScale = 0.8f;

    [Header("Shop Items")]
    [SerializeField] private Button buyEnergyButton;
    [SerializeField] private Button buyGemsButton;
    [SerializeField] private GameObject gemsLockOverlay;

    [Header("UI Components")]
    [SerializeField] private UnityEngine.UI.ScrollRect shopScrollView;

    private int energyCost = 100;

    private void Start()
    {
        if (openShopButton != null)
        {
            openShopButton.onClick.RemoveAllListeners();

            openShopButton.onClick.AddListener(() => {
                StartCoroutine(AnimateButtonAndOpen());
            });
        }
        else
        {
            Debug.LogError("FORGOT TO ASSIGN OPEN SHOP BUTTON IN INSPECTOR!");
        }

        closeButton.onClick.AddListener(CloseShop);

        buyEnergyButton.onClick.AddListener(BuyEnergy);
        buyGemsButton.onClick.AddListener(BuyGems);

        LockGemsItem();

        shopContainer.localScale = Vector3.zero;
        shopPanel.SetActive(false);
    }

    // --- The Button "Punch" Animation ---
    private IEnumerator AnimateButtonAndOpen()
    {
        Transform btnTrans = openShopButton.transform;
        Vector3 original = Vector3.one;
        Vector3 shrunk = original * punchScale;

        float elapsed = 0;
        float halfDuration = punchDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            btnTrans.localScale = Vector3.Lerp(original, shrunk, elapsed / halfDuration);
            yield return null;
        }

        elapsed = 0;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            btnTrans.localScale = Vector3.Lerp(shrunk, original, elapsed / halfDuration);
            yield return null;
        }
        btnTrans.localScale = original;

        OpenShop();
        if (shopScrollView != null)
        {
            shopScrollView.verticalNormalizedPosition = 1f;
        }
    }

    // --- The Window Pop-Up Logic ---
    public void OpenShop()
    {

        shopPanel.SetActive(true);
        shopContainer.localScale = Vector3.zero;

        StopAllCoroutines();
        StartCoroutine(AnimateScale(Vector3.zero, Vector3.one, 0.3f, true));
    }

    public void CloseShop()
    {
        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        yield return StartCoroutine(AnimateScale(Vector3.one, Vector3.zero, 0.2f, false));
        shopPanel.SetActive(false);
    }

    private IEnumerator AnimateScale(Vector3 start, Vector3 end, float duration, bool elastic)
    {
        float time = 0;
        shopContainer.localScale = start;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float value;
            if (elastic)
            {
                float c1 = 1.70158f;
                float c3 = c1 + 1;
                value = 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
            }
            else
            {
                value = t * t * (3f - 2f * t);
            }

            shopContainer.localScale = Vector3.LerpUnclamped(start, end, value);
            yield return null;
        }
        shopContainer.localScale = end;
    }

    // --- Shop Logic ---
    private void BuyEnergy()
    {
        if (gameController.TrySpendCoins(energyCost))
        {
            gameController.AddEnergy(5);
            Debug.Log("Bought 5 Energy!");
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    private void BuyGems()
    {
        Debug.Log("This item is locked!");
    }

    private void LockGemsItem()
    {
        gemsLockOverlay.SetActive(true);
        buyGemsButton.interactable = false;
        buyGemsButton.GetComponentInChildren<TextMeshProUGUI>().text = "LOCKED";
    }
}