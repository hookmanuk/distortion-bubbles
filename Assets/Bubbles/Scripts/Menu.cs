using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
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
        public GameObject StartOptions;
        public GameObject InGameOptions;
        public GameObject GenericOptions;
        public bool IsStartMenu;

        private int _frameUpdate = 90;
        private int _currentFrame = 0;

        private void Start()
        {            
            if (IsStartMenu)
            {
                int intCurrentVendingMachine = PlayerPrefs.GetInt("CurrentVendingMachine");
                if (intCurrentVendingMachine > 0)
                {
                    StartOptions.SetActive(true);
                }
                InGameOptions.SetActive(false);
                GenericOptions.SetActive(false); //disable graphics as nothing to see here??
            }
            SetText();
        }

        public void CycleSettings()
        {
            if (UnityEngine.QualitySettings.GetQualityLevel() == 3)
            {
                UnityEngine.QualitySettings.SetQualityLevel(0);
            }
            else
            {
                UnityEngine.QualitySettings.SetQualityLevel(UnityEngine.QualitySettings.GetQualityLevel() + 1);
            }
            switch (UnityEngine.QualitySettings.GetQualityLevel())
            {
                case 0:
                case 1:
                    PlayerController.Instance.GraphicsQuality = QualitySettings.Instance.QualityLow;
                    break;
                case 2:
                    PlayerController.Instance.GraphicsQuality = QualitySettings.Instance.QualityLow;
                    break;
                case 3:
                    PlayerController.Instance.GraphicsQuality = QualitySettings.Instance.QualityMedium;
                    break;
                default:
                    break;
            }
            SetText();
        }        

        private void SetText()
        {
            GraphicsInfo.Text = UnityEngine.QualitySettings.names[UnityEngine.QualitySettings.GetQualityLevel()] + " Quality";
            GraphicsInfo.GenerateText();
            //ResInfo.Text = UnityEngine.QualitySettings.names[UnityEngine.QualitySettings.GetQualityLevel()];
            //ResInfo.GenerateText();
        }

        public void ResumeGame()
        {
            PlayerController.Instance.IntroStart = false;

            PlayerController.Instance.CurrentVendingMachine.NextGlow.SetActive(false);

            int intCurrentVendingMachine = PlayerPrefs.GetInt("CurrentVendingMachine");
            VendingMachine vend = PhysicsManager.Instance.VendingMachines.Where(vm => vm.Order == intCurrentVendingMachine).FirstOrDefault();

            for (int i = 0; i < PlayerController.Instance.Levels.Length; i++)
            {
                PlayerController.Instance.Levels[i].SetActive(true);
                foreach (var item in PlayerController.Instance.Levels[i].GetComponentsInChildren<PhysicsObject>())
                {
                    item.Reset();
                }

                foreach (var item in PlayerController.Instance.Levels[i].GetComponentsInChildren<TeleportRoom>())
                {
                    item.Reset();
                }
            }

            vend.ButtonPressed();

            PlayerController.Instance.Reset(true);
        }

        private void FixedUpdate()
        {
            //if (!IsStartMenu)
            //{
            //    _currentFrame++;

            //    if (_currentFrame == _frameUpdate)
            //    {
            //        FPSInfo.Text = DynamicResolution.ReportedFPS.ToString();
            //        FPSInfo.GenerateText();

            //        DynamicResInfo.Text = (DynamicResolution.DynamicResolutionEnabled ? Math.Round(DynamicResolution.MinDynamicResolution + (100 - DynamicResolution.MinDynamicResolution) * DynamicResolution.ResScale, 0).ToString() + "%" : "100%");
            //        DynamicResInfo.GenerateText();

            //        _currentFrame = 0;
            //    }
            //}
        }
    }
}
