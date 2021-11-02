using Antilatency.SDK;
using System.Linq;
using UnityEngine;

public class AltEnvironmentHeight : MonoBehaviour
{
    public AltEnvironmentComponent Environment;

    private void Start()
    {
        if (Environment == null)
        {
            Debug.LogError("Environment is null");
        }
    }

    public float GetHeight()
    {
        if (Environment == null)
        {
            Debug.LogError("Environment is null");
        }

        var nativeEnvironment = Environment.GetEnvironment();
        if (nativeEnvironment == null)
        {
            Debug.LogError("Native environment is null");
            return 0.0f;
        }

        var envMarkers = nativeEnvironment.getMarkers();

        var min = envMarkers.Aggregate((x, y) => x.y < y.y ? x : y).y;
        var max = envMarkers.Aggregate((x, y) => x.y > y.y ? x : y).y;

        if (Mathf.Abs(max - min) > 0.001f) {
            return 0.0f;
        } else {
            return max;
        }
    }
}