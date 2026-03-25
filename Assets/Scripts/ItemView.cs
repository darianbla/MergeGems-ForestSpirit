using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image glowImage;
    [SerializeField] private Image shadowImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float spawnDuration = 0.4f;
    [SerializeField] private AnimationCurve spawnCurve = new AnimationCurve();
    [SerializeField]
    private List<Vector2> popKeyframes = new List<Vector2>()
    {
        new Vector2(0f, 0f),
        new Vector2(0.6f, 1.2f),
        new Vector2(0.8f, 0.9f),
        new Vector2(1f, 1f)
    };

    // --- MAGICAL SYNC FUNCTION ---
    private void OnValidate()
    {
        if (popKeyframes.Count > 0)
        {
            spawnCurve = new AnimationCurve();
            foreach (Vector2 point in popKeyframes)
            {
                spawnCurve.AddKey(point.x, point.y);
            }
            for (int i = 0; i < spawnCurve.length; i++)
            {
                spawnCurve.SmoothTangents(i, 0);
            }
        }
    }

    // State
    public int Level { get; private set; } = 1;
    public CellView CurrentCell { get; set; }

    // Dependencies
    private RectTransform rect;
    private Transform dragLayer;
    private Canvas rootCanvas;

    private void Awake()
    {
        OnValidate();
    }

    public void Initialize(int startLevel, Sprite sprite, Transform dragLayerRef, Canvas canvasRef)
    {
        rect = GetComponent<RectTransform>();
        dragLayer = dragLayerRef;
        rootCanvas = canvasRef;
        SetLevel(startLevel, sprite);
    }

    public void SetLevel(int newLevel, Sprite sprite)
    {
        Level = newLevel;
        name = $"Item_Lvl_{Level}";

        if (sprite != null)
        {
            if (iconImage != null) iconImage.sprite = sprite;
            if (glowImage != null) glowImage.sprite = sprite;

            if (shadowImage != null) shadowImage.sprite = sprite;
        }

        if (glowImage != null)
        {
            bool shouldGlow = (Level >= 1);
            glowImage.gameObject.SetActive(shouldGlow);
        }
    }

    // --- Drag Logic ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragLayer == null) return;
        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
        transform.SetParent(dragLayer);
        MoveToMousePosition(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveToMousePosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;
        if (transform.parent == dragLayer)
        {
            ReturnToCell();
        }
    }

    private void MoveToMousePosition(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)dragLayer,
            screenPosition,
            rootCanvas.worldCamera,
            out Vector2 localPoint
        );
        transform.localPosition = localPoint;
        Vector3 fixedPos = transform.localPosition;
        fixedPos.z = 0;
        transform.localPosition = fixedPos;
    }

    public void ReturnToCell()
    {
        if (CurrentCell == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 cellWorldPos = CurrentCell.transform.position;
        Vector3 targetLocalPos = transform.parent.InverseTransformPoint(cellWorldPos);
        StartCoroutine(SmoothReturnRoutine(targetLocalPos));
    }

    private IEnumerator SmoothReturnRoutine(Vector3 targetPos)
    {
        Vector3 startPos = transform.localPosition;
        float duration = 0.2f;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.SetParent(CurrentCell.transform);
        rect.anchoredPosition = Vector2.zero;
    }

    // --- Animations ---

    public void AnimateSpawn()
    {
        transform.localScale = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(AnimateScale(Vector3.zero, Vector3.one));
    }

    public void AnimateMergeUpgrade()
    {
        StartCoroutine(PunchScale(1.2f, 0.2f));
    }

    private IEnumerator AnimateScale(Vector3 start, Vector3 end)
    {
        float time = 0;
        while (time < spawnDuration)
        {
            time += Time.deltaTime;
            float linearT = time / spawnDuration;
            float curveT = spawnCurve.Evaluate(linearT);
            transform.localScale = Vector3.LerpUnclamped(start, end, curveT);
            yield return null;
        }
        transform.localScale = end;
    }

    private IEnumerator PunchScale(float punchAmount, float duration)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * punchAmount;
        float halfTime = duration / 2;
        float time = 0;
        while (time < halfTime)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, time / halfTime);
            yield return null;
        }
        time = 0;
        while (time < halfTime)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, time / halfTime);
            yield return null;
        }
        transform.localScale = originalScale;
    }

    public void MoveToLocalPosition(Vector2 targetPos, float duration = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothMoveRoutine(targetPos, duration));
    }

    private IEnumerator SmoothMoveRoutine(Vector2 target, float duration)
    {
        Vector2 start = rect.anchoredPosition;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            rect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }
        rect.anchoredPosition = target;
    }
}