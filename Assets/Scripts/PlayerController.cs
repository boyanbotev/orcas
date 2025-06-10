using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

enum OrcaState
{
    Swimming,
    Boosting,
    Recharging,
    Idle
}

public class PlayerController : MonoBehaviour, IResetable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float boostDuration = 0.2f;
    [SerializeField] private float boostCooldown = 1f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float strafeSpeed = 4f;
    [SerializeField] private float verticalStrafeSpeed = 2f;
    [SerializeField] private Vector2 movementInput;
    [SerializeField] private OrcaState currentState = OrcaState.Swimming;
    [SerializeField] private float minXRotation = -50f;
    [SerializeField] private float maxXRotation = 50f;
    [SerializeField] bool torqueRotation = true;

    Rigidbody rb;
    private OrcaAnimation anim;
    private bool isBoostButtonPressed;
    private Coroutine _boostCoroutineReference = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<OrcaAnimation>();
    }

    public void SetIdle()
    {
        currentState = OrcaState.Idle;
        if (_boostCoroutineReference != null)
        {
            StopCoroutine(_boostCoroutineReference);
            _boostCoroutineReference = null;
        }
        isBoostButtonPressed = false;
        rb.isKinematic = true;
    }

    public void SetActive()
    {
        currentState = OrcaState.Swimming;
        rb.isKinematic = false;
    }

    public void StartRound()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    public void Boost(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isBoostButtonPressed = true;

            if (currentState == OrcaState.Swimming && _boostCoroutineReference == null)
            {
                _boostCoroutineReference = StartCoroutine(BoostRoutine());
            }
        }
        else if (context.canceled)
        {
            isBoostButtonPressed = false;
            anim.StopBoost();
        }
    }

    public void SetDir(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (currentState == OrcaState.Idle) return;

        Move();

        if (torqueRotation) AddTorque();
        else Rotate();
    }

    private void Move()
    {
        var speed = currentState == OrcaState.Boosting ? boostSpeed : moveSpeed;
        rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
        rb.AddForce(transform.right * movementInput.x * Time.deltaTime * strafeSpeed);
        rb.AddForce(transform.up * movementInput.y * Time.deltaTime * strafeSpeed);
    }

    void AddTorque()
    {
        var torque = rb.rotation * new Vector3(movementInput.y * turnSpeed * Time.deltaTime, movementInput.x * turnSpeed * Time.deltaTime);
        rb.AddTorque(torque, ForceMode.VelocityChange);

        float xRotation = ClampAngle(rb.rotation.eulerAngles.x, minXRotation, maxXRotation);
        rb.rotation = Quaternion.Euler(xRotation, rb.rotation.eulerAngles.y, 0);
    }

    void Rotate()
    {
        var yRotation = Quaternion.AngleAxis(movementInput.x * turnSpeed, Vector3.up);
        var xRotation = Quaternion.AngleAxis(movementInput.y * turnSpeed, Vector3.right);
        rb.MoveRotation(rb.rotation * xRotation * yRotation);

        float clampedXRotation = ClampAngle(rb.rotation.eulerAngles.x, minXRotation, maxXRotation);
        rb.rotation = Quaternion.Euler(clampedXRotation, rb.rotation.eulerAngles.y, 0);
    }

    float ClampAngle(float val, float min, float max)
    {
        float relRange = (max - min) / 2f;
        float offset = max - relRange;
        float z = ((val + 540) % 360) - 180 - offset;

        if (Mathf.Abs(z) > relRange)
        {
            val = relRange * Mathf.Sign(z) + offset;
        }
        return val;
    }

    IEnumerator BoostRoutine()
    {
        currentState = OrcaState.Boosting;
        anim.StartBoost();
        yield return new WaitForSeconds(boostDuration);

        if (currentState == OrcaState.Boosting)
        {
            currentState = OrcaState.Recharging;
            anim.StopBoost();
            yield return new WaitForSeconds(boostCooldown);
        } else
        {
            anim.StopBoost();
            _boostCoroutineReference = null;
            yield break;
        }

        if (currentState == OrcaState.Recharging)
        {
            if (isBoostButtonPressed)
            {
                _boostCoroutineReference = StartCoroutine(BoostRoutine());
            } else
            {
                currentState = OrcaState.Swimming;
                _boostCoroutineReference = null;
                anim.StopBoost();
            }
        } else
        {
            anim.StopBoost();
            _boostCoroutineReference = null;
        }
    }
}
