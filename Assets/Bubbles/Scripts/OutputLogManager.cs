using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

namespace BubbleDistortionPhysics
{
    class OutputLogManager : MonoBehaviour
    {
        public bool EnableLogText;
        public bool EnableLogPerformance;
        private static string _LogText = "";
        private static string _LogPerformance = "";
        private DateTime _lastToggle;

        public OutputLogManager()
        {            
        }

        private void Awake()
        {
            _instance = this;
            HMDLog = _instance.GetComponent<SimpleHelvetica>();
            HMDLog.Text = "";
            HMDLog.GenerateText();
        }

        public static SimpleHelvetica HMDLog;

        private static OutputLogManager _instance;
        public static OutputLogManager Instance { 
            get
            {                
                return _instance;
            }            
        }

        private void Update()
        {
            Vector2 currentState;

            if (PlayerController.Instance.RightController.inputDevice.TryGetFeatureValue(CommonUsages.secondary2DAxis, out currentState))
            {
                if ((DateTime.Now - _lastToggle).TotalMilliseconds > 1500 && currentState.magnitude > 0.8)
                {
                    _lastToggle = DateTime.Now;
                    if (currentState.y > 0.8)
                    {
                        EnableLogText = !EnableLogText;

                        if (!EnableLogText)
                        {
                            _LogText = "";                            
                        }
                        else
                        {
                            _LogText = "Logging text enabled";
                        }
                        HMDLog.Text = _LogPerformance + Environment.NewLine + _LogText;
                        HMDLog.Text = HMDLog.Text.Replace("\r", "");
                        HMDLog.GenerateText();
                    }
                    if (currentState.x > 0.8)
                    {
                        EnableLogPerformance = !EnableLogPerformance;

                        if (!EnableLogPerformance)
                        {
                            _LogPerformance = "";                            
                        }
                        else
                        {
                            _LogPerformance = "Performance logging enabled";
                        }
                        HMDLog.Text = _LogPerformance + Environment.NewLine + _LogText;
                        HMDLog.Text = HMDLog.Text.Replace("\r", "");
                        HMDLog.GenerateText();
                    }
                }
            }
        }

        public static void OutputText(string text)
        {
            if (Convert.ToBoolean(Instance?.EnableLogText))
            {
                if (_LogText.Length > 1000)
                {
                    _LogText = "";
                }
                _LogText += Environment.NewLine + text;

                HMDLog.Text = _LogPerformance + Environment.NewLine + _LogText;
                HMDLog.Text = HMDLog.Text.Replace("\r", "");
                HMDLog.GenerateText();
            }

            Debug.Log(text);
        }

        public static void UpdateLogPerformance(string text)
        {
            if (Convert.ToBoolean(Instance?.EnableLogPerformance))
            {
                _LogPerformance = text;
                HMDLog.Text = _LogPerformance + Environment.NewLine + _LogText;
                HMDLog.Text = HMDLog.Text.Replace("\r", "");
                HMDLog.GenerateText();
            }
        }
    }
}
