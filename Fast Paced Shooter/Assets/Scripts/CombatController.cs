using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator anim;

    public enum State {Idle, Shooting, Reloading} // Player combat states
    [Header("State ")]
    public State state;

    [Header("Movement Settings")]
    [SerializeField] float movmentSpeed;
    [SerializeField] float slideForce;
    [SerializeField] float jumpForce;


    ////////// INPUTS //////////
    void OnFire(InputValue value){

    }

    void OnReload(InputValue value){

    }

    ////////// STATES //////////
    // Takes the current state, and calls the Coroutine for that state
    public void NextState()
    {
        // Get state name
        string methodName = state.ToString() + "State";

        // Get method
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);

        StartCoroutine((IEnumerator)info.Invoke(this, null)); // Call the next state
    }
    
    IEnumerator IdleMState(){
        yield return null;
    }

    ////////// UNITY FUNCTIONS //////////
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
