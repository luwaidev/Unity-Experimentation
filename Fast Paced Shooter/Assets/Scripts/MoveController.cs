using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator anim;
    public Transform cam;

    public enum State { Idle, Running, Dash, InAir, Jumping, Slide, Dead } // Player movement state
    [Header("State ")]
    public State state;
    private Vector2 input;
    public Vector3 vel;

    [Header("View Settings")]
    public Vector2 sensModifier;
    [SerializeField] float mouseSens;

    [Space(10)]
    [Header("Movement Settings")]
    [Space(10)]
    [Header("Running and Jumping")]
    [SerializeField] float movmentSpeed;
    [SerializeField] float jumpForce;

    [Header("Sliding and Dashing")]
    bool sliding = false;
    [SerializeField] float slideForce;
    [SerializeField] float dashForce;
    [SerializeField] float dashTime;


    ////////// INPUTS //////////

    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        // Handle look input
        Vector2 lookInput = value.Get<Vector2>() * mouseSens * Time.deltaTime * sensModifier;
        print(cam.transform.localEulerAngles.x - lookInput.y * mouseSens * Time.deltaTime);
        lookInput.y = Mathf.Clamp(cam.transform.localEulerAngles.x - lookInput.y * mouseSens * Time.deltaTime, -90f, 90f);

        cam.transform.localEulerAngles = new Vector3(lookInput.y, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x);
    }

    void OnJump(InputValue value)
    {

    }

    void OnCrouch(InputValue value)
    {
        sliding = !sliding;
    }

    void OnDash(InputValue value)
    {
        state = State.Dash;
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

    IEnumerator IdleState()
    {
        // Enter State
        while (state == State.Idle)
        {
            // chill
            if (input != Vector2.zero)
            {
                state = State.Running;
                break;
            }
            if (sliding && IsGrounded()
            {
                state = State.Slide;
                break;
            }
            yield return null;
            SetVelocity();
        }
        // Exit State
        NextState();
    }

    IEnumerator RunningState()
    {
        // Enter State
        while (state == State.Running)
        {
            vel.x = input.x * movmentSpeed;
            vel.y = input.y * movmentSpeed;
            // Check States
            if (input == Vector2.zero)
            {
                state = State.Idle;
                break;
            }
            if (sliding && IsGrounded())
            {
                state = State.Slide;
                break;
            }
            yield return null;
            SetVelocity();
        }

        NextState();
    }

    IEnumerator DashState()
    {
        // Set Velocity
        vel.x = input.x * dashForce;

        // Wait dash time
        yield return new WaitForSeconds(dashTime);

        state = State.Idle;

        NextState();
    }

    IEnumerator SlideState()
    {
        // Set Velocity
        vel.x = input.x * slideForce;

        state = State.Idle;

        // Hold until let go of input
        while (sliding)
        {
            vel.x = input.x * movmentSpeed;
            vel.y = input.y * movmentSpeed;
            // Check States
            if (input == Vector2.zero || !sliding)
            {
                state = State.Idle;
                break;
            }
            yield return null;
            SetVelocity();
        }

        NextState();
    }
    ////////// UNITY FUNCTIONS //////////
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        NextState();
    }

    // Update is called once per frame
    void Update()
    {
    }

    ////////// Functions //////////
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    void SetVelocity()
    {
        rb.velocity = transform.forward * vel.y + transform.right * vel.x;
    }

}
