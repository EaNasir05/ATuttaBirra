using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bottle : MonoBehaviour
{
    [SerializeField] GameObject brokenBottlePrefab;
    public InputActionAsset inputActions;
    private InputAction explodeAction;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        explodeAction = InputSystem.actions.FindAction("Explode");
    }

    void Update() // just for testing
    {
        if(explodeAction.WasPressedThisFrame())
        {
            Explode();
        }
    }
    
    void Explode()
    {
        GameObject brokenBottle = Instantiate(brokenBottlePrefab, this.transform.position, Quaternion.identity);
        brokenBottle.GetComponent<BrokenBottle>().RandomVelocities();
        Destroy(gameObject);
    }
}
