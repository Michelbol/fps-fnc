﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetMovimentation : MonoBehaviour
{

    private DateTime StopDate;
    
    public TextMesh TextComponent;

    public bool selected = false;

    public float speed = 1f;

    public string Symbol = "S";
    // Start is called before the first frame update
    void Start()
    {
        TextComponent.text = Symbol;
        StopDate = DateTime.MaxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z >= -10)
        {
            Vector3 vector = transform.position;
            vector.z -= 0.1f * speed;
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
                Vector3 vector = transform.position;
                vector.z = 10;
                transform.position = vector;
                StopDate = DateTime.MaxValue;
            }
        }
    }
}
