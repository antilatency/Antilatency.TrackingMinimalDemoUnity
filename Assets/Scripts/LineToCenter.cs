using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToCenter : MonoBehaviour
{
    public LineRenderer _lineRenderer;

    private void Update()
    {
        _lineRenderer.SetPosition(0, transform.root.position);
        _lineRenderer.SetPosition(1, transform.position);
    }
}