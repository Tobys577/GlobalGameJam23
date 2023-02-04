using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
