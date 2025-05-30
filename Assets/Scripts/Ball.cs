using System;
using UnityEngine;

public enum BallState
{
    Inactive,
    Active
}
public class Ball : MonoBehaviour, IResetable
{
    public static event Action OnHitHomeWall;
    public static event Action OnHitOpponentWall;
    BallState currentState = BallState.Active;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == BallState.Inactive)
            return;

        if (collision.gameObject.CompareTag("Home Wall"))
        {
            OnHitHomeWall?.Invoke();
        }
        else if (collision.gameObject.CompareTag("Opponent Home Wall"))
        {
            OnHitOpponentWall?.Invoke();
        }
    }

    public void SetIdle()
    {
        currentState = BallState.Inactive;
    }

    public void SetActive()
    {
        currentState = BallState.Active;
    }

    public void StartRound()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
