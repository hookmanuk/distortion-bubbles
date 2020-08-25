using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{

    public class PlayerController : MonoBehaviour
    {
        public float speed = 1;
        public CharacterController characterController { get; set; }
        public CapsuleCollider capsuleCollider { get; set; }
        public XRController LeftController;
        public XRController RightController;
        public GameObject MainCamera { get; set; }
        public List<PuzzleArea> ActivePuzzleAreas { get; set; }

        public Volume SkyVolume;
        public List<Color> SkyColours;
        public List<float> SkyBoundaries;
        public GameObject LowerCutOff;
        public GameObject UpperCutOff;
        public GameObject[] Levels;
        public int[] LevelCeilings;
        public int[] LevelFloors;

        private bool _preventCharacterMovement;
        private bool _resetting;
        private bool _disolving;
        private DateTime _lastSkipTime = DateTime.Now;
        private bool _flipping;
        private static PlayerController _instance;
        public static PlayerController Instance { get { return _instance; } }
        public bool Flipping { get { return _flipping; } }
        private MeshRenderer _outOfBoundsFace;
        public bool ReverseGravity;
        private float GravityMultiplier = 1f;
        private DateTime _lastLaunch;
        private bool _isAirbourne;
        private GradientSky _sky;
        public float TriggerPercentage;

        public float LightsDistance { get; set; } = 10f;

        public List<GameObject> HeldObjects { get; set; } = new List<GameObject>();
        private List<int> HeldObjectLayers { get; set; } = new List<int>();

        Vector2 currentState;
        Vector3 direction;

        float mass = 1.0F; // defines the character mass
        Vector3 impact = Vector3.zero;        

        private void Start()
        {         
            characterController = GetComponent<CharacterController>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            _outOfBoundsFace = GameObject.FindGameObjectWithTag("OutOfBoundsFace").GetComponent<MeshRenderer>();
            _instance = this;
            ActivePuzzleAreas = new List<PuzzleArea>();

            PhysicsManager.Instance.TurnOffLights();
                     
            SkyVolume.profile.TryGet(out _sky);            
        }

        private void Update()
        {
            direction = characterController.GetComponentInChildren<Camera>().transform.TransformDirection(new Vector3(currentState.x, 0, currentState.y)); //Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));

            // apply the impact force:
            if (impact.magnitude > 0.2F) characterController.Move(impact * Time.deltaTime);
            // consumes the impact energy each cycle:
            impact = Vector3.Lerp(impact, Vector3.zero, 2 * Time.deltaTime);

            //code to show and hide lights based on distance from player
            //foreach (var item in PhysicsManager.Instance.LightStrips)
            //{
            //    if (!item.gameObject.activeSelf && Vector3.Distance(characterController.transform.position, item.transform.position) < LightsDistance)
            //    {
            //        item.gameObject.SetActive(true);
            //    }
            //    else if (item.gameObject.activeSelf && Vector3.Distance(characterController.transform.position, item.transform.position) > (LightsDistance * 1.2f))
            //    {
            //        item.gameObject.SetActive(false);
            //    }
            //}
        }

        public void Reset()
        {
            _disolving = true;                        
        }

        private bool _blnReversingGravity = false;
        private IEnumerator ReversePlayerGravity()
        {
            if (!_blnReversingGravity)
            {
                ReverseGravity = !ReverseGravity;
                
                _blnReversingGravity = true;

                var originalGravity = Physics.gravity.y;
                var t = 0f;
                var intTimeToOpen = 0.3f;
                while (t < 1)
                {
                    t += Time.deltaTime / intTimeToOpen;

                    Physics.gravity = new Vector3(0, originalGravity + (t * 2f * -originalGravity), 0);

                    if (Math.Abs(Physics.gravity.y) > Math.Abs(originalGravity))
                    {
                        Physics.gravity = new Vector3(0, -originalGravity, 0);
                        break;
                    }
                    //GravityMultiplier = 0;

                    //transform.rotation = Quaternion.Euler(0, 0,(ReverseGravity ? t : (1-t)) * 180);                    

                    //OutputLogManager.OutputText(transform.rotation.x.ToString());

                    //OutputLogManager.OutputText(t.ToString());

                    yield return null;
                }
                
                _blnReversingGravity = false;
            }
        }

        private IEnumerator RotatePlayer()
        {            
            var t = 0f;
            var intTimeToOpen = 0.7f;
            while (t < 1)
            {
                t += Time.deltaTime / intTimeToOpen;
                
                transform.rotation = Quaternion.Euler(0, 0, (ReverseGravity ? t : (1 - t)) * 180);
                yield return null;
            }

            _flipping = false;

            SetHeldObjectsInteractable();
        }

        public void FlipGravity()
        {
            _flipping = true;

            SetHeldObjectsUninteractable();

            KeyObject keyObject = LeftController.gameObject.GetComponentInChildren<KeyObject>();
            if (keyObject != null)
            {
                keyObject.GetComponent<Rigidbody>().isKinematic = true;
            }

            if (ReverseGravity)
            {
                PhysicsManager.Instance.GravityNormal();
            }
            else
            {                
                PhysicsManager.Instance.GravityInverse();
            }
            
            StartCoroutine(ReversePlayerGravity());

            StartCoroutine(RotatePlayer());
        }

        public void LaunchPlayer()
        {
            if ((DateTime.Now - _lastLaunch).TotalMilliseconds > 100)
            {
                _lastLaunch = DateTime.Now;
                Vector3 direction = new Vector3(characterController.velocity.x, 3f, characterController.velocity.z);
                AddImpact(direction, 10f);
                _isAirbourne = true;
            }            
        }

        // call this function to add an impact force:
        public void AddImpact(Vector3 dir, float force)
        {            
            //dir.Normalize();
            OutputLogManager.OutputText(dir.ToString());
            if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
            //impact += dir.normalized * force / mass;
            impact += dir * force / mass;
        }
        
        void FixedUpdate()
        {
            bool blnResetClicked = false;
            bool blnDebugSkipClicked = false;
            int LastVend = -1;            

            RightController?.inputDevice.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out blnDebugSkipClicked);

            RightController?.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out TriggerPercentage);

            if (blnDebugSkipClicked && _lastSkipTime < DateTime.Now.AddSeconds(-.5f))
            {
                _lastSkipTime = DateTime.Now;
                VendingMachine vendingMachine = PhysicsManager.Instance.VendingMachines.OrderByDescending(vm => vm.LastButtonPressed).FirstOrDefault();

                if (vendingMachine != null)
                {
                    LastVend = vendingMachine.Order;
                }
                LastVend++;

                OutputLogManager.OutputText("Skipping to machine " + LastVend.ToString());

                vendingMachine = PhysicsManager.Instance.VendingMachines.Where(vm => vm.Order == LastVend).FirstOrDefault();

                if (vendingMachine == null)
                { 
                    vendingMachine = PhysicsManager.Instance.VendingMachines.Where(vm => vm.Order == 0).FirstOrDefault();
                }
             
                vendingMachine.LastButtonPressed = DateTime.Now;             

                Reset();
            }            

            LeftController?.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out blnResetClicked);

            if (blnResetClicked)
            {
                Reset();
            }

            if (_disolving)
            {
                _disolving = false;
                StartCoroutine(DissolveFace());
            }

            if (_resetting)
            {                
                if (ReverseGravity)
                {
                    OutputLogManager.OutputText("flip from reset");
                    FlipGravity();
                }
                if (!_flipping)
                {
                    _isAirbourne = false;
                    _resetting = false;
                    OutputLogManager.OutputText("no longer flipping, now reset");
                    Vector3 resetPosition = PhysicsManager.Instance.Reset();
                    characterController.transform.position = resetPosition - new Vector3(1 + MainCamera.transform.localPosition.x, 0, MainCamera.transform.localPosition.z);
                    capsuleCollider.height = MainCamera.transform.localPosition.y;
                    capsuleCollider.center = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y / 2, MainCamera.transform.localPosition.z);
                    
                }                
            }
            else
            {
                capsuleCollider.height = MainCamera.transform.localPosition.y;
                capsuleCollider.center = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y / 2, MainCamera.transform.localPosition.z);

                if (!_preventCharacterMovement)
                {
                    characterController.height = MainCamera.transform.localPosition.y;
                    characterController.center = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y / 2, MainCamera.transform.localPosition.z);


                    InputDevice device = LeftController.inputDevice;
                    InputFeatureUsage<Vector2> feature = CommonUsages.secondary2DAxis;
                    Vector3 movement;

                    //movement = new Vector3(0, -9.81f * GravityMultiplier, 0) * Time.deltaTime;
                    movement = Physics.gravity * Time.deltaTime;

                    if (device.TryGetFeatureValue(feature, out currentState))
                    {
                        if (currentState.magnitude > 0.1)
                        {
                            movement = movement + speed * (!characterController.isGrounded ? 4f : 1f) * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up);
                        }
                    }

                    characterController.Move(movement);
                    
                }
            }

            Color? colourBelow = null;
            Color? colourAbove = null;
            float? boundaryBelow = null;
            float? boundaryAbove = null;

            float charPosition = characterController.transform.position.y;
            for (int i = 0; i < SkyBoundaries.Count; i++)
            {
                if (SkyBoundaries[i] < charPosition)
                {
                    boundaryBelow = SkyBoundaries[i];
                    colourBelow = SkyColours[i];
                }
                else
                {
                    if (colourAbove == null)
                    {
                        boundaryAbove = SkyBoundaries[i];
                        colourAbove = SkyColours[i];
                    }
                }
            }
           
            if (colourAbove.HasValue && colourBelow.HasValue && colourAbove != colourBelow)
            {
                Color skyColour;
                float skyTopRatio;

                skyTopRatio = (charPosition - boundaryBelow.Value) / (boundaryAbove.Value - boundaryBelow.Value);

                skyColour = colourBelow.Value * (1 - skyTopRatio) + colourAbove.Value * skyTopRatio;
                _sky.top.value = skyColour;
            }
            else if (!colourAbove.HasValue && colourBelow.HasValue && (_sky.top.value != colourBelow))
            {
                _sky.top.value = colourBelow.Value;
            }
            else if (!colourBelow.HasValue && colourAbove.HasValue && (_sky.top.value != colourAbove))
            {
                _sky.top.value = colourAbove.Value;
            }

            //doesnt really work for ceiling, too claustrophobic
            //Color cutOffSkyColor = new Color(_sky.top.value.r, _sky.top.value.g, _sky.top.value.b, 0.1f);
            //foreach (var item in UpperCutOff.GetComponentsInChildren<MeshRenderer>())
            //{
            //    if (item.material.color != cutOffSkyColor)
            //    {
            //        item.material.color = cutOffSkyColor;
            //    }
            //}

            for (int i = 0; i < Levels.Length - 1; i++)
            {
                if (LowerCutOff.transform.position.y - 3 > LevelCeilings[i] || gameObject.transform.position.y + 15 < LevelFloors[i])
                {
                    if (Levels[i].activeSelf)
                    {
                        Levels[i].SetActive(false);
                    }
                }
                else
                {
                    if (!Levels[i].activeSelf)
                    {
                        Levels[i].SetActive(true);
                    }
                }
            }

            //for (int i = 0; i < Levels.Length - 1; i++)
            //{
            //    if (gameObject.transform.position.y + 20 < LevelFloors[i])
            //    {
            //        if (Levels[i].activeSelf)
            //        {
            //            Levels[i].SetActive(false);
            //        }
            //    }
            //    else
            //    {
            //        if (!Levels[i].activeSelf)
            //        {
            //            Levels[i].SetActive(true);
            //        }
            //    }
            //}
        }

        private void OnCollisionEnter(Collision collision)
        {
            _isAirbourne = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {                
                if (other.gameObject.GetComponent<PhysicsObject>().Speed > 0)
                {
                    OutputLogManager.OutputText("player hit " + other.gameObject.name);
                    if (other.gameObject.GetComponent<Drone>() != null)
                    {
                        //hit a drone, so reset
                        PhysicsManager.Instance.Reset();
                    }                    
                }
                
                //OutputLogManager.OutputText("character y pos " + characterController.gameObject.transform.position.y.ToString());

                //allow walking on platforms
                if (characterController.gameObject.transform.position.y - other.gameObject.transform.position.y > 0.1f)
                {
                    //stop!
                    //OutputLogManager.OutputText("Player entered object " + other.gameObject.name);
                    _preventCharacterMovement = true;
                    //Color32 col = _outOfBoundsFace.material.GetColor("_Color");
                    //col.a = 255;
                    //_outOfBoundsFace.material.SetColor("_Color", col);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                //go!
                //OutputLogManager.OutputText("Player exited object " + other.gameObject.name);
                _preventCharacterMovement = false;
                //Color32 col = _outOfBoundsFace.material.GetColor("_Color");
                //col.a = 0;
                //_outOfBoundsFace.material.SetColor("_Color", col);
            }
        }

        void LateUpdate()
        {
            
               // characterController.center = new Vector3(characterController.center.x, MainCamera.transform.localPosition.y, characterController.center.z);
            
        }       

        public static void AddGrabbedObject(GameObject gameObject)
        {
            OutputLogManager.OutputText("added grabbed object " + gameObject.name);
            Instance.HeldObjects.Add(gameObject);
            Instance.HeldObjectLayers.Add(gameObject.layer);
        }

        public static void RemoveGrabbedObject(GameObject gameObject)
        {
            Instance.HeldObjects.Remove(gameObject);
            Instance.HeldObjectLayers.Remove(gameObject.layer);
        }

        private void SetHeldObjectsUninteractable()
        {            
            foreach (var item in HeldObjects)
            {
                item.layer = 30;
            }
        }

        private void SetHeldObjectsInteractable()
        {         
            for (int i = 0; i < HeldObjects.Count; i++)
            {
                HeldObjects[i].layer = HeldObjectLayers[i];
            }            
        }

        public void AddPuzzleArea(PuzzleArea puzzleArea)
        {
            ActivePuzzleAreas.Add(puzzleArea);
            UpdateLights();
        }

        public void RemovePuzzleArea(PuzzleArea puzzleArea)
        {
            ActivePuzzleAreas.Remove(puzzleArea);
            UpdateLights();
        }

        private void UpdateLights()
        {
            foreach (LightStrip lightStrip in PhysicsManager.Instance.LightStrips)
            {
                bool blnLightState = true; //enabled all lights!

                foreach (PuzzleArea puzzleArea in ActivePuzzleAreas)
                {
                    if (puzzleArea.Lights.Contains(lightStrip))
                    {
                        blnLightState = true;
                    }
                }

                lightStrip.gameObject.SetActive(blnLightState);
            }
        }

        IEnumerator DissolveFace()
        {
            float currentTime = 0f;
            float totalTime = 1f;                        

            while (currentTime < totalTime)
            {
                _outOfBoundsFace.material.SetFloat("HIDDEN_RATIO", totalTime - currentTime);
                
                currentTime += Time.deltaTime;
                yield return null;
            }

            _resetting = true;

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(UnDissolveFace());
        }

        IEnumerator UnDissolveFace()
        {
            float currentTime = 0f;
            float totalTime = 0.5f;

            while (currentTime < totalTime)
            {
                _outOfBoundsFace.material.SetFloat("HIDDEN_RATIO", currentTime*2);

                currentTime += Time.deltaTime;
                yield return null;
            }

            _resetting = true;
        }
    }
}