using UnityEngine;

[CreateAssetMenu(fileName = "MergeConfig", menuName = "Game/MergeConfig")]
public class MergeConfig : ScriptableObject
{
    [Header("Item Levels")]
    public Sprite[] levelSprites;

    public Sprite GetSprite(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, levelSprites.Length - 1);
        return levelSprites[index];
    }
}