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

        private bool _isSlowed;

        public bool IsSlowed
        {
            get { return _isSlowed; }
            set {
                _isSlowed = value; 

                if (_isSlowed)
                {
                    Speed = Speed * 0.1f;
                }
                else
                {
                    Speed = Speed * 10;
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
                    transform.position = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2), transform.position.z);
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 4, transform.localScale.z);                    
                }
                else
                {                    
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 4, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 2), transform.position.z);
                }
            }
        }


        public void Start()
        {
            PhysicsManager.Instance.PhysicsObjects.Add(this);
            RigidBody = GetComponent<Rigidbody>();
            this.tag = "PhysicsObject";
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.PhysicsObjects.Remove(this);
        }

       
        public void FixedUpdate()
        {
            Vector3 direction;            

            if (_ticksSincePathChange > 10 && (RigidBody.transform.position - Path[_currentPathIndex]).magnitude < 0.05)
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

            direction = (Path[_currentPathIndex] - RigidBody.transform.position);
            direction.Normalize();

            RigidBody.MovePosition(RigidBody.transform.position + direction * Speed * Time.deltaTime);
            RigidBody.velocity = Speed * RigidBody.velocity.normalized;

            _ticksSincePathChange++;
        }

        private void Update()
        {            
        }
    }
}
