using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AlertController : MonoBehaviour
{

    public float time;

    public bool active = false;

    [SerializeField] Text alert;

    // Start is called before the first frame update
    void Start()
    {
        alert.enabled = false;
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

    public void startTimer(string text)
    {
        alert.text = text;
        active = true;
    }
}
