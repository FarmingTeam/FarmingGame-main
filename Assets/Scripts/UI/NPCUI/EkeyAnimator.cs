using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EkeyAnimator : MonoBehaviour
{
    public float moveDistance = 10f;
    public float moveSpeed = 2f;
    private Vector2 startPos;

    void Start()
    {
        startPos = ((RectTransform)transform).anchoredPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.unscaledTime * moveSpeed) * moveDistance;
        ((RectTransform)transform).anchoredPosition = startPos + Vector2.up * offsetY;
    }
}
