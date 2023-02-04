using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    GameObject camera;
    private NetworkObject networkObject;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        networkObject = GetComponent<NetworkObject>();
        camera = GameObject.Find("Main Camera");
    }

    private void Update()
    {
        if (networkObject.HasInputAuthority)
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
        }

         Vector3 mousePosition = Input.mousePosition;
         Vector3 targetPosition = camera.GetComponent<Camera>().ScreenToWorldPoint(mousePosition);
         Vector3 relativePos = targetPosition - transform.position;
         
         Quaternion rotation = Quaternion.LookRotation(relativePos);
         transform.rotation = rotation;
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
