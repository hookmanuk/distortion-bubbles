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
        public int StockGravityLevel;
        public int StockLaunchLevel;

        public SimpleHelvetica CountSlowText;
        public SimpleHelvetica CountGrowText;
        public SimpleHelvetica CountShrinkText;
        public SimpleHelvetica CountGravityText;
        public SimpleHelvetica CountLaunchText;
        public VendingButton SlowButton;
        public VendingButton GrowButton;
        public VendingButton ShrinkButton;
        public VendingButton GravityButton;
        public VendingButton LaunchButton;
        public Light PointLight;
        public int Order;        

        public PhysicsDistorter BubbleSlow { get; set; }
        public PhysicsDistorter BubbleGrow { get; set; }
        public PhysicsDistorter BubbleShrink { get; set; }
        public PhysicsDistorter BubbleGravity { get; set; }
        public PhysicsDistorter BubbleLaunch { get; set; }

        public DateTime LastButtonPressed { get; set; }

        public List<GameObject> MyBubbles { get; set; }
        

        public List<PhysicsObject> MyPhysicsObjects;
        private int _startStockSlowLevel;
        private int _startStockGrowLevel;
        private int _startStockShrinkLevel;
        private int _startStockGravityLevel;
        private int _startStockLaunchLevel;

        private void Start()
        {
            MyBubbles = new List<GameObject>();
        }

        private void Awake()
        {
            BubbleSlow = GameObject.FindGameObjectWithTag("BubbleSlow").GetComponent<PhysicsDistorter>();
            BubbleGrow = GameObject.FindGameObjectWithTag("BubbleGrow").GetComponent<PhysicsDistorter>();
            BubbleShrink = GameObject.FindGameObjectWithTag("BubbleShrink").GetComponent<PhysicsDistorter>();
            BubbleGravity = GameObject.FindGameObjectWithTag("BubbleGravity").GetComponent<PhysicsDistorter>();
            BubbleLaunch = GameObject.FindGameObjectWithTag("BubbleLaunch").GetComponent<PhysicsDistorter>();

            SetStockSlowLevel(StockSlowLevel);
            SetStockGrowLevel(StockGrowLevel);
            SetStockShrinkLevel(StockShrinkLevel);
            SetStockGravityLevel(StockGravityLevel);
            SetStockLaunchLevel(StockLaunchLevel);

            LastButtonPressed = DateTime.Now;
            
            PhysicsManager.Instance.VendingMachines.Add(this);
            _startStockSlowLevel = StockSlowLevel;
            _startStockGrowLevel = StockGrowLevel;
            _startStockShrinkLevel = StockShrinkLevel;
            _startStockGravityLevel = StockGravityLevel;
            _startStockLaunchLevel = StockLaunchLevel;
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.VendingMachines.Remove(this);
        }

        public void Reset()
        {
            foreach (var item in MyBubbles)
            {
                item.gameObject.SetActive(false);
                //Destroy(item);
            }

            foreach (PhysicsObject physicsObject in MyPhysicsObjects)
            {
                physicsObject.Reset();
            }

            SetStockSlowLevel(_startStockSlowLevel);
            SetStockGrowLevel(_startStockGrowLevel);
            SetStockShrinkLevel(_startStockShrinkLevel);
            SetStockGravityLevel(_startStockGravityLevel);
            SetStockLaunchLevel(_startStockLaunchLevel);
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
            else
            {
                SlowButton.ButtonEnabled = true;
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
            else
            {
                GrowButton.ButtonEnabled = true;
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
            else
            {
                ShrinkButton.ButtonEnabled = true;
            }
        }

        public void SetStockGravityLevel(int stockLevel)
        {
            StockGravityLevel = stockLevel;

            CountGravityText.Text = StockGravityLevel.ToString();
            CountGravityText.GenerateText();

            if (stockLevel == 0)
            {
                GravityButton.ButtonEnabled = false;
            }
            else
            {
                GravityButton.ButtonEnabled = true;
            }
        }

        public void SetStockLaunchLevel(int stockLevel)
        {
            StockLaunchLevel = stockLevel;

            CountLaunchText.Text = StockLaunchLevel.ToString();
            CountLaunchText.GenerateText();

            if (stockLevel == 0)
            {
                LaunchButton.ButtonEnabled = false;
            }
            else
            {
                LaunchButton.ButtonEnabled = true;
            }
        }
    }
}
