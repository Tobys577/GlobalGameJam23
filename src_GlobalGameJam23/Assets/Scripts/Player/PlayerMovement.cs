using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public SpriteRenderer bodySprite;

    [SerializeField] private float speed;
    private FieldOfView fieldOfView;
    private Vector3 mousePos;
    private NetworkObject networkObject;
    private Rigidbody2D rb;

    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        networkObject = GetComponent<NetworkObject>();

        mainCamera = FindObjectOfType<Camera>();

        //fieldOfView = GameObject.Find("FieldOfView").GetComponent<FieldOfView>();
    }

    private void Update()
    {
        if (networkObject.HasInputAuthority)
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);

            faceMouse();
            //fieldOfView.SetDirection(transform.up);
            //fieldOfView.SetOrigin(transform.position);
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void faceMouse()
    {
        mousePos = Input.mousePosition;
        mousePos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2 direction = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        direction.Normalize();
        transform.up = direction;
    }
}
