using BubbleDistortionPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Start : MonoBehaviour
{
    private bool _isLevel3 = false;
    private DateTime _lastTrigger;

    private static Level3Start _instance;
    public static Level3Start Instance
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
                OutputLogManager.OutputText("Level 3 triggered by " + other.name);
                StartLevel3();
            }
        }
    }

    public void StartLevel3()
    {
        if (_lastTrigger < DateTime.Now.AddSeconds(-1))
        {
            if (!_isLevel3)
            {
                OutputLogManager.OutputText("Entering Level 3");
                AudioManager.Instance.Level3Triggered();
                _isLevel3 = !_isLevel3;
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
