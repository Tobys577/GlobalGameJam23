using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(destroyAfterTen());
    }

    IEnumerator destroyAfterTen()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
