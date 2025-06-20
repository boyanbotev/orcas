using UnityEngine;
using System.Collections;

public enum OpponentState
{
    Idle,
    Navigating,
    Attacking,
    Defending,
}

public class OpponentController : MonoBehaviour, IResetable
{
    private OrcaState movementState = OrcaState.Swimming;
    protected OpponentState currentBehaviour = OpponentState.Navigating;
    protected Vector3 targetPos;
    protected Rigidbody rb;
    protected Rigidbody ballRb;
    [SerializeField] float boostDuration = 0.2f;
    [SerializeField] float boostCooldown = 1f;
    [SerializeField] protected Transform ball;
    [SerializeField] protected Vector3 playDirection;
    [SerializeField] float attackDistance = 5f;
    [SerializeField] float moveSpeed;
    [Tooltip("This makes a large difference to how difficult the enemy feels to play against")]
    [SerializeField] float boostSpeed;
    [SerializeField] protected float turnSpeed = 2.5f;
    [SerializeField] float minBoostDelay = 0.42f;
    [SerializeField] float maxBoostDelay = 0.8f;
    [SerializeField] float decisionInterval = 1f;
    [Tooltip("The amount which the ai tries to avoid hitting the ball as it navigates to be behind it")]
    [SerializeField] float avoidOffsetScale = 10f;
    [Tooltip("How much to anticipate where the ball will be based on its movement")]
    [SerializeField] float anticipateTargetAmount = 0;
    OrcaAnimation anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRb = ball.GetComponent<Rigidbody>();
        InvokeRepeating("ChooseBehaviour", 0f, decisionInterval);
        StartCoroutine(BoostTriggerRoutine());
        anim = GetComponentInChildren<OrcaAnimation>();
    }

    public void StartRound()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(playDirection);
    }

    public void SetIdle()
    {
        currentBehaviour = OpponentState.Idle;
        movementState = OrcaState.Idle;
        rb.isKinematic = true;
    }

    public void SetActive()
    {
        currentBehaviour = OpponentState.Navigating;
        movementState = OrcaState.Swimming;
        rb.isKinematic = false;
    }

    void Boost()
    {
        if (movementState != OrcaState.Swimming)
            return;

        movementState = OrcaState.Boosting;
        StartCoroutine(BoostRoutine());
    }

    protected virtual void ChooseBehaviour()
    {
        if (currentBehaviour == OpponentState.Idle)
            return;

        currentBehaviour = IsBehindBall() ? OpponentState.Attacking : OpponentState.Navigating;
    }

    bool IsBehindBall()
    {
        Vector3 toBall = ball.position - transform.position;
        return Vector3.Dot(toBall, playDirection) > 0;
    }

    private void FixedUpdate()
    {
        if (currentBehaviour == OpponentState.Idle)
            return;

        targetPos = ChooseTargetPos();
        Move(targetPos);
    }

    protected virtual void Move(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float speed = movementState == OrcaState.Boosting ? boostSpeed : moveSpeed;

        rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * turnSpeed));
    }

    protected virtual Vector3 ChooseTargetPos()
    {
        if (currentBehaviour == OpponentState.Attacking)
        {
            return GetAnticipatedTarget(ball.position);
        }
        else
        {
            var posBehindBall = ball.position - playDirection * attackDistance;
            return GetOffsetTarget(transform.position, ball.position, posBehindBall);
        }
    }

    /// <summary>
    /// Offset the position behind the ball to be on the side of the ball the opponent is already on
    /// to avoid collision with the ball as the opponent tries to get behind it.
    /// </summary>
    Vector3 GetOffsetTarget(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ac = c - a;
        Vector3 bc = c - b;
        Vector3 ab = b - a;

        float t = Vector3.Dot(bc, ac) / Vector3.Dot(ac, ac);
        Vector3 d = a + t * ac;

        Vector3 bd = d - b;

        var parallelismMultiplier = (Vector3.Dot(ac.normalized, ab.normalized) + 1) / 2;
        var offset = bd.normalized * avoidOffsetScale * parallelismMultiplier;

        return c + offset;
    }

    protected Vector3 GetAnticipatedTarget(Vector3 targetPos)
    {
        Vector3 projectedBallMovement = ballRb.linearVelocity * Vector3.Distance(ball.position, transform.position) * anticipateTargetAmount;
        return targetPos + projectedBallMovement;
    }


IEnumerator BoostTriggerRoutine()
    {
        Boost();
        var waitTime = IsNearTarget() ? maxBoostDelay : minBoostDelay;
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(BoostTriggerRoutine());
    }

    bool IsNearTarget()
    {
        return Vector3.Distance(transform.position, targetPos) < attackDistance;
    }

    IEnumerator BoostRoutine()
    {
        anim?.StartBoost();
        yield return new WaitForSeconds(boostDuration);
        anim?.StopBoost();
        if (movementState != OrcaState.Boosting)
            yield break;

        movementState = OrcaState.Recharging;
        yield return new WaitForSeconds(boostCooldown);
        if (movementState == OrcaState.Recharging)
            movementState = OrcaState.Swimming;
    }
}
