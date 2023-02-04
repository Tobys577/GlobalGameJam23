using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Animations;

public class BulletTrailScript : MonoBehaviour
{
    private Vector3 startpos;
    private Vector3 targetpos;
    private float progress;

    [SerializeField] private float speed = 40f;
    void Start()
    {
        startpos = transform.position.WithAxis(VectorsExtension.Axis.Z, -1);
    }


    void Update()
    {
        progress += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(startpos, targetpos, progress);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        targetpos = targetPosition.WithAxis(VectorsExtension.Axis.Z, -1);
    }
}
