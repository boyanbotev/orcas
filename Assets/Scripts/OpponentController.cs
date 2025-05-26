using UnityEngine;
using System.Collections;

enum OpponentState
{
    Navigating,
    Attacking,
}

public class OpponentController : MonoBehaviour
{
    private OrcaState movementState = OrcaState.Swimming;
    private OpponentState currentBehaviour = OpponentState.Navigating;
    private Vector3 targetPos;
    Rigidbody rb;
    [SerializeField] float boostDuration = 0.2f;
    [SerializeField] float boostCooldown = 1f;
    [SerializeField] Transform ball;
    [SerializeField] Vector3 playDirection;
    [SerializeField] float attackDistance = 5f;
    [SerializeField] float moveSpeed;
    [SerializeField] float boostSpeed;

    void Start()
    {
        InvokeRepeating("ChooseBehaviour", 0f, 1f);
        StartCoroutine(BoostTriggerRoutine());
    }

    void Boost()
    {
        movementState = OrcaState.Boosting;
        StartCoroutine(BoostRoutine());
    }

    void ChooseBehaviour()
    {
        if (IsBehindBall())
        {
            currentBehaviour = OpponentState.Attacking;
        }
        else
        {
            currentBehaviour = OpponentState.Navigating;
        }
    }

    bool IsBehindBall()
    {
        Vector3 toBall = ball.position - transform.position;
        Debug.Log($"To Ball: {toBall}, Play Direction: {playDirection}, dot {Vector3.Dot(toBall, playDirection)}");
        return Vector3.Dot(toBall, playDirection) > 0;
    }

    private void FixedUpdate()
    {
        if (currentBehaviour == OpponentState.Attacking)
        {
            targetPos = ball.position;
        }
        else
        {
            targetPos = ball.position - playDirection * attackDistance;
        }

        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        rb = GetComponent<Rigidbody>();

        float speed;
        if (movementState == OrcaState.Boosting)
        {
            speed = boostSpeed;
        }
        else
        {
            speed = moveSpeed;
        }

        rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 2.5f));
    }
    IEnumerator BoostTriggerRoutine()
    {
        Boost();
        var waitTime = GetRandomDelay(0.42f, 0.8f); // was 0.5f, 1f
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(BoostTriggerRoutine());
    }

    float GetRandomDelay(float min, float max)
    {
        return Random.Range(min, max);
    }

    IEnumerator BoostRoutine()
    {
        yield return new WaitForSeconds(boostDuration);
        movementState = OrcaState.Recharging;
        yield return new WaitForSeconds(boostCooldown);
        movementState = OrcaState.Swimming;
    }
}
