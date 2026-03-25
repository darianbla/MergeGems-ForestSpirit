using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    public float speed = 50f;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}