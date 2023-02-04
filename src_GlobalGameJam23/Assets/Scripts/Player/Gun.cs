using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField] GameObject obstacleRayObject;
    [SerializeField] GameObject characterObject;
    [SerializeField] float obstacleRayDistance;
    float characterDirection;


    // Start is called before the first frame update
    void Start()
    {
        characterDirection = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hitObstacle = Physics2D.Raycast(obstacleRayObject.transform.position, Vector2.right * new Vector2(characterDirection, 0f), obstacleRayDistance);

        if(hitObstacle.collider != null)
        {
            Debug.Log("Ray collision detected");
            Debug.DrawRay(obstacleRayObject.transform.position, Vector2.right * hitObstacle.distance * new Vector2(characterDirection, 0f), Color.red);

        }
        else
        {
            Debug.Log("No collision detected");
            Debug.DrawRay(obstacleRayObject.transform.position, Vector2.right * hitObstacle.distance * new Vector2(characterDirection, 0f), Color.green);
        }
    }
}
