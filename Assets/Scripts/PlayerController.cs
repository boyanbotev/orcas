using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

enum OrcaState
{
    Swimming,
    Boosting,
    Recharging
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float boostDuration = 0.2f;
    [SerializeField] private float boostCooldown = 1f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private Vector2 movementInput;
    [SerializeField] private OrcaState currentState = OrcaState.Swimming;
    [SerializeField] private float minXRotation = -50f;
    [SerializeField] private float maxXRotation = 50f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Boost(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed || currentState != OrcaState.Swimming)
            return;

        currentState = OrcaState.Boosting;
        StartCoroutine(BoostRoutine());
    }

    public void SetDir(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        var speed = currentState == OrcaState.Boosting ? boostSpeed : moveSpeed;
        //var targetRotation = rb.rotation * Quaternion.Euler(movementInput.y * turnSpeed * Time.deltaTime, movementInput.x * turnSpeed * Time.deltaTime, 0);
        //rb.MoveRotation(targetRotation);
        var torque = rb.rotation * new Vector3(movementInput.y * turnSpeed * Time.deltaTime, movementInput.x * turnSpeed * Time.deltaTime);
        rb.AddTorque(torque, ForceMode.VelocityChange);
        rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);

        float xRotation = ClampAngle(rb.rotation.eulerAngles.x, minXRotation, maxXRotation);
        rb.rotation = Quaternion.Euler(xRotation, rb.rotation.eulerAngles.y, 0);
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
        yield return new WaitForSeconds(boostDuration);
        currentState = OrcaState.Recharging;
        yield return new WaitForSeconds(boostCooldown);
        currentState = OrcaState.Swimming;
    }
}
