using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class Menu : MonoBehaviour
    {                        
        public VendingButton GraphicsButton;
        public VendingButton ResButton;
        public VendingButton BackButton;
        public SimpleHelvetica GraphicsInfo;
        public SimpleHelvetica ResInfo;
        public SimpleHelvetica FPSInfo;
        public SimpleHelvetica DynamicResInfo;

        private int _frameUpdate = 90;
        private int _currentFrame = 0;

        public void CycleSettings()
        {
            PlayerController.Instance.CycleGraphicsQuality();
            GraphicsInfo.Text = PlayerController.Instance.GraphicsQuality.Name + " Quality";
            GraphicsInfo.GenerateText();
        }

        public void ToggleDynamicResolution()
        {
            DynamicResolution.DynamicResolutionEnabled = !DynamicResolution.DynamicResolutionEnabled;
            ResInfo.Text = (DynamicResolution.DynamicResolutionEnabled ? "Enabled" : "Disabled");
            ResInfo.GenerateText();
        }

        private void FixedUpdate()
        {
            _currentFrame++;

            if (_currentFrame == _frameUpdate)
            {
                FPSInfo.Text = DynamicResolution.ReportedFPS.ToString();
                FPSInfo.GenerateText();

                DynamicResInfo.Text = (DynamicResolution.DynamicResolutionEnabled ? DynamicResolution.ResScale.ToString() : "1");
                DynamicResInfo.GenerateText();

                _currentFrame = 0;
            }
        }
    }
}
