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
               (currentBehaviour == OpponentState.Defending && (!isNearBall || !isWithinLocale));
    }
    protected override Vector3 ChooseTargetPos()
    {
        if (currentBehaviour == OpponentState.Defending)
        {
            var anticipatedPos = GetAnticipatedTarget(ball.position);

            return new Vector3(
                anticipatedPos.x + initialPosition.x,
                anticipatedPos.y,
                initialPosition.z
            );
        }
        else
        {
            return base.ChooseTargetPos();
        }
    }

   protected override void Move(Vector3 targetPos)
{
        if (currentBehaviour == OpponentState.Defending 
            && Vector3.Distance(targetPos, transform.position) < 5
        )
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * turnSpeed));
            return;
        }

        base.Move(targetPos);
   }
}
