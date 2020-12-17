using BubbleDistortionPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHeavenStart : MonoBehaviour
{
    private bool _isLevelHeaven = false;
    private DateTime _lastTrigger;
    public GameObject HeavenAudio;

    private static LevelHeavenStart _instance;
    public static LevelHeavenStart Instance
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
                OutputLogManager.OutputText("Level Heaven triggered by " + other.name);
                StartLevelHeaven();
            }
        }
    }

    public void StartLevelHeaven()
    {
        if (_lastTrigger < DateTime.Now.AddSeconds(-1))
        {
            if (!_isLevelHeaven)
            {
                OutputLogManager.OutputText("Entering Level Heaven");
                HeavenAudio.SetActive(true);
                AudioManager.Instance.LevelHeavenTriggered();
                _isLevelHeaven = !_isLevelHeaven;
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
