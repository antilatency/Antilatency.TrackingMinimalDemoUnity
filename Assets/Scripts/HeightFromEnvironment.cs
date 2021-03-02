using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightFromEnvironment : MonoBehaviour
{
    public AltEnvironmentHeight EnvironmentHeight;

    public float HeightOnFloor = 2.0f;

    public void Start()
    {
        if (EnvironmentHeight == null)
        {
            Debug.Log("No environment height component attached");
        }

        var height = EnvironmentHeight.GetHeight();

        transform.position = new Vector3(transform.position.x, height > 0.0f ? height : HeightOnFloor, transform.position.z);
    }
}