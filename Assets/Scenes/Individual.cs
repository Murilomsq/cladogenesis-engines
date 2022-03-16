using System;
using System.Collections;
using System.Collections.Generic;
using NeuralNetwork;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class Individual : MonoBehaviour
{
    private const int IDLE = 0;
    private const int WANDERING = 1;
    private const int DOM_MATING = 2;
    private const int SUB_MATING = 3;
    
    public int environment;
    public int state;
    
    //Refs
    [SerializeField] private CharacterController charController;
    [SerializeField] private Transform groundCheckT;
    [SerializeField] private GameObject childrenPrefab;
    [SerializeField] private Transform childSpawn;
    [SerializeField] private Transform goTransform;
    [SerializeField] private GameObject breedingObject;

    //Genetic identity 
    [SerializeField] private bool isFirstGen;
    [SerializeField] private bool randVal;
    [SerializeField] private float initialVal;

    [SerializeField] private float identity;
    [SerializeField] private float speed;
    private Color color;
    
    // Variables
    [Header("Character movement Variables")]
    [SerializeField] private float gravityMultiplier = 1.0f;
    
    private float gravity = 0.0f;
    private float deathTimer = 0.0f;
    private float mateCooldown = 0.0f;

    [ContextMenu("Step")]
    public void StartStep()
    {
        StartCoroutine(Walk());
    }
    
    public IEnumerator Walk()
    {
        //Checks if the individual can walk (is idle)
        if (state != IDLE) 
            yield break;
        
        state = WANDERING;
        
        Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized;
        for (float i = 0; i < 1.0f; i += Time.deltaTime)
        {
            charController.Move(Time.deltaTime * dir * speed/5);
            yield return null;
        }
        state = IDLE;
    }

    public IEnumerator ReproduceDom(float childIdentity)
    {
        EvolutionManager.Instance.numOfIndividuals[environment]++;
        yield return new WaitForSeconds(EvolutionManager.Instance.matingDuration);
        Individual child = EvolutionManager.lookUp[Instantiate(childrenPrefab, childSpawn.position, Quaternion.identity)];
        child.identity = childIdentity;
        child.BornIndividual();
        state = IDLE;
    }

    public IEnumerator ReproduceSub()
    {
        state = SUB_MATING;
        yield return new WaitForSeconds(EvolutionManager.Instance.matingDuration);
        state = IDLE;
    }

    private void BornIndividual()
    {
        speed = (identity / 10) + 1;
        
        color = new Color(identity/1000, 1 - identity/1000, 0.5f - identity/1000, 1);
        Renderer indRenderer = GetComponent<Renderer>();
        indRenderer.material.color = color;
        state = IDLE;
    }

    private void Awake()
    {
        EvolutionManager.lookUp.Add(gameObject, this);
        EvolutionManager.Instance.onScriptDisable += BuggedDisable;
    }

    private void Start()
    {
        goTransform = transform;

        if (!isFirstGen) return;

        identity = randVal ? Random.Range(0.1f, 1000.0f) : initialVal;

        speed = (identity / 10) + 1;
    
        color = new Color(identity/1000, 1 - identity/1000, 0.5f - identity/1000, 1);
        Renderer indRenderer = GetComponent<Renderer>();
        indRenderer.material.color = color;
        state = IDLE;
    }

    private void OnTriggerEnter(Collider other)
    {
        // checking for cooldown
        if (mateCooldown < EvolutionManager.Instance.matingCooldown) return;
        
        // checking for max num of individuals per habitat
        if (EvolutionManager.Instance.numOfIndividuals[environment] >= EvolutionManager.Instance.maxIndividualsPerEnv) return;
        
        GameObject mate = other.gameObject;
        if (!mate.CompareTag("BreedingRange")) return;
        
        // checking for self breeding
        if (other.gameObject == breedingObject) return;
        
        Individual mateIndividual = EvolutionManager.lookUp[mate.transform.parent.gameObject]; // too much overhead on this
        // Too Different populations cant breed
        if (Math.Abs(identity - mateIndividual.identity) >= 75.0f) return;
        
        mateCooldown = 0.0f;
        
        if (state == WANDERING && mateIndividual.state == WANDERING)
        {
            
            //Calculating child identity
            float childVal = (identity + mateIndividual.identity)/2.0f;
            childVal += EvolutionManager.Instance.mutationRate * Random.Range(-1.0f,1.0f);
            childVal = Mathf.Clamp(childVal, 0.1f, 1000.0f);

            StopAllCoroutines();
            state = DOM_MATING;
            mateIndividual.state = SUB_MATING;
            StartCoroutine(ReproduceDom(childVal));
            StartCoroutine(mateIndividual.ReproduceSub());
        }
    }
    private void OnDestroy()
    {
        EvolutionManager.Instance.numOfIndividuals[environment]--;
        EvolutionManager.Instance.onScriptDisable -= BuggedDisable;
    }

    public void BuggedDisable()
    {
        if (this.enabled == false)
        {
            EvolutionManager.Instance.numOfIndividuals[environment]--;
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(state != WANDERING) return;
    }

    private void Update()
    {
        mateCooldown += Time.deltaTime;
        deathTimer += Time.deltaTime;
        
        if(deathTimer >= EvolutionManager.Instance.individualLifetime)
            Destroy(gameObject);
        
        if(goTransform.position.y <= -5) Destroy(gameObject);
        // StateMachine stuff
        // Im not really using any kind of refence to build this statemachine, im just throwing out the way I think it should work
        StartStep();

        //Gravity stuff
        bool groundCheck = Physics.CheckSphere(groundCheckT.position, 0.1f, 1 << 6); // Terrain mask = 6
        gravity -= 9.81f * gravityMultiplier * Time.deltaTime;
        charController.Move(new Vector3(0, gravity, 0)*Time.deltaTime );
        if ( groundCheck ) gravity = 0;
        
    }
}
