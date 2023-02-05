using Fusion;
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
    public float variability = 1f;
    
    [Networked] public bool attacking { set; get; }

    private NetworkObject networkObject;

    // Start is called before the first frame update
    void Start()
    {
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
        }
    }

    void Shoot()
    {
        bool moving = false;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            moving = true;
        }

        Vector3 originalEulerAngle = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, 0, moving ? Random.Range(transform.eulerAngles.z - 10, transform.eulerAngles.z + 10) : transform.eulerAngles.z);

        RaycastHit2D hit = Physics2D.Raycast(gunPoint.position, transform.up, range);
        Transform trail = Instantiate(bulletTrail, gunPoint.position, transform.rotation);
        BulletTrailScript trailScript = trail.GetComponent<BulletTrailScript>();


        if (hit.collider != null)
        {
            trailScript.SetTargetPosition(hit.point);
            if (hit.collider.gameObject.transform.parent.tag == "Player" && hit.collider.gameObject.GetComponentInParent<Gun>().attacking != attacking)
            {
                hit.collider.gameObject.GetComponentInParent<PlayerLife>().UpdateHealth(30);
            }
        }
        else
        {
            Vector3 endPos = gunPoint.position + transform.up * range;
            trailScript.SetTargetPosition(endPos);
        }
        transform.eulerAngles = originalEulerAngle;

    }
}
