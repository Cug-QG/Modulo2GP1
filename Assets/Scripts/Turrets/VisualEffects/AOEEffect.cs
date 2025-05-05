using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEEffect : MonoBehaviour
{
    public float maxScale = 5f;
    public float duration = 0.5f;
    public GameObject circle;
    private float timer;

    void Start()
    {
        timer = 0f;
        circle.transform.localScale = Vector3.zero;
        Destroy(gameObject, duration);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        float scale = Mathf.Lerp(0f, maxScale, t);
        circle.transform.localScale = new Vector3(scale, scale, 1f);
    }
}
