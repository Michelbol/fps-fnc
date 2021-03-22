using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Utils
{
    private float time;

    public bool startTimer(GameObject[] ObjectstoActive, GameObject[] ObjectstoInactive, GameObject Timer, int maxSeconds = 0)
    {

        if (maxSeconds > 0 && (time + Time.deltaTime) % 60 > maxSeconds)
        {

            activeGameObjects(ObjectstoActive);
            inactiveGameObjects(ObjectstoInactive);
            Timer.SetActive(false);
            time = 0;
            return false;
        }

        inactiveGameObjects(ObjectstoActive);
        inactiveGameObjects(ObjectstoInactive);
        Timer.SetActive(true);
        time += Time.deltaTime;

        var minutes = time / 60;
        var seconds = time % 60;
        var fraction = (time * 100) % 1000;
        Timer.GetComponent<Text>().text = $"{Math.Truncate(minutes)}: {Math.Truncate(seconds)}: {Math.Truncate(fraction)}";
        return true;
    }


    static void activeGameObjects(GameObject[] ObjectstoActive)
    {
        foreach (GameObject ObjectToActive in ObjectstoActive)
        {
            ObjectToActive.SetActive(true);
        }
    }

    static void inactiveGameObjects(GameObject[] Objectstoinactive)
    {
        foreach (GameObject ObjectToActive in Objectstoinactive)
        {
            ObjectToActive.SetActive(false);
        }
    }
    public bool setTimeOut(int maxSeconds = 0)
    {
        if (maxSeconds > 0 && (time + Time.deltaTime) % 60 > maxSeconds)
        {
            time = 0;
            return false;
        }
        time += Time.deltaTime;
        return true;
    }

}
