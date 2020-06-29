using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace BubbleDistortionPhysics
{
    class AudioManager : MonoBehaviour
    {
        public AudioManager()
        {                        
        }

        private static AudioManager _instance;
        public static AudioManager Instance { 
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioManager>();
                }
                return _instance;
            }            
        }

        int freq = 0;
        float[] number = new float[256];
        public float BassLevel;        
        bool isAboveMax = false;

        public DateTime LastBeat { get; set; }        

        private void FixedUpdate()
        {            
            AudioListener.GetSpectrumData(number, 0, FFTWindow.Hamming);
            //OutputLogManager.OutputText((number[freq] * 25f).ToString());
            BassLevel = Math.Max(Math.Min(number[freq] * 25f, 2f), 0.5f);

            if (!isAboveMax && AudioManager.Instance.BassLevel >= 2)
            {
                isAboveMax = true;
                LastBeat = DateTime.Now;
            }
            else if (isAboveMax && BassLevel < 2)
            {
                isAboveMax = false;
            }
        }
    }
}
