using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class KeyObject : MonoBehaviour
    {
        public Rigidbody RigidBody { get; set; }
        XRGrabInteractable m_GrabInteractable;
        public XRInteractionManager InteractionManager;

        public void Start()
        {            
            RigidBody = GetComponent<Rigidbody>();
            m_GrabInteractable = GetComponent<XRGrabInteractable>();
            m_GrabInteractable.onSelectEnter.AddListener(OnGrabbed);
            m_GrabInteractable.onSelectExit.AddListener(OnReleased);            
        }

        private void OnGrabbed(XRBaseInteractor obj)
        {
            PlayerController.AddGrabbedObject(gameObject);            
        }

        void OnReleased(XRBaseInteractor obj)
        {
            PlayerController.RemoveGrabbedObject(gameObject);
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
                GetComponent<PhysicsObject>().Reset();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            OutputLogManager.OutputText(this.name + " triggered with " + other.gameObject.name);
            if (other.gameObject.CompareTag("Forcefield"))
            {
                if (PlayerController.Instance.HeldObjects.Contains(gameObject))
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    GetComponent<PhysicsObject>().Reset();
                }
            }
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
