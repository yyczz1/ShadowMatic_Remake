using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if(instance == null)
                instance = new InputManager();
            return instance; 
        }
    }
    public bool canControl = true;

    public Camera WorldCam;
    public Transform RotateTarget;
    public List<Transform> RotateTargetList = new List<Transform>();
    public List<Quaternion> RotateTargetRotList = new List<Quaternion>();
    public Transform TargetParent;

    public bool isRotateBoth = false;
    public bool canRotate;
    public bool canRotateWithShadow;
    public float horizontalSpeed = 4.0f;
    public float verticalSpeed = 4.0f;

    public GameObject ScreenGO;
    private Mesh ScreenMesh;
    [SerializeField]
    private Vector3 ScreenNormal;

    float testValue = 0;

    void Awake()
    {
        instance = this;

        var goArray = GameObject.FindGameObjectsWithTag("Target");
        foreach (var go in goArray)
        {
            if (go.name.Contains("1"))
            {
                RotateTargetList.Insert(0, go.transform);
            }
            if (go.name.Contains("2"))
                RotateTargetList.Add(go.transform);
        }
        foreach (var go in goArray)
        {
            if (go.name.Contains("1"))
                RotateTargetRotList.Insert(0, go.transform.rotation);
            if (go.name.Contains("2"))
                RotateTargetRotList.Add(go.transform.rotation);
        }

        ScreenMesh = ScreenGO.GetComponent<MeshFilter>().mesh;
        ScreenNormal = ScreenMesh.normals[0];
        ScreenNormal = ScreenGO.transform.TransformDirection(ScreenNormal);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!canControl)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            canRotate = true;
        }
        if(Input.GetMouseButtonUp(0))
        {
            canRotate = false;
        }
        if(canRotate)
        {
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            float v = verticalSpeed * Input.GetAxis("Mouse Y");
            if (!isRotateBoth)
            {
                RotateTarget.Rotate(v, -h, 0);
                //Debug.Log("v " + v + " h " + h);
            }
            else
            {
                //foreach (var target in RotateTargetList)
                //{

                //Quaternion oldRot = target.transform.localRotation;
                //target.RotateAround(TargetParent.position, Vector3.up, -h);
                //target.RotateAround(TargetParent.position, Vector3.right, v);
                //target.transform.localRotation = oldRot;
                //target.Rotate(0, -h, 0);
                //}
                for (int i = 0; i < RotateTargetList.Count; i++)
                {
                    RotateTargetRotList[i] = RotateTargetList[i].rotation;
                }
                TargetParent.Rotate(v, -h, 0);
                for (int i = 0; i < RotateTargetList.Count; i++)
                {
                   RotateTargetList[i].rotation = RotateTargetRotList[i];
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            canRotateWithShadow = true;

        }
        if(Input.GetMouseButtonUp(1))
        {
            canRotateWithShadow = false;
            //Debug.Log("normal " + ScreenNormal);
        }
        if (canRotateWithShadow)
        {
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            if (!isRotateBoth)
            {
                //RotateTarget.Rotate(new Vector3(0, 0, 1), -1f * h, Space.World);
                //RotateTarget.Rotate(ScreenNormal,h,Space.World);
                RotateTarget.localRotation = Quaternion.AngleAxis(h, ScreenNormal) * RotateTarget.localRotation;
                //Debug.Log("angle " + Quaternion.AngleAxis(h, ScreenNormal).eulerAngles);
            }
            else 
            {
                for (int i = 0; i < RotateTargetList.Count; i++)
                {
                    RotateTargetRotList[i] = RotateTargetList[i].rotation;
                }
                TargetParent.Rotate(0,0,h);
                for (int i = 0; i < RotateTargetList.Count; i++)
                {
                    RotateTargetList[i].rotation = RotateTargetRotList[i];
                }
                //foreach (var target in RotateTargetList)
                //{
                //    //target.localRotation = Quaternion.AngleAxis(h, TargetParent.position) * target.localRotation;
                //    Quaternion oldRot = target.transform.localRotation;
                //    target.RotateAround(TargetParent.position, Vector3.forward, -h);
                //    target.transform.localRotation = oldRot;
                //    //target.Rotate(0, -h, 0);
                //}
            }
        }

        if (Input.GetKeyDown("space"))
        {
            if (isRotateBoth)
            {
                testValue += 1;
                TargetParent.localRotation = Quaternion.AngleAxis(testValue, ScreenNormal) * TargetParent.localRotation;
            }
            else
            {
                testValue += 1;
                RotateTargetList[0].localRotation = Quaternion.Euler(RotateTargetList[0].localEulerAngles + new Vector3(0,1,0) * testValue);
            }
        }
    }

    public void PushDifferentResultToList(float degree)
    {
        //Debug.Log(" isRotateBoth " + isRotateBoth + " degree " + degree);
        if (isRotateBoth)
        {
            TargetParent.localRotation = Quaternion.AngleAxis(degree, ScreenNormal) * TargetParent.localRotation;
        }
    }
}
