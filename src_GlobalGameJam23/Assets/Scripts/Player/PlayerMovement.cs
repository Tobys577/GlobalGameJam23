using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public SpriteRenderer bodySprite;

    [SerializeField] private float speed;
    private Vector3 mousePos;
    private NetworkObject networkObject;
    private Rigidbody2D rb;

    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        networkObject = GetComponent<NetworkObject>();

        mainCamera = FindObjectOfType<Camera>();

    }

    private void Update()
    {
        if (networkObject.HasInputAuthority)
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);

            faceMouse();
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

    public void callChangeSprite(int sprIndex)
    {
        RPC_ChangeSprite(sprIndex);
    }

    [Rpc]
    public void RPC_ChangeSprite(int spr)
    {
        bodySprite.sprite = FindObjectOfType<BasicSpawner>().characterSelectionScreen.characters[spr].characterSprite;
    }
}
