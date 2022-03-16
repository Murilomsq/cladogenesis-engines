using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalSpace : MonoBehaviour
{
    public GameObject parent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vital Space"))
        {
            Debug.Log("hera");
            Destroy(parent);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Vital Space"))
        {
            Destroy(parent);
        }
    }
}
