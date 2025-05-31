using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.Cinemachine;

public enum RoundConclusion
{
    HomeWallHit,
    OpponentWallHit
}

public class GameManager : MonoBehaviour
{
    public static event Action<RoundConclusion> OnRoundConclusion;
    public static event Action OnRoundStart;
    public static event Action OnCountdownStart;
    [SerializeField] private float rounds;
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] opponents;
    [SerializeField] private Transform ballStartOffense;
    [SerializeField] private Transform ballStartDefense;
    [SerializeField] private Transform ball;
    [SerializeField] CameraController cameraController;
    [Header("Delays")]
    [SerializeField] private float roundStartDelay = 3f;
    [SerializeField] private float roundEndDelay = 1f;
    Vector3 playerStartPosition;
    private List<Vector3> opponentStartPositions;
    [SerializeField] Vector3 opponentOffenseOffset;
    private RoundConclusion lastRoundConclusion;
    private int playerScore;
    private int opponentScore;

    private void Awake()
    {
        opponentStartPositions = new List<Vector3>();
        for (int i = 0; i < opponents.Length; i++)
        {
            if (opponents[i] != null)
            {
                opponentStartPositions.Add(opponents[i].position);
            }
        }

        playerStartPosition = player.position;
    }

    private void OnEnable()
    {
        Ball.OnHitHomeWall += OnHitHomeWall;
        Ball.OnHitOpponentWall += OnHitOpponentWall;
    }

    private void OnDisable()
    {
        Ball.OnHitHomeWall -= OnHitHomeWall;
        Ball.OnHitOpponentWall -= OnHitOpponentWall;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnHitOpponentWall();
        }
    }
    private void OnHitHomeWall()
    {
        Debug.Log("Home wall hit! Rounds remaining: " + rounds);
        lastRoundConclusion = RoundConclusion.HomeWallHit;
        opponentScore++;
        StartCoroutine(StartRoundRoutine());
    }

    private void OnHitOpponentWall()
    {
        Debug.Log("Opponent wall hit! Rounds remaining: " + rounds);
        lastRoundConclusion = RoundConclusion.OpponentWallHit;
        playerScore++;
        StartCoroutine(StartRoundRoutine());
    }

    private void LoadRound()
    {
        OnCountdownStart?.Invoke();
        cameraController.TransitionTo(player.position, playerStartPosition);

        for (int i = 0; i < opponents.Length; i++)
        {
            Vector3 opponentStart = lastRoundConclusion == RoundConclusion.OpponentWallHit
                ? opponentStartPositions[i] + opponentOffenseOffset
                : opponentStartPositions[i];
            opponents[i].position = opponentStart;
            opponents[i].GetComponent<IResetable>().StartRound();
        }

        player.position = playerStartPosition;
        player.GetComponent<IResetable>().StartRound();

        ball.position = lastRoundConclusion == RoundConclusion.HomeWallHit ? ballStartOffense.position : ballStartDefense.position;
        ball.GetComponent<IResetable>().StartRound();
    }

    void Disable()
    {
        ball.GetComponent<IResetable>().SetIdle();
        player.GetComponent<IResetable>().SetIdle();
        foreach (var opponent in opponents)
        {
            opponent.GetComponent<IResetable>().SetIdle();
        }
    }

    void Enable()
    {
        player.GetComponent<IResetable>().SetActive();
        ball.GetComponent<IResetable>().SetActive();
        foreach (var opponent in opponents)
        {
            opponent.GetComponent<IResetable>().SetActive();
        }
    }

    void EndLevel()
    {
        Debug.Log("Game Over! Final Score - Player: " + playerScore + ", Opponents: " + opponentScore);
        Time.timeScale = 0f;
    }

    IEnumerator StartRoundRoutine()
    {
        rounds--;
        OnRoundConclusion?.Invoke(lastRoundConclusion);
        Disable();

        yield return new WaitForSecondsRealtime(roundEndDelay);

        if (rounds > 0) LoadRound();
        else EndLevel();

        yield return new WaitForSecondsRealtime(roundStartDelay);
        OnRoundStart?.Invoke();
        Enable();

        cameraController.TransitionEnd();
    }
}
