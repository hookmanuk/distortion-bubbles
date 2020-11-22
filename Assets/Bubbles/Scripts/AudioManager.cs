using System;
using System.Collections;
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
        public GameObject[] Level1Speakers;
        public GameObject[] Level2Speakers;
        public GameObject[] Level3Speakers;

        public DateTime LastBeat { get; set; }        

        private void FixedUpdate()
        {            
            AudioListener.GetSpectrumData(number, 0, FFTWindow.Hamming);
            //OutputLogManager.OutputText((number[freq] * 25f).ToString());
            BassLevel = Math.Max(Math.Min(number[freq] * 28f, 2f), 0.5f);

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

        public void Level3Triggered()
        {
            foreach (var item in Level1Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }
            foreach (var item in Level2Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }
            foreach (var item in Level3Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0.2f));
            }
        }

        public void Level2Triggered()
        {
            foreach (var item in Level1Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));                
            }
            foreach (var item in Level2Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0.2f));
            }
            foreach (var item in Level3Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }
        }

        public void Level1Triggered()
        {
            foreach (var item in Level1Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0.2f));
            }
            foreach (var item in Level2Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }            
            foreach (var item in Level3Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }
        }

        public IEnumerator VolumeOverTime(AudioSource source, float time, float volume)
        {
            float currentTime = 0f;
            float originalVolume = source.volume;
            float differenceVolume = (volume - originalVolume);

            if (!source.isPlaying && volume > 0)
            {
                source.Play();
            }

            if (!(source.volume == 0 && volume == 0))
            {
                while (currentTime < time)
                {
                    source.volume = originalVolume + (differenceVolume * currentTime / time);

                    currentTime += Time.deltaTime;
                    yield return null;
                }
            }

            if (source.volume == 0)
            {
                source.Stop();
            }
        }
    }
}
