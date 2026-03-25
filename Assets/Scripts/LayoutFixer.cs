using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LayoutFixer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FixLayout());
    }

    private IEnumerator FixLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void Refresh()
    {
        StartCoroutine(FixLayout());
    }
}