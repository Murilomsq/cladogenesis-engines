using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCounter : MonoBehaviour
{
    [SerializeField] private int envNum;
    public Vector3 envSize = new Vector3(70.0f, 10.0f, 70.0f);

    public void UpdateIndividualsInside()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, envSize);

        int sum = 0;
        foreach (var col in hitColliders)
        {
            if (!col.CompareTag("Player")) continue;
            sum++;
        }
        EvolutionManager.Instance.numOfIndividuals[envNum] = sum;
    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject o = other.gameObject;
        if (!o.CompareTag("Player")) return;
        EvolutionManager.lookUp[o].environment = envNum;
        EvolutionManager.Instance.numOfIndividuals[envNum]++;
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject o = other.gameObject;
        if (!other.gameObject.CompareTag("Player")) return;
        EvolutionManager.Instance.numOfIndividuals[EvolutionManager.lookUp[o].environment]--;
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateIndividualsInside), 5.0f, 20.0f);
    }
}
