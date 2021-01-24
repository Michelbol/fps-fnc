using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetMovimentation : MonoBehaviour
{

    private DateTime StopDate;
    // Start is called before the first frame update
    void Start()
    {
        StopDate = DateTime.MaxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z >= -10)
        {
            Vector3 vector = transform.position;
            vector.z -= 0.1f;
            transform.position = vector;
        }
        else
        {
            if(StopDate == DateTime.MaxValue)
            {
                StopDate = DateTime.Now;
            }

            if (DateTime.Now.CompareTo(StopDate.AddSeconds(3)) > 0)
            {
                Debug.Log("Reiniciar!");
                Vector3 vector = transform.position;
                vector.z = 10;
                transform.position = vector;
                StopDate = DateTime.MaxValue;
            }
        }
    }
}
