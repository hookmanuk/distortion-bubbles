using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class TeleportRoom : PhysicsObject
    {
        public TeleportRoom()
        {
            IgnoreMusic = true;
            IgnoresGravityFlip = true;
        }

        public GameObject Teleport1;
        public GameObject Teleport2;
        public List<TeleportDRoom> DRooms;

        private bool _is1Triggered;
        private bool _is2Triggered;
        private int _currentDRoom;
        private bool _initDone = false;

        //new private void Start()
        //{
        //    base.Start();
        //    UpdateDRooms();
        //}

        public void Update()
        {
            if (!_initDone)
            {
                _initDone = true;
                UpdateDRooms();
            }
        }

        public override void Reset()
        {
            _currentDRoom = 0;
            UpdateDRooms();
            foreach (var room in DRooms)
            {
                foreach (var item in room.MyPhysicsObjects)
                {
                    item.transform.localPosition = item.LocalPosition;
                }
            }
        }

        public void Trigger1()
        {
            if (!_is1Triggered && !_is2Triggered)
            {
                _is1Triggered = true; //entered 1st trigger
            }
            else if (_is1Triggered && !_is2Triggered)
            {
                _is1Triggered = false; //entered then left 1st trigger
            }
            else if (!_is1Triggered && _is2Triggered)
            {                
                //entered through 2 then triggered 1 - switch DRooms - 1
                _currentDRoom--;
                if (_currentDRoom < 0)
                {
                    _currentDRoom = DRooms.Count - 1;
                }
                UpdateDRooms();

                //reset triggers so start from scratch
                _is1Triggered = false;
                _is2Triggered = false;
            }
        }

        public void Trigger2()
        {
            if (!_is1Triggered && !_is2Triggered)
            {
                _is2Triggered = true; //entered 2nd trigger
            }
            else if (!_is1Triggered && _is2Triggered)
            {
                _is2Triggered = false; //entered then left 2nd trigger
            }
            else if (_is1Triggered && !_is2Triggered)
            {
                //entered through 1 then triggered 2 - switch DRooms + 1
                _currentDRoom++;
                if (_currentDRoom > DRooms.Count - 1)
                {
                    _currentDRoom = 0;
                }
                UpdateDRooms();

                //reset triggers so start from scratch
                _is1Triggered = false;
                _is2Triggered = false;
            }
        }

        private void UpdateDRooms()
        {
            for (int i = 1; i <= DRooms.Count; i++)
            {
                TeleportDRoom dRoom = DRooms[i-1];
                dRoom.transform.position = new Vector3(dRoom.StartPosition.x, dRoom.StartPosition.y, dRoom.StartPosition.z + (i * 1000));                
            }
            
            DRooms[_currentDRoom].transform.position = new Vector3(DRooms[_currentDRoom].StartPosition.x, DRooms[_currentDRoom].StartPosition.y, DRooms[_currentDRoom].StartPosition.z);            
        }
    }
}
