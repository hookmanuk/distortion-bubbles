using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BubbleDistortionPhysics
{
    class OutputLogManager : MonoBehaviour
    {
        public bool EnableLog;
        public OutputLogManager()
        {            
        }

        private void Awake()
        {
            _instance = this;
            HMDLog = _instance.GetComponent<TextMeshPro>();
        }

        public static TextMeshPro HMDLog;

        private static OutputLogManager _instance;
        public static OutputLogManager Instance { 
            get
            {                
                return _instance;
            }            
        }
        
        public static void OutputText(string text)
        {
            if (OutputLogManager.Instance.EnableLog)
            {
                if (HMDLog.text.Length > 1000)
                {
                    HMDLog.text = "";
                }
                HMDLog.text += Environment.NewLine + text;
            }

            Debug.Log(text);
        }
    }
}
