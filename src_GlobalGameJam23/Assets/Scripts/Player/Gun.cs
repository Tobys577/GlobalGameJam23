using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] public Transform gunPoint;
    [SerializeField] public Transform bulletTrail;
    [SerializeField] public float range = 100f;
    public float variability = 1f;
    bool attacking = true;

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
        bool moving = false;
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            moving = true;
        }

        Vector3 originalEulerAngle = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, 0, moving ? Random.Range(transform.eulerAngles.z - 10, transform.eulerAngles.z + 10) : transform.eulerAngles.z);

        RaycastHit2D hit = Physics2D.Raycast(gunPoint.position, transform.up ,range);
        Transform trail = Instantiate(bulletTrail, gunPoint.position, transform.rotation);
        BulletTrailScript trailScript = trail.GetComponent<BulletTrailScript>();

        

        if(hit.collider != null)
        {
            Debug.Log("hit something");
            Debug.Log(hit.collider.name);
            trailScript.SetTargetPosition(hit.point);
            if(hit.collider.gameObject.tag == "Player" && attacking)
            {
                hit.collider.gameObject.GetComponentInParent<PlayerLife>().UpdateHealth(30);
                Debug.Log("damaged player");
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
