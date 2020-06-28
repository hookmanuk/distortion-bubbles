using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class SpeakerPulse : MonoBehaviour
    {
        AudioListener Audio;

        Color colorStart = Color.red;
        Color colorEnd = Color.green;
        float duration = 1.0f;

        public GameObject ball;
        GameObject ball2;
        Light pointLight;        
        int freq = 0;
        float[] number = new float[64];
        float scale;
        int updateEvery = 10;
        int currentFrame = 0;

        private void FixedUpdate()
        {
            currentFrame++;

            //if (currentFrame == updateEvery)
            //{
                AudioListener.GetSpectrumData(number, 0, FFTWindow.Rectangular);
                scale = Math.Max(Math.Min(number[freq] * 40f, 2), 0.5f);
                ball.transform.localScale = new Vector3(scale, scale, scale);

            //    currentFrame = 0;
            //}

            //Camera.main.fieldOfView = 60 + number[0] * 15;
            //pointLight.GetComponent<Light>().intensity = 0.7f + number[freq] * 1.2f;

            //var lerp : float = Mathf.PingPong(number[freq], duration) / duration;
            //ball2.renderer.material.color = Color.Lerp(colorStart, colorEnd, lerp);
        }

    }
}
