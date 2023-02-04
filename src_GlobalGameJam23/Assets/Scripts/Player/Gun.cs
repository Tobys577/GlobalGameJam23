using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] public Transform gunPoint;
    [SerializeField] public Transform bulletTrail;
    [SerializeField] public float range = 10f;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(gunPoint.position, transform.up, range);
        Transform trail = Instantiate(bulletTrail, gunPoint.position, transform.rotation);
        BulletTrailScript trailScript = trail.GetComponent<BulletTrailScript>();

        if(hit.collider != null)
        {
            trailScript.SetTargetPosition(hit.point);
            //code here for hit target
        }
        else
        {
            Vector3 endPos = gunPoint.position + transform.up * range;
            trailScript.SetTargetPosition(endPos);
        }

    }
}
