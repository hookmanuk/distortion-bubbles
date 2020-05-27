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
        private Rigidbody _rigidBody;
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
                    transform.localScale = transform.localScale * 3;
                }
                else
                {
                    transform.localScale = transform.localScale / 3;
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
                    transform.localScale = transform.localScale / 3;
                }
                else
                {
                    transform.localScale = transform.localScale * 3;
                }
            }
        }


        public void Start()
        {
            PhysicsManager.Instance.PhysicsObjects.Add(this);
            _rigidBody = GetComponent<Rigidbody>();
            this.tag = "PhysicsObject";
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.PhysicsObjects.Remove(this);
        }

       
        public void FixedUpdate()
        {
            Vector3 direction;            

            if (_ticksSincePathChange > 10 && (_rigidBody.transform.position - Path[_currentPathIndex]).magnitude < 0.05)
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

            direction = (Path[_currentPathIndex] - _rigidBody.transform.position);
            direction.Normalize();

            _rigidBody.MovePosition(_rigidBody.transform.position + direction * Speed * Time.deltaTime);
            _rigidBody.velocity = Speed * _rigidBody.velocity.normalized;

            _ticksSincePathChange++;
        }

        private void Update()
        {            
        }
    }
}
