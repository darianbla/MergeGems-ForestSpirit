using UnityEngine;
using UnityEngine.UI;

public class GlowPulse : MonoBehaviour
{
    private Image img;
    private float minAlpha = 0.2f;
    private float maxAlpha = 0.6f;
    private float speed = 1.5f;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    private void Update()
    {
        if (img == null) return;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * speed, 1));

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}