using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class VendingMachine : MonoBehaviour
    {        
        public int StockSlowLevel;
        public int StockGrowLevel;
        public int StockShrinkLevel;

        public SimpleHelvetica CountSlowText;
        public SimpleHelvetica CountGrowText;
        public SimpleHelvetica CountShrinkText;
        public VendingButton SlowButton;
        public VendingButton GrowButton;
        public VendingButton ShrinkButton;

        public PhysicsDistorter BubbleSlow { get; set; }
        public PhysicsDistorter BubbleGrow { get; set; }
        public PhysicsDistorter BubbleShrink { get; set; }

        public DateTime LastButtonPressed { get; set; }

        public List<GameObject> MyBubbles { get; set; }

        public List<PhysicsObject> MyPhysicsObjects;
        private int _startStockSlowLevel;
        private int _startStockGrowLevel;
        private int _startStockShrinkLevel;

        private void Start()
        {
            MyBubbles = new List<GameObject>();
        }

        private void Awake()
        {
            BubbleSlow = GameObject.FindGameObjectWithTag("BubbleSlow").GetComponent<PhysicsDistorter>();
            BubbleGrow = GameObject.FindGameObjectWithTag("BubbleGrow").GetComponent<PhysicsDistorter>();
            BubbleShrink = GameObject.FindGameObjectWithTag("BubbleShrink").GetComponent<PhysicsDistorter>();

            SetStockSlowLevel(StockSlowLevel);
            SetStockGrowLevel(StockGrowLevel);
            SetStockShrinkLevel(StockShrinkLevel);            

            LastButtonPressed = DateTime.Now;
            
            PhysicsManager.Instance.VendingMachines.Add(this);
            _startStockSlowLevel = StockSlowLevel;
            _startStockGrowLevel = StockGrowLevel;
            _startStockShrinkLevel = StockShrinkLevel;
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.VendingMachines.Remove(this);
        }

        public void Reset()
        {
            foreach (var item in MyBubbles)
            {
                Destroy(item);
            }

            foreach (PhysicsObject physicsObject in MyPhysicsObjects)
            {
                physicsObject.Reset();
            }

            SetStockSlowLevel(_startStockSlowLevel);
            SetStockGrowLevel(_startStockGrowLevel);
            SetStockShrinkLevel(_startStockShrinkLevel);
        }

        public void SetStockSlowLevel(int stockLevel)
        {
            StockSlowLevel = stockLevel;

            CountSlowText.Text = StockSlowLevel.ToString();
            CountSlowText.GenerateText();

            if (stockLevel == 0)
            {
                SlowButton.ButtonEnabled = false;
            }
        }

        public void SetStockGrowLevel(int stockLevel)
        {
            StockGrowLevel = stockLevel;

            CountGrowText.Text = StockGrowLevel.ToString();
            CountGrowText.GenerateText();

            if (stockLevel == 0)
            {
                GrowButton.ButtonEnabled = false;
            }
        }

        public void SetStockShrinkLevel(int stockLevel)
        {
            StockShrinkLevel = stockLevel;

            CountShrinkText.Text = StockShrinkLevel.ToString();
            CountShrinkText.GenerateText();

            if (stockLevel == 0)
            {
                ShrinkButton.ButtonEnabled = false;
            }
        }
    }
}
