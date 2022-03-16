using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    #region Singleton

    private static EvolutionManager _instance;

    public static EvolutionManager Instance { get { return _instance; } }
    
    private void Awake()
    {
        Application.targetFrameRate = 30;
        
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    #endregion

    // using a hash table to avoid GetComponentCalls 
    public static Dictionary<GameObject, Individual> lookUp = new Dictionary<GameObject, Individual>();

    public Action onScriptDisable;
    
    public int[] numOfIndividuals = new int[2];
    public int maxIndividualsPerEnv;

    [Header("Evolution variables")]
    public float matingCooldown =  10.0f;
    public float matingDuration = 5.0f;
    [Range(0,200.0f)]
    public float mutationRate;
    public float individualLifetime = 10.0f;

    [Space(50)]
    [Range(0.0f, 200.0f)]
    public float timeMultiplier = 1.0f;

    public void CleanGlitchedObjects()
    {
        onScriptDisable.Invoke();
    }
    private void Start()
    {
        InvokeRepeating(nameof(CleanGlitchedObjects), 30.0f, 30.0f);
    }
}
