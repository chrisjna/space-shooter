using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float maxTime = 5f;
    float timeLeft;
    Image timerBar;
    // Start is called before the first frame update
    void Start()
    {
        timerBar = GetComponent<Image>();
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        } else
        {
            timeLeft = 5f;
            timerBar.fillAmount = maxTime;
        }
    }
}
