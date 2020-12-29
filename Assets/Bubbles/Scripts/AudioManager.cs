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
        public List<AudioClip> Level1Music;
        public List<AudioClip> Level2Music;
        public List<AudioClip> Level3Music;
        public int Level1PuzzleCount;
        public int Level2PuzzleCount;
        public int Level3PuzzleCount;

        public DateTime LastBeat { get; set; }

        private int _nextTrackIndex = -1;
        private List<AudioSource> _currentSpeakers { get; set; } = new List<AudioSource>();
        private List<AudioClip> _currentMusic { get; set; } = new List<AudioClip>();

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

            bool blnIncremented = false;
            
            foreach (var item in _currentSpeakers)
            {
                if (!item.isPlaying)
                {
                    if (!blnIncremented)
                    {
                        _nextTrackIndex++;
                        if (_nextTrackIndex >= _currentMusic.Count)
                        {
                            _nextTrackIndex = 0;
                        }
                        blnIncremented = true;
                    }
                    //Debug.Log("playing track " + _nextTrackIndex.ToString() + " " + _currentMusic[_nextTrackIndex].name);
                    //start next track   
                    item.PlayOneShot(_currentMusic[_nextTrackIndex]);
                }
            }
            
        }

        private void SetCurrentAudioSources(GameObject[] speakers, List<AudioClip> currentMusic)
        {
            _currentMusic = currentMusic;
            _currentSpeakers.Clear();
            foreach (var item in speakers)
            {
                _currentSpeakers.Add(item.GetComponent<AudioSource>());
            }
            _nextTrackIndex = -1;
            //get track to play when switching level, could be a resume half way through level
            if (PlayerController.Instance.CurrentVendingMachine.Order <= Level1PuzzleCount)
            {
                _nextTrackIndex = (int)Math.Round((float)PlayerController.Instance.CurrentVendingMachine.Order / (float)Level1PuzzleCount * Level1Music.Count, 0) - 1;
            }
            else if (PlayerController.Instance.CurrentVendingMachine.Order <= Level1PuzzleCount + Level2PuzzleCount)
            {
                _nextTrackIndex = (int)Math.Round((float)PlayerController.Instance.CurrentVendingMachine.Order / (float)(Level1PuzzleCount + Level2PuzzleCount) * Level2Music.Count, 0) - 1;
            }
            else if (PlayerController.Instance.CurrentVendingMachine.Order <= Level1PuzzleCount + Level2PuzzleCount + Level3PuzzleCount)
            {
                _nextTrackIndex = (int)Math.Round((float)PlayerController.Instance.CurrentVendingMachine.Order / (float)(Level1PuzzleCount + Level2PuzzleCount + Level3PuzzleCount) * Level3Music.Count, 0) - 1;
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
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0.1f));
            }
            SetCurrentAudioSources(Level3Speakers, Level3Music);
        }

        public void LevelHeavenTriggered()
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
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
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
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0.1f));
            }
            foreach (var item in Level3Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }
            SetCurrentAudioSources(Level2Speakers, Level2Music);
        }

        public void Level1Triggered()
        {
            foreach (var item in Level1Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0.1f));
            }
            foreach (var item in Level2Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }            
            foreach (var item in Level3Speakers)
            {
                StartCoroutine(VolumeOverTime(item.GetComponent<AudioSource>(), 4f, 0));
            }

            SetCurrentAudioSources(Level1Speakers, Level1Music);
        }

        public IEnumerator VolumeOverTime(AudioSource source, float time, float volume)
        {
            float currentTime = 0f;
            float originalVolume = source.volume;
            float differenceVolume = (volume - originalVolume);

            //if (!source.isPlaying && volume > 0)
            //{
            //    source.Play();
            //}

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
