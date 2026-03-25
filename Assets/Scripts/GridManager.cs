using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform gridRoot;
    [SerializeField] private Transform dragLayer;
    [SerializeField] private Canvas mainCanvas;
    [Header("Prefabs & Config")]
    [SerializeField] private CellView cellPrefab;
    [SerializeField] private ItemView itemPrefab;
    [SerializeField] private MergeConfig mergeConfig;

    [Header("Size")]
    [SerializeField] private int columns = 6;
    [SerializeField] private int rows = 6;

    [Header("Effects")]
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private ParticleSystem mergeParticlePrefab;

    private CellView[,] cells;

    public event Action<int> OnMerge;

    private void Awake()
    {
        if (mergeConfig == null)
            Debug.LogError("Assign MergeConfig in GridManager!");

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        foreach (Transform child in gridRoot) Destroy(child.gameObject);

        cells = new CellView[columns, rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var cell = Instantiate(cellPrefab, gridRoot);
                cell.name = $"Cell_{x}_{y}";
                cell.Init(x, y, this);
                cells[x, y] = cell;
            }
        }
    }

    public bool TrySpawnItem(int level = 1)
    {
        if (!GetRandomEmptyCell(out var cell)) return false;

        SpawnItemAt(cell, level);
        return true;
    }

    private void SpawnItemAt(CellView cell, int level)
    {
        var item = Instantiate(itemPrefab, cell.transform);
        var sprite = mergeConfig.GetSprite(level);

        item.Initialize(level, sprite, dragLayer, mainCanvas);
        item.CurrentCell = cell;
        cell.SetItem(item);

        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        item.AnimateSpawn();
    }

    // --- The Core Logic ---

    public void OnItemDroppedOnCell(ItemView droppedItem, CellView targetCell)
    {
        ItemView occupantItem = targetCell.GetComponentInChildren<ItemView>();

        if (occupantItem == null)
        {
            MoveItem(droppedItem, targetCell);
        }
        else if (occupantItem.Level == droppedItem.Level)
        {
            MergeItems(droppedItem, occupantItem);
        }
        else
        {
            droppedItem.ReturnToCell();
        }
    }

    private void MoveItem(ItemView item, CellView newCell)
    {
        if (item.CurrentCell != null) item.CurrentCell.SetItem(null);

        item.CurrentCell = newCell;
        newCell.SetItem(item);

        item.transform.SetParent(newCell.transform);

        item.MoveToLocalPosition(Vector2.zero);
    }

    private void MergeItems(ItemView incoming, ItemView target)
    {
        int newLevel = target.Level + 1;


        Sprite newSprite = mergeConfig.GetSprite(newLevel);

        target.SetLevel(newLevel, newSprite);

        target.AnimateMergeUpgrade();

        ShowFloatingText($"Level {target.Level}!", target.transform.position);

        if (mergeParticlePrefab != null)
        {
            var vfx = Instantiate(mergeParticlePrefab, target.transform.position, Quaternion.identity);

        }

        Destroy(incoming.gameObject);

        OnMerge?.Invoke(newLevel);
    }

    // --- Utility ---
    private bool GetRandomEmptyCell(out CellView cell)
    {
        var emptyCells = new System.Collections.Generic.List<CellView>();

        foreach (var c in cells)
        {
            if (c.GetComponentInChildren<ItemView>() == null)
                emptyCells.Add(c);
        }

        if (emptyCells.Count == 0)
        {
            cell = null;
            return false;
        }

        cell = emptyCells[Random.Range(0, emptyCells.Count)];
        return true;
    }

    private void ShowFloatingText(string text, Vector3 position)
    {
        if (floatingTextPrefab != null)
        {
            var floaty = Instantiate(floatingTextPrefab, mainCanvas.transform);
            floaty.Init(text, position, mainCanvas);
        }
    }
}