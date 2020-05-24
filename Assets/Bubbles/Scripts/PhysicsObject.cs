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

        public void Start()
        {
            PhysicsManager.Instance.PhysicsObjects.Add(this);
            _rigidBody = GetComponent<Rigidbody>();
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.PhysicsObjects.Remove(this);
        }

       
        public void FixedUpdate()
        {
            Vector3 direction;

            if ((_rigidBody.transform.position - Path[_currentPathIndex]).magnitude < 0.01)
            {                
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
        }

        private void Update()
        {            
        }
    }
}
