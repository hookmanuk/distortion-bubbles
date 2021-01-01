using BubbleDistortionPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Start : MonoBehaviour
{
    private bool _isLevel1 = false;
    private DateTime _lastTrigger;

    private static Level1Start _instance;
    public static Level1Start Instance
    {
        get
        {           
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel1()
    {
        if (_lastTrigger < DateTime.Now.AddSeconds(-1))
        {
            if (!_isLevel1)
            {
                OutputLogManager.OutputText("Entering Level 1");
                AudioManager.Instance.Level1Triggered();
                _isLevel1 = !_isLevel1;
                _lastTrigger = DateTime.Now;
            }
            else
            {
                //OutputLogManager.OutputText("Entering Level 1");
                //AudioManager.Instance.Level1Triggered();
            }
        }        
    }
}
