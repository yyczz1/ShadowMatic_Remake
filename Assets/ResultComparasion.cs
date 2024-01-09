using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultComparasion : MonoBehaviour
{
    public Camera CaptureCam;

    public Texture2D TargetTexture;
    public Color[] targetTexturePixels;

    #region compare angle test
    public Vector3 Target1WorldPos = new Vector3(-3.008f, 2.945f, -6.224f);
    public Vector3 Target1WorldRot = new Vector3(0f, 0f, 0f);
    public Vector3 Target2WorldPos = new Vector3(-2.99f, 2.555f, -4.776f);
    public Vector3 Target2WorldRot = new Vector3(0.0f,0.0f,0.0f);

    public Vector3 TargetParentWorldPos = new Vector3(-3.0f, 2.75f, -5.5f);
    public Vector3 TargetParentWorldRot = new Vector3(-76.556f, -7.993f, 6.951f);

    private Quaternion CurrentTargetParentRotationQuaternion => Quaternion.Euler(InputManager.Instance.TargetParent.eulerAngles);
    private Quaternion TargetParentRotationQuaternion => Quaternion.Euler(TargetParentWorldRot);

    private Quaternion CurrentTarget1RotationQuaternion => Quaternion.Euler(InputManager.Instance.RotateTargetList[0].eulerAngles);
    private Quaternion Target1RotationQuaternion => Quaternion.Euler(Target1WorldRot);

    private Quaternion CurrentTarget2RotationQuaternion => Quaternion.Euler(InputManager.Instance.RotateTargetList[1].eulerAngles);
    private Quaternion Target2RotationQuaternion => Quaternion.Euler(Target2WorldRot);
    private float Threshold => 10;

    public bool isParentSolved;
    public bool is1Solved;
    public bool is2Solved;

    public float differenceParentSolved;
    public float difference1Solved;
    public float difference2Solved;
    #endregion

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
            InputManager.Instance.TargetParent.position = TargetParentWorldPos;
            InputManager.Instance.TargetParent.eulerAngles = TargetParentWorldRot;
            InputManager.Instance.RotateTargetList[0].position = Target1WorldPos;
            InputManager.Instance.RotateTargetList[0].eulerAngles = Target1WorldRot;
            InputManager.Instance.RotateTargetList[1].position = Target2WorldPos;
            InputManager.Instance.RotateTargetList[1].eulerAngles = Target2WorldRot;
        }
        //CompareAngle();
    }

    private void CompareAngle()
    {
        isParentSolved = Quaternion.Angle(CurrentTargetParentRotationQuaternion, TargetParentRotationQuaternion) < Threshold;
        is1Solved = Quaternion.Angle(CurrentTarget1RotationQuaternion, Target1RotationQuaternion) < Threshold;
        is2Solved = Quaternion.Angle(CurrentTarget2RotationQuaternion, Target2RotationQuaternion) < Threshold;

        differenceParentSolved = Quaternion.Angle(CurrentTargetParentRotationQuaternion, TargetParentRotationQuaternion);
        difference1Solved = Quaternion.Angle(CurrentTarget1RotationQuaternion, Target1RotationQuaternion);
        difference2Solved = Quaternion.Angle(CurrentTarget2RotationQuaternion, Target2RotationQuaternion);
    }

    private float ChangeAngle(float degree)
    {
        if (degree < 0) Mathf.Abs(degree %= 180);
        else if (degree >= 180) degree %= 180;
        return degree;
    }
}
