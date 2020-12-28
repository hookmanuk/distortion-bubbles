using BubbleDistortionPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Start : MonoBehaviour
{
    private bool _isLevel2 = false;
    private DateTime _lastTrigger;

    private static Level2Start _instance;
    public static Level2Start Instance
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!PlayerController.Instance.IntroStart)
            {
                OutputLogManager.OutputText("Level 2 triggered by " + other.name);
                StartLevel2();
            }
        }
    }

    public void StartLevel2()
    {
        if (_lastTrigger < DateTime.Now.AddSeconds(-1))
        {
            if (!_isLevel2)
            {
                OutputLogManager.OutputText("Entering Level 2");
                //AudioManager.Instance.Level2Triggered();
                _isLevel2 = !_isLevel2;
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
