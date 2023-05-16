using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI : MonoBehaviour
{
    private float curTime = 0;
    public static bool timerOn = false;
    public static int targetsLeft = 10;
    private int prevTargetsLeft;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI finishText;

    // Start is called before the first frame update
    void Start()
    {   
        finishText.enabled = false;
        timerText.enabled = false;
        prevTargetsLeft = targetsLeft;
        
        UpdateTargets();
    }

    // Update is called once per frame
    void Update() {
        if (timerOn) {
            updateTimer();

            if (targetsLeft < prevTargetsLeft) {
                UpdateTargets();
                prevTargetsLeft = targetsLeft;
            }

            // when game is finished remove all UI and display a finish text
            if (targetsLeft == 0) {
                timerOn = false;
                targetText.enabled = false;
                //timerText.enabled = false;

                float minutes = Mathf.FloorToInt(curTime / 60);
                float seconds = Mathf.FloorToInt(curTime % 60);

                finishText.text = string.Format("Finished in\n{0:00}:{1:00}", minutes, seconds);
                finishText.enabled = true;
            }
        }
    }

    // updates the timer
    void updateTimer() {
        curTime += Time.deltaTime;

        //float minutes = Mathf.FloorToInt(curTime / 60);
        //float seconds = Mathf.FloorToInt(curTime % 60);
        //timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    // displays the current number of targets
    void UpdateTargets() {
        targetText.text = "Targets Left:\n" + targetsLeft.ToString();
    }
}
