using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class PhysicsObject : MonoBehaviour
    {
        public float Speed;        
        public Vector3[] Path;
        private int _currentPathIndex = 0;
        public Rigidbody RigidBody { get; set; }
        private int _pathIncrement = 1;
        private int _ticksSincePathChange = 0;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Material _material;        
        private Color _emissiveColor;
        private float _lastBassLevelUsed;
        private DateTime _lastEmission;
        public bool IgnoreMusic;

        private bool _isSlowed;

        public bool IsSlowed
        {
            get { return _isSlowed; }
            set {
                _isSlowed = value;

                if (Path.Length > 0)
                {
                    //path based, we use speed variable to manually plot path
                    if (_isSlowed)
                    {
                        Speed = Speed * 0.1f;
                    }
                    else
                    {
                        Speed = Speed * 10;
                    }
                }
                else
                {
                    //not a path based object, so we have to alter unity physics instead
                    if (_isSlowed)
                    {
                        OutputLogManager.OutputText(this.gameObject.name + " movement reduced");
                        RigidBody.velocity = RigidBody.velocity * 0.1f;
                        RigidBody.useGravity = false; //hack, really gravity should be 1/10th instead of disabled
                    }
                    else
                    {
                        OutputLogManager.OutputText(this.gameObject.name + " movement sped up");
                        RigidBody.velocity = RigidBody.velocity * 10f;
                        RigidBody.useGravity = true;
                    }
                }
                //OutputLogManager.OutputText(name + " speed now " + Speed);
            }
        }

        private bool _isGrown;

        public bool IsGrown
        {
            get { return _isGrown; }
            set
            {
                _isGrown = value;

                if (_isGrown)
                {
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 3, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 2), transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2), transform.position.z);
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 3, transform.localScale.z);
                }                
            }
        }

        private bool _isShrunk;

        public bool IsShrunk
        {
            get { return _isShrunk; }
            set
            {
                _isShrunk = value;

                if (_isShrunk)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 3), transform.position.z);
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 4, transform.localScale.z);                    
                }
                else
                {                    
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 4, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 3), transform.position.z);
                }
            }
        }


        public void Start()
        {
            PhysicsManager.Instance.PhysicsObjects.Add(this);
            RigidBody = GetComponent<Rigidbody>();
            this.tag = "PhysicsObject";

            _startPosition = transform.position;
            _startRotation = transform.rotation;

            _material = GetComponent<MeshRenderer>()?.material;

            _emissiveColor = new Color(0.2f, 0, 0);
        }

        public void Reset()
        {
            RigidBody.velocity = Vector3.zero;
            transform.position = _startPosition;
            transform.rotation = _startRotation;            
            _currentPathIndex = 0;
            if (IsShrunk)
            {
                IsShrunk = false;
            }
            if (IsGrown)
            {
                IsGrown = false;
            }
            if (IsSlowed)
            {
                IsSlowed = false;
            }
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.PhysicsObjects.Remove(this);
        }

       
        public void FixedUpdate()
        {
            Vector3 direction;

            if (Path?.Length > 0)
            {
                if (_ticksSincePathChange > 10 && (RigidBody.transform.position - RigidBody.transform.parent.position - Path[_currentPathIndex]).magnitude < 0.05f)
                {
                    _ticksSincePathChange = 0;
                    _currentPathIndex += _pathIncrement;

                    if (_currentPathIndex > Path.Length - 1)
                    {
                        _pathIncrement = -_pathIncrement;
                        _currentPathIndex += _pathIncrement;
                        _currentPathIndex += _pathIncrement;
                    }
                    else if (_currentPathIndex < 0)
                    {
                        _pathIncrement = -_pathIncrement;
                        _currentPathIndex += _pathIncrement;
                        _currentPathIndex += _pathIncrement;
                    }
                }

                direction = (Path[_currentPathIndex] - (RigidBody.transform.position - RigidBody.transform.parent.position));
                direction.Normalize();

                RigidBody.MovePosition(RigidBody.transform.position + direction * Speed * Time.deltaTime);
                RigidBody.velocity = Speed * RigidBody.velocity.normalized;

                _ticksSincePathChange++;
            }

            //if (Math.Abs(_lastBassLevelUsed - AudioManager.Instance.BassLevel / 2f) > 0.2f)
            //{
            //    _material.SetColor("_EmissiveColor", _emissiveColor * AudioManager.Instance.BassLevel / 2f);
            //    _lastBassLevelUsed = AudioManager.Instance.BassLevel / 2f;
            //}

            if (!IgnoreMusic && _material != null)
            {
                if (AudioManager.Instance.BassLevel == 2f)
                {
                    _lastEmission = DateTime.Now;
                    _material.SetColor("_EmissiveColor", _emissiveColor * AudioManager.Instance.BassLevel / 2f);
                }
                else
                {
                    if ((DateTime.Now - _lastEmission).TotalMilliseconds > 100)
                    {
                        _material.SetColor("_EmissiveColor", _emissiveColor * 0);
                    }
                }
            }


            //long ticksSinceBeat = DateTime.Now.Ticks - AudioManager.Instance.LastBeatTicks;
            //if (ticksSinceBeat < 1000)
            //{
            //    if (!_isEmitting)
            //    {
            //        _isEmitting = true;
            //        Color color = new Color(255, 255, 255);

            //        // for some reason, the desired intensity value (set in the UI slider) needs to be modified slightly for proper internal consumption
            //        //float adjustedIntensity = 0.3f;

            //        // redefine the color with intensity factored in - this should result in the UI slider matching the desired value
            //        //color *= Mathf.Pow(2.0F, adjustedIntensity);
            //        _material.SetColor("_BaseColor", color);                    
            //    }
            //}
            //else
            //{
            //    _isEmitting = false;
            //    Color color = new Color(0, 0, 0);
            //    _material.SetColor("_Color", color);
            //}
        }

        private void Update()
        {            
        }

        private void OnCollisionEnter(Collision collision)
        {
            //OutputLogManager.OutputText(this.name + " collided with " + collision.gameObject.name);
            if (Path.Length > 0 && collision.gameObject.GetComponent<PhysicsObject>() != null)
            {
                //OutputLogManager.OutputText(this.name + " collided with " + collision.gameObject.name);
                //OutputLogManager.OutputText(this.name + " reversed direction");
                //reverse direction
                _pathIncrement = -_pathIncrement;
                _currentPathIndex += _pathIncrement;
                RigidBody.velocity = Vector3.zero;
            }
        }
    }
}
