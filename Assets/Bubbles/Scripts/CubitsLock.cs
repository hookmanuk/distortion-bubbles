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
        public GameObject[] Walls;
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

            Color[] colors;

            colors = new Color[4];

            if (CurrentCubits == 0 && !_lockOpened)
            {
                colors[0] = Color.red;
                colors[1] = Color.red;
                colors[2] = Color.red;
                colors[3] = Color.red;
            }
            else if (CurrentCubits == TargetCubits || _lockOpened)
            {
                colors[0] = Color.green;
                colors[1] = Color.green;
                colors[2] = Color.green;
                colors[3] = Color.green;
            }
            else
            {
                colors[0] = Color.green;
                colors[1] = Color.green;
                colors[2] = Color.red;
                colors[3] = Color.red;
            }            

            float[] steps;

            float progress = (float)CurrentCubits / (float)TargetCubits;

            steps = new float[4];
            steps[0] = 0f;
            steps[1] = Math.Min(0.98f, Math.Max(0.01f, progress - 0.1f));
            steps[2] = Math.Min(0.99f, Math.Max(0.02f, progress + 0.1f));
            steps[3] = 1f;
            
            //RearClose.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");

            foreach (var wall in Walls)
            {                
                wall.GetComponent<MeshRenderer>().material.SetTexture("_EmissiveColorMap", CreateGradientTexture(colors, steps));                
            }
            
            //RearClose.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSIONMAP");
            //RearClose.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        }
        
        private void OpenLock()
        {
            GetComponent<AudioSource>().Play();

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
            var soundDone = false;

            while (t < 2.5f)
            {
                if (!soundDone && t >= 0.1f)
                {
                    soundDone = true;
                    Stand.GetComponent<AudioSource>().Play();
                }
                t += Time.deltaTime / 2.5f;
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

        public Texture2D CreateGradientTexture(Color[] colors, float[] stepsArray, TextureWrapMode textureWrapMode = TextureWrapMode.Clamp, FilterMode filterMode = FilterMode.Point, bool isLinear = false, bool hasMipMap = false)
        {
            int width = 1;
            int height = 256;

            if (colors == null || colors.Length == 0)
            {
                Debug.LogError("No colors assigned");
                return null;
            }

            int length = colors.Length;
            if (colors.Length > 8)
            {
                Debug.LogWarning("Too many colors! maximum is 8, assigned: " + colors.Length);
                length = 8;
            }

            // build gradient from colors
            var colorKeys = new GradientColorKey[length];
            var alphaKeys = new GradientAlphaKey[length];

            float steps = length - 1f;
            for (int i = 0; i < length; i++)
            {
                float step = i / steps;
                colorKeys[i].color = colors[i];
                colorKeys[i].time = stepsArray[i];
                alphaKeys[i].alpha = colors[i].a;
                alphaKeys[i].time = stepsArray[i];
            }

            // create gradient
            Gradient gradient = new Gradient();
            gradient.SetKeys(colorKeys, alphaKeys);

            // create texture
            Texture2D outputTex = new Texture2D(width, height, TextureFormat.ARGB32, false, isLinear);
            outputTex.wrapMode = textureWrapMode;
            outputTex.filterMode = filterMode;            

            // draw texture
            for (int i = 0; i < height; i++)
            {
                outputTex.SetPixel(0, i, gradient.Evaluate((float)i / (float)height));
            }
            outputTex.Apply(false);

            return outputTex;
        } // BuildGradientTexture
    }
}


