﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class Elevator : MonoBehaviour
    {
        public GameObject LeftDoor;
        public GameObject RightDoor;
        public AudioSource OpenDoorButtonClip;
        public AudioSource ClosedDoorButtonClip;
        public AudioSource OpenDoorClip;
        public AudioSource CloseDoorClip;
        public AudioSource StartDescentClip;

        public void Start()
        {            
            
        }

        public void OpenDoor()
        {
            StartCoroutine(OpenDoorAnimate());
        }

        public void CloseDoor()
        {
            StartCoroutine(CloseDoorAnimate());
        }       

        public IEnumerator OpenDoorAnimate()
        {
            OpenDoorButtonClip.Play();
            new WaitForSeconds(1f);

            OpenDoorClip.Play();
            
            var t = 0f;            
            var currentLeftPos = LeftDoor.transform.localPosition;
            var currentLeftScale = LeftDoor.transform.localScale;
            var currentRightPos = RightDoor.transform.localPosition;
            var currentRightScale = RightDoor.transform.localScale;

            while (t < 1)
            {
                t += Time.deltaTime / 1f;
                LeftDoor.transform.localPosition = Vector3.Lerp(currentLeftPos, new Vector3(currentLeftPos.x + 0.495f, currentLeftPos.y, currentLeftPos.z), t);
                LeftDoor.transform.localScale = Vector3.Lerp(currentLeftScale, new Vector3(currentLeftScale.x - 0.99f, currentLeftScale.y, currentLeftScale.z), t);

                RightDoor.transform.localPosition = Vector3.Lerp(currentRightPos, new Vector3(currentRightPos.x - 0.495f, currentRightPos.y, currentRightPos.z), t);
                RightDoor.transform.localScale = Vector3.Lerp(currentRightScale, new Vector3(currentRightScale.x - 0.99f, currentRightScale.y, currentRightScale.z), t);
                yield return null;
            }            
        }

        public IEnumerator CloseDoorAnimate()
        {
            ClosedDoorButtonClip.Play();
            new WaitForSeconds(1f);

            OpenDoorClip.Play();

            var t = 0f;
            var currentLeftPos = LeftDoor.transform.localPosition;
            var currentLeftScale = LeftDoor.transform.localScale;
            var currentRightPos = RightDoor.transform.localPosition;
            var currentRightScale = RightDoor.transform.localScale;

            while (t < 1)
            {
                t += Time.deltaTime / 1f;
                LeftDoor.transform.localPosition = Vector3.Lerp(currentLeftPos, new Vector3(currentLeftPos.x - 0.495f, currentLeftPos.y, currentLeftPos.z), t);
                LeftDoor.transform.localScale = Vector3.Lerp(currentLeftScale, new Vector3(currentLeftScale.x + 0.99f, currentLeftScale.y, currentLeftScale.z), t);

                RightDoor.transform.localPosition = Vector3.Lerp(currentRightPos, new Vector3(currentRightPos.x + 0.495f, currentRightPos.y, currentRightPos.z), t);
                RightDoor.transform.localScale = Vector3.Lerp(currentRightScale, new Vector3(currentRightScale.x + 0.99f, currentRightScale.y, currentRightScale.z), t);
                yield return null;
            }

            foreach (var item in AudioManager.Instance.Level1Speakers)
            {
                item.GetComponent<AudioSource>().Play();
            }

            StartDescentClip.Play();
            StartCoroutine(PlayerController.Instance.IntroMovement());

            yield return null;
        }

        public IEnumerator SplitMesh()
        {
            if (GetComponent<MeshFilter>() == null && GetComponent<SkinnedMeshRenderer>() == null)
            {
                yield return null;
            }

            if (GetComponent<Collider>())
            {
                GetComponent<Collider>().enabled = false;
            }

            Mesh M = new Mesh();
            if (GetComponent<MeshFilter>())
            {
                M = GetComponent<MeshFilter>().mesh;
            }
            else if (GetComponent<SkinnedMeshRenderer>())
            {
                M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }

            Material[] materials = new Material[0];
            if (GetComponent<MeshRenderer>())
            {
                materials = GetComponent<MeshRenderer>().materials;
            }
            else if (GetComponent<SkinnedMeshRenderer>())
            {
                materials = GetComponent<SkinnedMeshRenderer>().materials;
            }

            Vector3[] verts = M.vertices;
            Vector3[] normals = M.normals;
            Vector2[] uvs = M.uv;
            for (int submesh = 0; submesh < M.subMeshCount; submesh++)
            {

                int[] indices = M.GetTriangles(submesh);

                for (int i = 0; i < indices.Length; i += 3)
                {
                    Vector3[] newVerts = new Vector3[3];
                    Vector3[] newNormals = new Vector3[3];
                    Vector2[] newUvs = new Vector2[3];
                    for (int n = 0; n < 3; n++)
                    {
                        int index = indices[i + n];
                        newVerts[n] = verts[index];
                        newUvs[n] = uvs[index];
                        newNormals[n] = normals[index];
                    }

                    Mesh mesh = new Mesh();
                    mesh.vertices = newVerts;
                    mesh.normals = newNormals;
                    mesh.uv = newUvs;

                    mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                    GameObject GO = new GameObject("Triangle " + (i / 3));
                    GO.layer = LayerMask.NameToLayer("Particle");
                    GO.transform.position = transform.position;
                    GO.transform.rotation = transform.rotation;
                    GO.AddComponent<MeshRenderer>().material = materials[submesh];
                    GO.AddComponent<MeshFilter>().mesh = mesh;
                    GO.AddComponent<BoxCollider>();
                    Vector3 explosionPos = new Vector3(transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f), transform.position.y + UnityEngine.Random.Range(0f, 0.5f), transform.position.z + UnityEngine.Random.Range(-0.5f, 0.5f));
                    GO.AddComponent<Rigidbody>().AddExplosionForce(UnityEngine.Random.Range(100, 200), explosionPos, 3);
                    Destroy(GO, UnityEngine.Random.Range(0.0f, 1.0f));
                }
            }

            GetComponent<Renderer>().enabled = false;

            yield return new WaitForSeconds(1.0f);

            GetComponent<Collider>().enabled = true;
            GetComponent<PhysicsObject>().Reset();
            GetComponent<Renderer>().enabled = true;
        }
    }
}