using Fusion;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : NetworkBehaviour
{
    [SerializeField] public Transform gunPoint;
    [SerializeField] public Transform bulletTrail;
    [SerializeField] public float range = 100f;
    [SerializeField] private float shotCoolDown = 0.2f;
    [SerializeField] private GameObject smoke;
    public float variability = 1f;
    int smokeCount = 2;
    
    [Networked] public bool attacking { set; get; }

    private NetworkObject networkObject;
    BulletTrailScript trailScript = new BulletTrailScript();

    public float lastShot;

    // Start is called before the first frame update
    void Start()
    {
        lastShot = Time.time;
        networkObject = GetComponent<NetworkObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (networkObject.HasInputAuthority)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
            if (Input.GetMouseButtonDown(1) && smokeCount > 0)
            {
                Smoke();
            }
        }
    }

    void Shoot()
    {
        if(Time.time - lastShot < shotCoolDown)
        {
            return;
        }

        bool moving = false;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            moving = true;
        }

        Vector3 originalEulerAngle = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, 0, moving ? Random.Range(transform.eulerAngles.z - 25, transform.eulerAngles.z + 25) : transform.eulerAngles.z);

        RaycastHit2D hit = Physics2D.Raycast(gunPoint.position, transform.up, range);
        Vector2 targetPos = Vector2.zero;

        if (hit.collider != null)
        {
            targetPos = (hit.point);
            if (hit.collider.gameObject.transform.parent.tag == "Player" && hit.collider.gameObject.GetComponentInParent<Gun>().attacking != attacking)
            {
                hit.collider.gameObject.GetComponentInParent<PlayerLife>().UpdateHealth(30);
            }
        }
        else
        {
            Vector3 endPos = gunPoint.position + transform.up * range;
            targetPos = (endPos);
        }
        transform.eulerAngles = originalEulerAngle;

        RPC_SpawnBulletTrail(gunPoint.position, transform.rotation, targetPos);
    }

    void Smoke()
    {
        RPC_SpawnSmoke(transform.position);
        smokeCount--;
    }

    [Rpc]
    public void RPC_SpawnBulletTrail(Vector2 pos, Quaternion ang, Vector2 targetPos)
    {
        Transform trail = Instantiate(bulletTrail, pos, ang);
        trailScript = trail.GetComponent<BulletTrailScript>();
        trailScript.SetTargetPosition(targetPos);
    }

    [Rpc]
    public void RPC_SpawnSmoke(Vector2 pos)
    {
        Instantiate(smoke, pos, Quaternion.identity);
    }
}