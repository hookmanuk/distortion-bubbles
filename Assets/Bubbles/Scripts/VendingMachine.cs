using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int StockShowLevel;
        public int StockShowHintLevel;
        public int StockBlackHoleLevel;
        public int StockLightLevel;

        public VendingButton SlowButton;
        public VendingButton GrowButton;
        public VendingButton ShrinkButton;
        public VendingButton GravityButton;
        public VendingButton LaunchButton;        
        public VendingButton ShowButton;
        public VendingButton ShowHintButton;
        public VendingButton BlackHoleButton;
        public VendingButton LightButton;

        public Light PointLight;
        public int Order;

        public List<HintArea> HintAreas;
        public List<PhysicsObject> HintObjects;

        public PhysicsDistorter BubbleSlow { get; set; }
        public PhysicsDistorter BubbleGrow { get; set; }
        public PhysicsDistorter BubbleShrink { get; set; }
        public PhysicsDistorter BubbleGravity { get; set; }
        public PhysicsDistorter BubbleLaunch { get; set; }        
        public PhysicsDistorter BubbleShow { get; set; }
        public PhysicsDistorter BubbleBlackHole { get; set; }
        public PhysicsDistorter BubbleLight { get; set; }

        public DateTime LastButtonPressed { get; set; }        

        public List<GameObject> MyBubbles { get; set; }
        

        public List<PhysicsObject> MyPhysicsObjects;

        public GameObject ShowHintGlowQuad;

        private int _startStockSlowLevel;
        private int _startStockGrowLevel;
        private int _startStockShrinkLevel;
        private int _startStockGravityLevel;
        private int _startStockLaunchLevel;
        private int _startStockCutOffLevel;
        private int _startStockShowLevel;
        private int _startStockShowHintLevel;
        private int _startStockBlackHoleLevel;
        private int _startStockLightLevel;

        private bool _firstButtonPress = true;
        private bool _showHints = false;
        private int _intSecondsSincePressed = 0;
        private int _totalHints;
        private int _nextHint = 1;

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
            BubbleShow = GameObject.FindGameObjectWithTag("BubbleShow").GetComponent<PhysicsDistorter>();
            BubbleBlackHole = GameObject.FindGameObjectWithTag("BubbleBlackHole").GetComponent<PhysicsDistorter>();
            BubbleLight = GameObject.FindGameObjectWithTag("BubbleLight").GetComponent<PhysicsDistorter>();

            _startStockSlowLevel = StockSlowLevel;
            _startStockGrowLevel = StockGrowLevel;
            _startStockShrinkLevel = StockShrinkLevel;
            _startStockGravityLevel = StockGravityLevel;
            _startStockLaunchLevel = StockLaunchLevel;
            _startStockShowLevel = StockShowLevel;
            _startStockBlackHoleLevel = StockBlackHoleLevel;
            _startStockLightLevel = StockLightLevel;
            _startStockShowHintLevel = 0;            

            bool blnHintFound = false;
            foreach (var HintArea in HintAreas)
            {                
                if (!blnHintFound)
                {
                    blnHintFound = true;
                    _totalHints++;
                }
                if (HintArea == null) //using null entry as divider between hints
                {
                    blnHintFound = false;
                }
            }                        

            InitStockLevels(true);

            LastButtonPressed = DateTime.Now;
            
            PhysicsManager.Instance.VendingMachines.Add(this);            
        }

        public void ButtonPressed()
        {
            LastButtonPressed = DateTime.Now;

            //set counter going to then show hint
            if (_intSecondsSincePressed == 0)
            {
                StartCoroutine(CountSincePressed());
            }
        }

        public IEnumerator CountSincePressed()
        {
            if (_totalHints > 0)
            {
                while (!_showHints)
                {
                    yield return new WaitForSeconds(1);
                    _intSecondsSincePressed++;

                    if (_intSecondsSincePressed >= 60)
                    {
                        VendingMachine vendingMachine = PhysicsManager.Instance.VendingMachines.OrderByDescending(vm => vm.LastButtonPressed).FirstOrDefault();
                        if (vendingMachine == this)
                        {
                            _intSecondsSincePressed = 0;
                            _showHints = true;
                            ShowHintButton.GetComponent<AudioSource>().Play();
                            //ShowHintGlowQuad.GetComponent<MeshRenderer>().material.SetFloat("GLOW_ALPHA", 0.1f); Doesn't work :(
                            SetStockShowHintLevel(_totalHints);
                        }
                    }
                }
            }
        }

        private void InitStockLevels(bool blnInitialSetup)
        {
            _firstButtonPress = false;

            SetStockSlowLevel(_startStockSlowLevel);
            SetStockGrowLevel(_startStockGrowLevel);
            SetStockShrinkLevel(_startStockShrinkLevel);
            SetStockGravityLevel(_startStockGravityLevel);
            SetStockLaunchLevel(_startStockLaunchLevel);
            SetStockShowLevel(_startStockShowLevel);
            SetStockBlackHoleLevel(_startStockBlackHoleLevel);
            SetStockLightLevel(_startStockLightLevel);

            if (blnInitialSetup)
            {
                SetStockShowHintLevel(_startStockShowHintLevel);
            }

            _firstButtonPress = true;
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

            InitStockLevels(false);

            _firstButtonPress = true;
        }

        public void SetStockSlowLevel(int stockLevel)
        {
            StockSlowLevel = stockLevel;
            SlowButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockGrowLevel(int stockLevel)
        {
            StockGrowLevel = stockLevel;
            GrowButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockShrinkLevel(int stockLevel)
        {
            StockShrinkLevel = stockLevel;
            ShrinkButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockGravityLevel(int stockLevel)
        {
            StockGravityLevel = stockLevel;
            GravityButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockLaunchLevel(int stockLevel)
        {
            StockLaunchLevel = stockLevel;
            LaunchButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }       

        public void SetStockShowLevel(int stockLevel)
        {
            StockShowLevel = stockLevel;
            ShowButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockBlackHoleLevel(int stockLevel)
        {
            StockBlackHoleLevel = stockLevel;
            BlackHoleButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockLightLevel(int stockLevel)
        {
            StockLightLevel = stockLevel;
            LightButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockShowHintLevel(int stockLevel)
        {
            StockShowHintLevel = stockLevel;
            ShowHintButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        private void CheckForKeyReset()
        {
            if (_firstButtonPress)
            {
                foreach (PhysicsObject physicsObject in MyPhysicsObjects)
                {
                    KeyObject keyObject;
                    if (physicsObject.TryGetComponent<KeyObject>(out keyObject))
                    {
                        OutputLogManager.OutputText("Resetting key " + keyObject.name);
                        physicsObject.Reset();
                    }                    
                }

                _firstButtonPress = false;
            }
        }

        public void ActivateNextHint()
        {
            if (_nextHint > 0 && _nextHint <= _totalHints)
            {
                int intCurrentHintLoop = 0;
                bool blnHintFound = false;
                foreach (var HintArea in HintAreas)
                {
                    if (!blnHintFound)
                    {
                        intCurrentHintLoop++;
                        if (intCurrentHintLoop == _nextHint)
                        {
                            HintArea.EnableGlow();
                            HintArea.GetComponent<AudioSource>().Play();
                        }                        
                        blnHintFound = true;                        
                    }
                    if (intCurrentHintLoop > _nextHint)
                    {
                        break;
                    }                    
                    if (HintArea == null) //using null entry as divider between hints
                    {
                        blnHintFound = false;
                    }
                    else
                    {
                        if (intCurrentHintLoop == _nextHint)
                        {
                            HintArea.EnableGlow();
                        }
                    }
                }

                intCurrentHintLoop = 0;
                blnHintFound = false;
                foreach (var HintObject in HintObjects)
                {
                    if (!blnHintFound)
                    {
                        blnHintFound = true;
                        intCurrentHintLoop++;
                    }
                    if (intCurrentHintLoop > _nextHint)
                    {
                        break;
                    }
                    if (intCurrentHintLoop == _nextHint)
                    {
                        //TODO
                        //HintObject.MakeItGlow();
                    }
                    if (HintObject == null) //using null entry as divider between hints
                    {
                        blnHintFound = false;
                    }
                }
                // Hints[_nextHint - 1].ShowHint();

                StockShowHintLevel--;
                SetStockShowHintLevel(StockShowHintLevel);
                _nextHint++;
            }
        }
    }
}
