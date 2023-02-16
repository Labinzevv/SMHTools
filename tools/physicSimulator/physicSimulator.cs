using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class physicSimulator : MonoBehaviour
{
    public bool createFromPrefab = false;
    public bool createBoxCollider = false;
    public bool createNewRoot = true; 
    bool createNewParent = true;
    public GameObject[] prefab;
    [Header("Count objects")]
    public int objCount = 10;
    public int maxIterations = 50;
    [Header("Scale objects")]
    public float minScaleObj = 0.1f;
    public float maxScaleObj = 1f;
    [Header("Rotate objects")]
    public float minRotateObj = -360f;
    public float maxRotateObj = 360f;
    [Header("Position objects")]
    public float minPositionObj = -1;
    public float maxPositionObj = 1;

   
    Rigidbody rb;
    GameObject rootObj;
    GameObject objSim;
    GameObject parentObj;

    public void General()
    {
        if (createFromPrefab == true)
        {
            if (prefab.Length > 0)
            {
                if (createNewRoot == true)
                {
                    createNewParent = true;
                    CreateNewRoot();
                }
                if (rootObj != null)
                {
                    for (int i = 0; i < objCount; i++)
                    {
                        int rand = Random.Range(0, prefab.Length);
                        objSim = Instantiate(prefab[rand]);
                        ObjSimSetting();
                        PhysicSim();

                        if (createNewParent == true)
                        {
                            CreateNewParent();
                        }
                        objSim.transform.parent = parentObj.transform;
                    }
                }
                else
                {
                    Debug.LogError("Create new root");
                }
            }
            else
            {
                Debug.LogError("Add prefab");
            }
        }
        else
        {
            if (createNewRoot == true)
            {
                createNewParent = true;
                CreateNewRoot();
            }
            if (rootObj != null)
            {
                for (int i = 0; i < objCount; i++)
                {
                    objSim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    ObjSimSetting();
                    PhysicSim();

                    if (createNewParent == true)
                    {
                        CreateNewParent();
                    }
                    objSim.transform.parent = parentObj.transform;
                }
            }
            else
            {
                Debug.LogError("Create new root");
            }
        }
    }
    void CreateNewRoot()
    {
        rootObj = new GameObject("root");
        rootObj.transform.position = transform.position;
        rootObj.transform.parent = transform;
        createNewRoot = false;
    }
    void CreateNewParent()
    {
        parentObj = new GameObject("parent");
        parentObj.transform.parent = rootObj.transform;
        parentObj.transform.position = objSim.transform.position;
        createNewParent = false;
    }
    void ObjSimSetting()
    {
        float X = Random.Range(minPositionObj, maxPositionObj);
        float Z = Random.Range(minPositionObj, maxPositionObj);
        float posX = transform.position.x + X;
        float posY = transform.position.y - 0.5f;
        float posZ = transform.position.z + Z;
        objSim.transform.position = new Vector3(posX, posY, posZ);

        float rotX = Random.Range(minRotateObj, maxRotateObj);
        float rotY = Random.Range(minRotateObj, maxRotateObj);
        float rotZ = Random.Range(minRotateObj, maxRotateObj);
        float rotW = Random.Range(minRotateObj, maxRotateObj);
        objSim.transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);

        float scaleX = Random.Range(minScaleObj, maxScaleObj);
        float scaleY = Random.Range(minScaleObj, maxScaleObj);
        float scaleZ = Random.Range(minScaleObj, maxScaleObj);
        objSim.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        objSim.AddComponent<Rigidbody>();
        if (createBoxCollider == true)
        {
            objSim.AddComponent<BoxCollider>();
        }
    }
    void PhysicSim()
    {
        Physics.autoSimulation = false;
        for (int k = 0; k < maxIterations; k++)
        {
            Physics.Simulate(Time.fixedDeltaTime);
        }
        Physics.autoSimulation = true;
        Selection.activeTransform = rootObj.transform;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
