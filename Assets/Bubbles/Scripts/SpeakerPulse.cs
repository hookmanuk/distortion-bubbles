using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class SpeakerPulse : MonoBehaviour
    {
        public GameObject[] ball;
        bool isAboveMax = false;

        public DateTime LastBeat { get; set; }

        private void FixedUpdate()
        {
            if (!isAboveMax && AudioManager.Instance.BassLevel >= 2)
            {
                StartCoroutine(ScaleOverTime(0.04f));
                isAboveMax = true;                
            }
            else if (isAboveMax && AudioManager.Instance.BassLevel < 2)
            {
                if ((DateTime.Now - AudioManager.Instance.LastBeat).TotalMilliseconds > 100)
                {
                    isAboveMax = false;
                    ball[0].transform.localScale = new Vector3(1f, 1f, 1f);
                    ball[1].transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }

        IEnumerator ScaleOverTime(float time)
        {
            Vector3 originalScale = ball[0].transform.localScale;
            Vector3 destinationScale = new Vector3(1.7f, 1.7f, 1.7f);

            float currentTime = 0.0f;

            do
            {
                ball[0].transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
                ball[1].transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= time);
        }
    }
}
