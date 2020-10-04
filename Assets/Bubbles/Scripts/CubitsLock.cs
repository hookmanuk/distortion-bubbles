using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class CubitsLock : PhysicsObject
    {
        public GameObject RearClose;
        public GameObject RearOpen1;
        public GameObject RearOpen2;
        public GameObject TopOpen;
        public GameObject TopClose;
        public GameObject Stand;
        public int TargetCubits;

        private int _currentCubits;
        public int CurrentCubits
        {
            get { return _currentCubits; }
            set { 
                _currentCubits = value;
                UpdateTargetText();
            }
        }
        
        public GameObject LockedObject;
        public SimpleHelvetica TargetText;

        private LayerMask _lockedMask;
        private XRGrabInteractable _lockedGrab;
        private bool _lockOpened;
        public override void Start()
        {
            base.Start();

            if (LockedObject != null)
            {
                _lockedGrab = LockedObject.GetComponent<XRGrabInteractable>();
                if (_lockedGrab != null)
                {
                    _lockedMask = _lockedGrab.interactionLayerMask;

                    _lockedGrab.interactionLayerMask = LayerMask.GetMask("Nothing");
                }                
            }

            UpdateTargetText();            
        }

        private void UpdateTargetText()
        {
            string strText;

            strText = CurrentCubits.ToString() + "/" + TargetCubits.ToString();
            TargetText.Text = strText;
            TargetText.GenerateText();
        }
        
        private void OpenLock()
        {
            RearClose.SetActive(false);
            TopClose.SetActive(false);
            RearOpen1.SetActive(true);
            RearOpen2.SetActive(true);
            TopOpen.SetActive(true);

            _lockedGrab.interactionLayerMask = _lockedMask;

            _lockOpened = true;

            StartCoroutine(RaiseStand());
        }

        public IEnumerator RaiseStand()
        {
            var currentPos = Stand.transform.localPosition;
            var currentScale = Stand.transform.localScale;            
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / 1;
                Stand.transform.localPosition = Vector3.Lerp(currentPos, new Vector3(currentPos.x, 0, currentPos.z), t);
                Stand.transform.localScale = Vector3.Lerp(currentScale, new Vector3(currentScale.x, 0.5f, currentScale.z), t);

                LockedObject.transform.position = new Vector3(Stand.transform.position.x, Stand.transform.position.y + Stand.transform.localScale.y*0.5f + 0.01f, Stand.transform.position.z);
                yield return null;
            }
        }

        private void CloseLock()
        {
            RearClose.SetActive(true);
            TopClose.SetActive(true);
            RearOpen1.SetActive(false);
            RearOpen2.SetActive(false);
            TopOpen.SetActive(false);

            _lockedGrab.interactionLayerMask = LayerMask.GetMask("Nothing");

            _lockOpened = false;            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                if (other.gameObject.GetComponent<PhysicsObject>().IsCubit)
                {
                    CurrentCubits++;

                    if (!_lockOpened && CurrentCubits >= TargetCubits)
                    {
                        OpenLock();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                if (other.gameObject.GetComponent<PhysicsObject>().IsCubit)
                {
                    CurrentCubits--;

                    if (CurrentCubits < 0)
                    {
                        CurrentCubits = 0;
                    }
                }
            }
        }

        public override void Reset()
        {
            if (_lockOpened)
            {
                CloseLock();
            }
        }
    }
}
