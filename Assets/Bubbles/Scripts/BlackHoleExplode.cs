using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class BlackHoleExplode : MonoBehaviour
    {       
        public void Start()
        {                 
        }

        public void Explode()
        {
            StartCoroutine(explode());
        }

        private IEnumerator explode()
        {
            var t = 0f;
            var explodeTime = 0.25f;
            var contractTime = 0.5f;

            while (t < 1)
            {
                t += Time.deltaTime / explodeTime;

                gameObject.transform.localScale = new Vector3(t, t, t);
                
                yield return null;
            }

            t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / contractTime;

                gameObject.transform.localScale = new Vector3(1 - t, 1- t, 1-t);

                yield return null;
            }
        }

        
    }
}
