using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace BubbleDistortionPhysics
{
    public class VendingButton : XRSimpleInteractable
    {        
        private float ymin;
        private float ymax;
        private bool previousPress = false;
        private Material _normal_background;
        private Material _hint_background;

        private VendingMachine _vendingMachine;
        public DistorterType Type;
        public bool ButtonEnabled;
        public GameObject StockValueText;
        private float previousHandHeight;
        private XRBaseInteractor hoverInteractor;
        public GameObject DisabledQuad;

        protected override void Awake()
        {
            base.Awake();

            _vendingMachine = GetComponentInParent<VendingMachine>();

            if (!ButtonEnabled)
            {
                Color32 col = gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
                col.a = 60;
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", col);
            }

            onHoverEnter.AddListener(StartPress);
            onHoverExit.AddListener(EndPress);
        }

        private void Start()
        {
            SetMinMax();
        }

        private void SetMinMax()
        {
            Collider collider = GetComponent<Collider>();

            ymin = transform.localPosition.z - (collider.bounds.size.x * 0.8f);
            ymax = transform.localPosition.z;
        }

        private void OnTriggerEnter(Collider other)
        {
            OutputLogManager.OutputText(this.name + " hit by " + other.name);
        }

        private void StartPress(XRBaseInteractor interactor)
        {
            hoverInteractor = interactor;
            previousHandHeight = GetLocalYPosition(hoverInteractor.transform.position);

            

            base.OnSelectEnter(interactor);
        }

        private void EndPress(XRBaseInteractor interactor)
        {
            hoverInteractor = null;
            previousHandHeight = 0;

            previousPress = false;
            SetYPosition(ymax);
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (hoverInteractor && !inPosition())
            {
                float newHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
                float handDifference = previousHandHeight - newHandHeight;
                
                previousHandHeight = newHandHeight;

                float newPosition = transform.localPosition.z - handDifference;                
                SetYPosition(newPosition);

                CheckPress();
            }
        }

        private float GetLocalYPosition(Vector3 position)
        {
            Vector3 localPosition = transform.root.InverseTransformPoint(position);

            return -localPosition.x;
        }

        private void SetYPosition(float position)
        {
            Vector3 newPosition = transform.localPosition;

            newPosition.z = Mathf.Clamp(position, ymin, ymax);
            transform.localPosition = newPosition;
        }

        private void CheckPress()
        {
            bool blnInPosition = inPosition();

            if (blnInPosition && blnInPosition != previousPress)
            {
                if (ButtonEnabled && (DateTime.Now - _vendingMachine.LastButtonPressed).TotalMilliseconds > 500)
                {
                    _vendingMachine.ButtonPressed();

                    if (Type == DistorterType.Hint)
                    {
                        _vendingMachine.ActivateNextHint();
                    }
                    else
                    {
                        GameObject bubbleClone;

                        bubbleClone = null;

                        if (Type == DistorterType.Slow)
                        {
                            bubbleClone = Instantiate(_vendingMachine.BubbleSlow.gameObject);
                            _vendingMachine.SetStockSlowLevel(_vendingMachine.StockSlowLevel - 1);
                        }
                        else if (Type == DistorterType.Grow)
                        {
                            bubbleClone = Instantiate(_vendingMachine.BubbleGrow.gameObject);
                            _vendingMachine.SetStockGrowLevel(_vendingMachine.StockGrowLevel - 1);
                        }
                        else if (Type == DistorterType.Shrink)
                        {
                            bubbleClone = Instantiate(_vendingMachine.BubbleShrink.gameObject);
                            _vendingMachine.SetStockShrinkLevel(_vendingMachine.StockShrinkLevel - 1);
                        }
                        else if (Type == DistorterType.Gravity)
                        {
                            bubbleClone = Instantiate(_vendingMachine.BubbleGravity.gameObject);
                            _vendingMachine.SetStockGravityLevel(_vendingMachine.StockGravityLevel - 1);
                        }
                        else if (Type == DistorterType.Launch)
                        {
                            bubbleClone = Instantiate(_vendingMachine.BubbleLaunch.gameObject);
                            _vendingMachine.SetStockLaunchLevel(_vendingMachine.StockLaunchLevel - 1);
                        }
                        else if (Type == DistorterType.Show)
                        {
                            bubbleClone = Instantiate(_vendingMachine.BubbleShow.gameObject);
                            _vendingMachine.SetStockShowLevel(_vendingMachine.StockShowLevel - 1);
                        }
                        else if (Type == DistorterType.BlackHole)
                        {
                            bubbleClone = Instantiate(_vendingMachine.BubbleBlackHole.gameObject);
                            _vendingMachine.SetStockBlackHoleLevel(_vendingMachine.StockBlackHoleLevel - 1);
                        }

                        bubbleClone.tag = "Untagged";
                        bubbleClone.transform.position = new Vector3(_vendingMachine.transform.position.x - 0.46f, _vendingMachine.transform.position.y + 0.505f, _vendingMachine.transform.position.z + 0.049f);
                        bubbleClone.transform.rotation = new Quaternion(120, 90, 0, 0);
                        bubbleClone.GetComponent<Rigidbody>().useGravity = true;
                        bubbleClone.GetComponents<AudioSource>()[0].Play();

                        _vendingMachine.MyBubbles.Add(bubbleClone);
                        _vendingMachine.PointLight.gameObject.SetActive(true);
                        bubbleClone.GetComponent<PhysicsDistorter>().SourceMachine = _vendingMachine;
                    }

                    if (_vendingMachine.Order >= 11)
                    {
                        Level2Start.Instance.StartLevel2();
                    }
                }
            }

            previousPress = blnInPosition;
        }

        private bool inPosition()
        {
            float inRange = Mathf.Clamp(transform.localPosition.z, ymin, ymin + 0.01f);

            return transform.localPosition.z == inRange;
        }

        public void SetStockLevel(int stockLevel)
        {
            StockValueText.GetComponent<SimpleHelvetica>().Text = stockLevel.ToString();
            StockValueText.GetComponent<SimpleHelvetica>().GenerateText();

            if (stockLevel == 0)
            {
                if (Type == DistorterType.Hint)
                {

                    MeshRenderer meshRenderer;
                    meshRenderer = GetComponent<MeshRenderer>();
                    if (_hint_background == null)
                    {
                        _normal_background = meshRenderer.materials[0];
                        _hint_background = meshRenderer.materials[1];
                    }
                    meshRenderer.material = _normal_background;
                }
                DisabledQuad.SetActive(true);
                ButtonEnabled = false;
            }
            else
            {
                if (Type == DistorterType.Hint && _hint_background != null)
                {
                    MeshRenderer meshRenderer;
                    meshRenderer = GetComponent<MeshRenderer>();
                    meshRenderer.material= _hint_background;
                }
                DisabledQuad.SetActive(false);
                ButtonEnabled = true;
            }
        }
    }    
}
