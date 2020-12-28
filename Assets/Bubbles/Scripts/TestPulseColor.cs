using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class TestPulseColor : MonoBehaviour
    {
        
        private Material _material;
        private bool _isEmitting = false;
        public float EmissiveMultiplier;
        private Color _emissiveColor;
        private Color _standardColor;
        private int _frameCount;

        public void Start()
        {           
            _material = GetComponent<MeshRenderer>().material;
            _emissiveColor = new Color(EmissiveMultiplier, 0, 0);
            _standardColor = new Color(0, 0, 0);
        }

      
    
        //public void FixedUpdate()
        //{
        //    if (_frameCount == 1)
        //    {
        //        _material.SetColor("_EmissiveColor", _emissiveColor * AudioManager.Instance.BassLevel / 2f);
        //        _frameCount = 0;
        //    }
        //    else
        //    {
        //        _frameCount++;
        //    }

        //    //double millisecondsSinceBeat = (DateTime.Now - AudioManager.Instance.LastBeat).TotalMilliseconds;
        //    //if (millisecondsSinceBeat < 100)
        //    //{
        //    //    if (!_isEmitting)
        //    //    {
        //    //        _isEmitting = true;
        //    //        Color color = new Color(0.1f, 0, 0);

        //    //        // for some reason, the desired intensity value (set in the UI slider) needs to be modified slightly for proper internal consumption
        //    //        //float adjustedIntensity = 0.3f;

        //    //        // redefine the color with intensity factored in - this should result in the UI slider matching the desired value
        //    //        //color *= Mathf.Pow(2.0F, adjustedIntensity);
        //    //        _material.SetColor("_EmissiveColor", _emissiveColor);
        //    //        //_material.SetFloat("_EmissionIntensity", 0.1f);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    _isEmitting = false;
        //    //    Color color = new Color(0, 0, 0);
        //    //    _material.SetColor("_EmissiveColor", _standardColor);
        //    //}
        //}

        private void Update()
        {            
        }       
    }
}
