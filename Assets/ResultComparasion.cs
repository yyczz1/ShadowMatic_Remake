using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultComparasion : MonoBehaviour
{
    public Camera CaptureCam;

    public Texture2D TargetTexture;
    public Color[] targetTexturePixels;

    public Vector3 Target1Pos = new Vector3(-3.00f, 2.925f, -6.23f);
    public Vector3 Target1Rot = new Vector3(0f, 0f, 0f);
    public Vector3 Target2Pos = new Vector3(-2.99f, 2.578f, -4.77f);
    public Vector3 Target2Rot = new Vector3(0.0f,0.0f,0.0f);

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("TargetTexture" + TargetTexture);
        //targetTexturePixels = TargetTexture.GetPixels(0, 0, TargetTexture.width, TargetTexture.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("return")) 
        {
            InputManager.Instance.RotateTargetList[0].position = Target1Pos;
            InputManager.Instance.RotateTargetList[0].eulerAngles = Target1Rot;
            InputManager.Instance.RotateTargetList[1].position = Target2Pos;
            InputManager.Instance.RotateTargetList[1].eulerAngles = Target2Rot;
        }
    }


    private float ChangeAngle(float degree)
    {
        if (degree < 0) Mathf.Abs(degree %= 180);
        else if (degree >= 180) degree %= 180;
        return degree;
    }
}
