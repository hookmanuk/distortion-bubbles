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
        public bool Flickers = true;
        public bool AlwaysOn;

        public void Start()
        {
            Color lightColor;

            PhysicsManager.Instance.LightStrips.Add(this);           
            _random = new System.Random(Convert.ToInt32(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)));
            _light = GetComponentInChildren<Light>();
            _lightIntensity = _light.intensity;
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material.EnableKeyword("_EMISSION");

            if (Flickers)
            {
                StartCoroutine(FlickerLight());
            }            
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
