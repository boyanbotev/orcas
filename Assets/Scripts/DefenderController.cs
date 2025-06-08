using UnityEngine;

public class DefenderController : OpponentController
{
    [SerializeField] float defendRadius = 10f;
    [SerializeField] float localeRadius = 50f;
    [SerializeField] Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    protected override void ChooseBehaviour()
    {
        if (currentBehaviour == OpponentState.Idle)
            return;

        if (ShouldDefend())
        {
            currentBehaviour = OpponentState.Defending;
        } 
        else
        {
            base.ChooseBehaviour();
        }
    }

    bool ShouldDefend()
    {
        var isNearBall = Vector3.Distance(ball.position, transform.position) < defendRadius;
        var isWithinLocale = Mathf.Abs(initialPosition.z - ball.position.z) < localeRadius;

        return currentBehaviour != OpponentState.Defending && !isWithinLocale ||
               (currentBehaviour == OpponentState.Defending && !isNearBall);
    }
    protected override Vector3 ChooseTargetPos()
    {
        if (currentBehaviour == OpponentState.Defending)
        {
            return new Vector3(
                ball.position.x + initialPosition.x,
                ball.position.y,
                initialPosition.z
            );
        }
        else
        {
            return base.ChooseTargetPos();
        }
    }
}
