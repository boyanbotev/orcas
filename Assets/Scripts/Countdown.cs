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
    }

    private void OnDisable()
    {
        GameManager.OnCountdownStart -= Count;
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
