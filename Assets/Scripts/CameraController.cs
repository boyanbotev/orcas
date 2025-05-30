using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineCamera cineCamera;
    [SerializeField] Transform player;
    [SerializeField] CinemachineFollow cameraFollow;

    public void TransitionTo(Vector3 startPos, Vector3 endPos)
    {
        var sequence = DOTween.Sequence();
        transform.position = startPos;
        transform.rotation = player.rotation;
        cineCamera.LookAt = transform.transform;
        cineCamera.Follow = transform.transform;

        var playerRotation = player.DORotate(new Vector3(0, 0, 0), 0.1f).SetEase(Ease.InOutQuad);
        var movt = transform.DOMove(endPos, 1f).SetEase(Ease.InOutQuad);

        var cameraRotation = transform.DORotate(Vector3.forward, 1f)
            .SetEase(Ease.InOutQuad);

        sequence.Append(playerRotation);
        sequence.Join(cameraRotation);
        sequence.Join(movt);

        cineCamera.transform.DORotate(new Vector3(0, 0, 0), 2f).SetEase(Ease.InOutQuad);
    }

    public void TransitionEnd()
    {
        cineCamera.LookAt = player;
        cineCamera.Follow = player;
        //cameraFollow.TrackerSettings.PositionDamping = new Vector3(1, 1, 1);
        //cameraFollow.TrackerSettings.RotationDamping = new Vector3(1, 1, 1);
    }
}
