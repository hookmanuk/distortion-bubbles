using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace BubbleDistortionPhysics
{
    class AudioManager : MonoBehaviour
    {
        private AudioClip _currentAudioClip;
        public List<AudioClip> _nextAudioClips = new List<AudioClip>();
        public AudioSource PlayerAudioSource;
        private int _nextIndex = 0;

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

        private void FixedUpdate()
        {            
            if (!PlayerAudioSource.isPlaying)
            {
                if (_nextAudioClips.Count > 0)
                {
                    if (_nextAudioClips.Count > _nextIndex + 1)
                    {
                        _nextIndex++;
                    }
                    PlayerAudioSource.PlayOneShot(_nextAudioClips[_nextIndex]);
                }
                else if (_currentAudioClip != null)
                {
                    PlayerAudioSource.PlayOneShot(_currentAudioClip);
                }
            }
        }

        public void SetAudioClip(string strAudioClip)
        {
            AudioClip audioClip = Resources.Load<AudioClip>("Music/" + strAudioClip);

            SetAudioClip(audioClip);
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            if (audioClip != null)
            {
                if (_currentAudioClip == null)
                {
                    _currentAudioClip = audioClip;
                    PlayerAudioSource.PlayOneShot(_currentAudioClip);
                }
                else
                {
                    _nextAudioClips.Add(audioClip);
                }
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
