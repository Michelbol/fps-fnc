using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AlertController : MonoBehaviour
{

    public float time;

    public float successTime;

    public bool active = false;

    public bool successActive = false;

    [SerializeField] Text alert;

    [SerializeField] Text successAlert;

    // Start is called before the first frame update
    void Start()
    {
        alert.enabled = false;
        successAlert.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (startTimer(3))
            {
                alert.enabled = true;
                return;
            }
            alert.enabled = false;
        }

        if (successActive)
        {
            if (startTimerSuccess(3))
            {
                successAlert.enabled = true;
                return;
            }
            successAlert.enabled = false;
        }
    }

    bool startTimer(int maxSeconds = 0)
    {
        if (maxSeconds > 0 && (time + Time.deltaTime) % 60 > maxSeconds)
        {
            time = 0;
            active = false;
            return false;
        }
        time += Time.deltaTime;
        return true;
    }

    bool startTimerSuccess(int maxSeconds = 0)
    {
        if (maxSeconds > 0 && (successTime + Time.deltaTime) % 60 > maxSeconds)
        {
            successTime = 0;
            successActive = false;
            return false;
        }
        successTime += Time.deltaTime;
        return true;
    }

    public void startTimer(string text)
    {
        alert.text = text;
        active = true;
    }

    public void successMessage(string text)
    {
        successAlert.text = text;
        successActive = true;
    }
}
