using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOnPlayer : MonoBehaviour
{
    public FogOfWar fogOfWar;
    public GameObject fogOfWarMask;
    public Transform secondaryFogOfWar;
    [Range(0, 5)]
    public float sightDistance;
    public float checkInterval;

    NetworkObject netObject;

    void Start()
    {
        netObject = GetComponent<NetworkObject>();

        StartCoroutine(CheckFogOfWar(checkInterval));
        secondaryFogOfWar.localScale = new Vector2(sightDistance, sightDistance) * 10f;
        fogOfWar = GameObject.Find("FogOfWar").GetComponent<FogOfWar>();
    }
    private IEnumerator CheckFogOfWar(float checkInterval)
    {
        while (true)
        {
            fogOfWarMask.SetActive(netObject.HasStateAuthority);

            fogOfWar.MakeHole(transform.position, sightDistance);
            yield return new WaitForSeconds(checkInterval);
        }
    }
}