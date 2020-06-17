using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class LightStrip : MonoBehaviour
    {        
        private System.Random _random;
        private Light _light;
        private float _lightIntensity;
        private MeshRenderer _meshRenderer;

        public void Awake()
        {            
            _random = new System.Random(Convert.ToInt32(DateTime.Now.Ticks.ToString().Substring(10)));
            _light = GetComponentInChildren<Light>();
            _lightIntensity = _light.intensity;
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material.EnableKeyword("_EMISSION");            

            StartCoroutine(FlickerLight());
        }
    

        public IEnumerator FlickerLight()
        {
            yield return new WaitForSeconds(_random.Next(3, 10));

            _light.intensity = 0;            
            _meshRenderer.forceRenderingOff = true;

            yield return new WaitForSeconds(_random.Next(1, 5) / 10f);

            _meshRenderer.forceRenderingOff = false;
            _light.intensity = _lightIntensity;

            yield return new WaitForSeconds(_random.Next(1, 5) / 10f);

            _light.intensity = 0;
            _meshRenderer.forceRenderingOff = true;

            yield return new WaitForSeconds(_random.Next(1, 5) / 10f);

            _meshRenderer.forceRenderingOff = false;
            _light.intensity = _lightIntensity;

            StartCoroutine(FlickerLight());
        }        
    }
}
