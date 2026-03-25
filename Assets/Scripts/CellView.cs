using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellView : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs")]
    [SerializeField] private Image highlight; 

    // State
    public int X { get; private set; }
    public int Y { get; private set; }
    private GridManager gridManager;

    public ItemView CurrentItem { get; private set; }

    public void Init(int x, int y, GridManager manager)
    {
        X = x;
        Y = y;
        gridManager = manager;

        if (highlight != null) highlight.gameObject.SetActive(false);
    }

    // --- Input Logic ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        ItemView incomingItem = eventData.pointerDrag.GetComponent<ItemView>();
        if (incomingItem == null) return;

        if (IsMergeable(incomingItem))
        {
            SetHighlight(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        SetHighlight(false);

        var droppedItem = eventData.pointerDrag?.GetComponent<ItemView>();
        if (droppedItem != null)
        {
            gridManager.OnItemDroppedOnCell(droppedItem, this);
        }
    }

    // --- Helpers ---

    private bool IsMergeable(ItemView incoming)
    {
        if (CurrentItem == null) return false;

        if (CurrentItem == incoming) return false;

        return CurrentItem.Level == incoming.Level;
    }

    private void SetHighlight(bool isOn)
    {
        if (highlight != null)
            highlight.gameObject.SetActive(isOn);
    }

    // GridManager calls this to update the cell's content
    public void SetItem(ItemView item)
    {
        CurrentItem = item;
    }
}