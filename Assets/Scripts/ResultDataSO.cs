using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 1)]
public class ResultDataSO : ScriptableObject
{

    [Header("Model1")]
    public GameObject Model1;
    public bool HasModel1FlipSet = true;
    public float Threshold1;
    public float PosThreshold1;
    public Vector3 StartRotation1;
    public Vector3 ResultRotation1;
    public Vector3 ResultRotation1_1;
    public Vector3 ResultPosition1;
    public Vector3 ResultPosition1_1;

    [Header("Model1")]
    public GameObject Model2;
    public bool HasModel2FlipSet = true;
    public float Threshold2;
    public float PosThreshold2;
    public Vector3 StartRotation2;
    public Vector3 ResultRotation2;
    public Vector3 ResultRotation2_1;
    public Vector3 ResultPosition2;
    public Vector3 ResultPosition2_1;

    [Header("Respective models rotation")]
    public float ParentThreshold;
    public Vector3 ParentRotationAll;
    public Vector3 ParentRotationAll_1;
}
