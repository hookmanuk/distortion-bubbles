using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class VendingMachine : MonoBehaviour
    {        
        public int StockLevel;

        public SimpleHelvetica CountText;

        public PhysicsDistorter BubbleSlow { get; set; }
        public PhysicsDistorter BubbleGrow { get; set; }
        public PhysicsDistorter BubbleShrink { get; set; }

        public DateTime LastButtonPressed { get; set; }

        private void Awake()
        {
            BubbleSlow = GameObject.FindGameObjectWithTag("BubbleSlow").GetComponent<PhysicsDistorter>();
            BubbleGrow = GameObject.FindGameObjectWithTag("BubbleGrow").GetComponent<PhysicsDistorter>();
            BubbleShrink = GameObject.FindGameObjectWithTag("BubbleShrink").GetComponent<PhysicsDistorter>();

            CountText.Text = StockLevel.ToString();
            CountText.GenerateText();

            LastButtonPressed = DateTime.Now;
        }

        public void SetStockLevel(int stockLevel)
        {
            StockLevel = stockLevel;

            CountText.Text = StockLevel.ToString();
            CountText.GenerateText();
        }
    }
}
