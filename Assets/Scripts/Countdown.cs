using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    TextMeshProUGUI countdownText;
    int value = 3;

    private void OnEnable()
    {
        GameManager.OnCountdownStart += Count;
        countdownText = GetComponent<TextMeshProUGUI>();
        countdownText.color = new Color(1, 1, 1, 0);

        Ball.OnHitOpponentWall += DisplayPlayerGoal;
        Ball.OnHitHomeWall += DisplayOpponentGoal;
    }

    private void OnDisable()
    {
        GameManager.OnCountdownStart -= Count;
        Ball.OnHitOpponentWall -= DisplayPlayerGoal;
        Ball.OnHitHomeWall -= DisplayOpponentGoal;
    }

    private void DisplayPlayerGoal()
    {
        countdownText.text = "GOAL";
        countdownText.color = new Color(1, 0, 0, 1);
    }

    private void DisplayOpponentGoal()
    {
        countdownText.text = "ENEMY GOAL";
        countdownText.color = new Color(1, 0, 0, 1);
    }

    private void Count()
    {
        if (value <= 0)
        {
            countdownText.color = new Color(1, 1, 1, 0);
            value = 3;
            return;
        }
        countdownText.color = new Color(1, 1, 1, 1);
        countdownText.text = value.ToString();
        value--;
        Invoke("Count", 1f);
    }


}
