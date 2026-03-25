using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;

    // Settings
    private float duration = 1.0f;
    private float moveSpeed = 1.0f;
    private float growAmount = 1.5f;

    public void Init(string text, Vector3 worldPosition, Canvas parentCanvas)
    {
        tmp.text = text;

        transform.SetParent(parentCanvas.transform, false);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        Color resetColor = tmp.color;
        resetColor.a = 1f;
        tmp.color = resetColor;

        transform.position = worldPosition + new Vector3(0, 0.5f, 0);
        Vector3 fixedPos = transform.localPosition;
        fixedPos.z = -50f;
        transform.localPosition = fixedPos;

        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float time = 0;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * growAmount;
        Color startColor = tmp.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            float alpha = 1f - (t * t);
            Color c = startColor;
            c.a = alpha;
            tmp.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}