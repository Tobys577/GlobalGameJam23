using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera cam;

    private void Start()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject.HasStateAuthority)
        {
            cam = FindObjectOfType<Camera>();
        } else
        {
            Destroy(this);
        }
    }
    void Update()
    {
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
