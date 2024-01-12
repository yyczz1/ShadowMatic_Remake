using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class ResultComparasion : MonoBehaviour
{
    public Camera CaptureCam;

    public Texture2D TargetTexture;
    public Color[] targetTexturePixels;

    #region snapshot comparasion
    public SnapShotComparasion SnapShotComparasion;
    public List<Texture2D> successfulSnapshots;
    [HideInInspector] public List<List<Quaternion>> successfulRotations;

    public float resultValue;
    public int resultIndex;

    #endregion

    #region compare angle test,should change to so
    public ResultDataSO Level1DataSO;

    //public Vector3 Target1WorldPos = new Vector3(-3.008f, 2.945f, -6.224f);
    //public Vector3 Target1WorldRot = new Vector3(0f, 0f, 0f);
    //public Vector3 Target2WorldPos = new Vector3(-2.99f, 2.555f, -4.776f);
    //public Vector3 Target2WorldRot = new Vector3(0.0f,0.0f,0.0f);

    //public Vector3 TargetParentWorldPos = new Vector3(-3.0f, 2.75f, -5.5f);
    //public Vector3 TargetParentWorldRot = new Vector3(-76.556f, -7.993f, 6.951f);

    private Quaternion CurrentTargetParentRotationQuaternion => Quaternion.Euler(InputManager.Instance.TargetParent.eulerAngles);
    //private Quaternion TargetParentRotationQuaternion => Quaternion.Euler(TargetParentWorldRot);

    private Quaternion CurrentTarget1RotationQuaternion => Quaternion.Euler(InputManager.Instance.RotateTargetList[0].eulerAngles);
    //private Quaternion Target1RotationQuaternion => Quaternion.Euler(Target1WorldRot);

    private Quaternion CurrentTarget2RotationQuaternion => Quaternion.Euler(InputManager.Instance.RotateTargetList[1].eulerAngles);
    //private Quaternion Target2RotationQuaternion => Quaternion.Euler(Target2WorldRot);
    //private float Threshold => 10;

    //public ResultTransform ResultTransforms = new ResultTransform();
   // public TargetTransform[] TargetTransforms = new TargetTransform[2];

    private bool isOutput = false;
    public bool isSuccess = false;
    public float lerpValue = 0f;
    private Vector3 oldPos1;
    private Vector3 oldPos2;
    private Vector3 newPos1;
    private Vector3 newPos2;

    private Quaternion oldRot1;
    private Quaternion oldRot2;
    private Quaternion newRot1;
    private Quaternion newRot2;

    public bool isParentSolved;
    public bool is1Solved;
    public bool is2Solved;
    public bool is1PosSolved;
    public bool is2PosSolved;

    public float differenceParentSolved;
    public float difference1Solved;
    public float difference2Solved;
    public float difference1_1Solved;
    public float difference2_1Solved;
    private float differenceRotTarget1;
    private float differenceRotTarget2;

    public float differencePos1Solved;
    public float differencePos2Solved;
    public float differencePos1_1Solved;
    public float differencePos2_1Solved;
    private float differencePosTarget1;
    private float differencePosTarget2;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("TargetTexture" + TargetTexture);
        //targetTexturePixels = TargetTexture.GetPixels(0, 0, TargetTexture.width, TargetTexture.height);
        (successfulSnapshots, successfulRotations) = SnapShotComparasion.LoadSnapshots(
                Path.Combine("Assets/Resources/Snapshots", $"{SceneManager.GetActiveScene().name}_snapshot"));
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown("return")) 
        //{
        //    InputManager.Instance.TargetParent.position = TargetParentWorldPos;
        //    InputManager.Instance.TargetParent.eulerAngles = Level1DataSO.ParentRotationAll;
        //    InputManager.Instance.RotateTargetList[0].position = Target1WorldPos;
        //    InputManager.Instance.RotateTargetList[0].eulerAngles = Level1DataSO.ResultRotation1;
        //    InputManager.Instance.RotateTargetList[1].position = Target2WorldPos;
        //    InputManager.Instance.RotateTargetList[1].eulerAngles = Level1DataSO.ResultRotation2;
        //}
        (resultValue ,resultIndex)= CalculateCurrentResult();
        if (is1Solved && is2Solved && is1PosSolved && is2PosSolved)
        {
            if (!isSuccess) 
            {
                oldPos1 = InputManager.Instance.RotateTargetList[0].position;
                oldRot1 = InputManager.Instance.RotateTargetList[0].rotation;
                //newRot1 = difference1Solved < difference1_1Solved ? Quaternion.Euler(Level1DataSO.ResultRotation1) : Quaternion.Euler(Level1DataSO.ResultRotation1_1);
                //newRot2 = difference2Solved < difference2_1Solved ? Quaternion.Euler(Level1DataSO.ResultRotation2) : Quaternion.Euler(Level1DataSO.ResultRotation2_1);
                if (difference1Solved < difference1_1Solved)
                {
                    newRot1 = Quaternion.Euler(Level1DataSO.ResultRotation1);
                    newPos1 = Level1DataSO.ResultPosition1;
                }
                else
                {
                    newRot1 = Quaternion.Euler(Level1DataSO.ResultRotation1_1);
                    newPos1 = Level1DataSO.ResultPosition1_1;
                }

                if (difference2Solved < difference2_1Solved)
                {
                    newRot2 = Quaternion.Euler(Level1DataSO.ResultRotation2);
                    newPos2 = Level1DataSO.ResultPosition2;
                }
                else
                {
                    newRot2 = Quaternion.Euler(Level1DataSO.ResultRotation2_1);
                    newPos2 = Level1DataSO.ResultPosition2_1;
                }

                oldPos2 = InputManager.Instance.RotateTargetList[1].position;
                oldRot2 = InputManager.Instance.RotateTargetList[1].rotation;

                //newPos1 = differencePos1Solved < differencePos1_1Solved ? Level1DataSO.ResultPosition1 : Level1DataSO.ResultPosition1_1;
                //newPos2 = differencePos2Solved < differencePos2_1Solved ? Level1DataSO.ResultPosition2 : Level1DataSO.ResultPosition2_1;
            }
            isSuccess = true;
            //InputManager.Instance.canControl = false;
            StartCoroutine(MergeWhenSuccess());
        }
        if (isSuccess && lerpValue <= 1)
        {
            lerpValue = lerpValue + Time.deltaTime * 0.8f;
            InputManager.Instance.RotateTargetList[0].rotation = Quaternion.Slerp(oldRot1, newRot1, lerpValue);
            InputManager.Instance.RotateTargetList[1].rotation = Quaternion.Slerp(oldRot2, newRot2, lerpValue);

            InputManager.Instance.RotateTargetList[0].position = Vector3.Lerp(oldPos1, newPos1, lerpValue);
            InputManager.Instance.RotateTargetList[1].position = Vector3.Lerp(oldPos2, newPos2, lerpValue);
            //Debug.Log("target1 pos and euler " + InputManager.Instance.RotateTargetList[0].position + InputManager.Instance.RotateTargetList[0].eulerAngles + "target2 pos and euler " + InputManager.Instance.RotateTargetList[1].position + InputManager.Instance.RotateTargetList[1].eulerAngles);
        }

        if (Input.GetKeyDown("m"))
        {
            OutputResult();
        }

        CompareAngle();
    }

    #region snapshot comparasion
    private (float, int) CalculateCurrentResult()
    {
        int bestResultIndex = -1;
        float bestResult = -1.0f;
        for (int i = 0; i < successfulSnapshots.Count; i++)
        {
            float snapshotsComparisonResultPercent =
                SnapShotComparasion.CompareSnapshotWithProjection(successfulSnapshots[i]);

            if (snapshotsComparisonResultPercent >= bestResult)
            {
                bestResult = snapshotsComparisonResultPercent;
                bestResultIndex = i;
            }
        }
        //Debug.Log("bestResult" + bestResult + " bestResultIndex " + bestResultIndex);
        return (bestResult, bestResultIndex);
    }
    #endregion

    #region angle comparasion
    private void CompareAngle()
    {
        differenceParentSolved = Quaternion.Angle(CurrentTargetParentRotationQuaternion, Quaternion.Euler(Level1DataSO.ParentRotationAll));

        difference1Solved = Quaternion.Angle(CurrentTarget1RotationQuaternion, Quaternion.Euler(Level1DataSO.ResultRotation1));
        difference2Solved = Quaternion.Angle(CurrentTarget2RotationQuaternion, Quaternion.Euler(Level1DataSO.ResultRotation2));
        difference1_1Solved = Quaternion.Angle(CurrentTarget1RotationQuaternion, Quaternion.Euler(Level1DataSO.ResultRotation1_1));
        difference2_1Solved = Quaternion.Angle(CurrentTarget2RotationQuaternion, Quaternion.Euler(Level1DataSO.ResultRotation2_1));
        differenceRotTarget1 = difference1Solved < difference1_1Solved ? difference1Solved : difference1_1Solved;
        differenceRotTarget2 = difference2Solved < difference2_1Solved ? difference2Solved : difference2_1Solved;

        differencePos1Solved = Vector3.Distance(InputManager.Instance.RotateTargetList[0].position, Level1DataSO.ResultPosition1);
        differencePos2Solved = Vector3.Distance(InputManager.Instance.RotateTargetList[1].position, Level1DataSO.ResultPosition2);
        differencePos1_1Solved = Vector3.Distance(InputManager.Instance.RotateTargetList[0].position, Level1DataSO.ResultPosition1_1);
        differencePos2_1Solved = Vector3.Distance(InputManager.Instance.RotateTargetList[1].position, Level1DataSO.ResultPosition2_1);
        differencePosTarget1 = differencePos1Solved < differencePos1_1Solved ? differencePos1Solved : differencePos1_1Solved;
        differencePosTarget2 = differencePos2Solved < differencePos2_1Solved ? differencePos2Solved : differencePos2_1Solved;

        isParentSolved = differenceParentSolved < Level1DataSO.ParentThreshold;
        is1Solved = differenceRotTarget1 < Level1DataSO.Threshold1;
        is2Solved = differenceRotTarget2 < Level1DataSO.Threshold2;
        is1PosSolved = differencePosTarget1 < Level1DataSO.PosThreshold1;
        is2PosSolved = differencePosTarget2 < Level1DataSO.PosThreshold2;
    }
    #endregion

    #region output result
    private void OutputResult()
    {
        if (lerpValue >= 1f && !isOutput)
        {
            isOutput = true;
            //TargetTransform[] TargetTransforms;
            ResultTransform resultTransforms = new ResultTransform();
            for (int i = 0; i < 360; i++)
            {
                //TargetTransforms = new TargetTransform[2];
                InputManager.Instance.PushDifferentResultToList(i);
                TargetTransform target1 = new TargetTransform();
                target1.Degree = i;
                //target1.ResultPosition = InputManager.Instance.RotateTargetList[0].position;
                //target1.ResultRotation = InputManager.Instance.RotateTargetList[0].rotation;
                target1.ResultPosition = "(" + InputManager.Instance.RotateTargetList[0].position.x.ToString("f2") + "," + InputManager.Instance.RotateTargetList[0].position.y.ToString("f2") + "," + InputManager.Instance.RotateTargetList[0].position.z.ToString("f2") + ")";
                target1.ResultRotation = "(" + InputManager.Instance.RotateTargetList[0].rotation.x.ToString("f2") + "," + InputManager.Instance.RotateTargetList[0].rotation.y.ToString("f2") + "," + InputManager.Instance.RotateTargetList[0].rotation.z.ToString("f2") + "," + InputManager.Instance.RotateTargetList[0].rotation.w.ToString("f2") + ")";
                //Debug.Log("target1" + target1.ResultPosition);
                //TargetTransforms[0] = target1;
                //TargetTransforms[0].DebugToString();

                TargetTransform target2 = new TargetTransform();
                target2.Degree = i;
                target2.ResultPosition = "(" + InputManager.Instance.RotateTargetList[1].position.x.ToString("f2") + "," + InputManager.Instance.RotateTargetList[1].position.y.ToString("f2") + "," + InputManager.Instance.RotateTargetList[1].position.z.ToString("f2") + ")";
                target2.ResultRotation = "(" + InputManager.Instance.RotateTargetList[1].rotation.x.ToString("f2") + "," + InputManager.Instance.RotateTargetList[1].rotation.y.ToString("f2") + "," + InputManager.Instance.RotateTargetList[1].rotation.z.ToString("f2") + "," + InputManager.Instance.RotateTargetList[1].rotation.w.ToString("f2") + ")";
                //TargetTransforms[1] = target2;


                resultTransforms.ResultTransforms.Add(target1);
                resultTransforms.ResultTransforms.Add(target2);

                //Debug.Log("ResultTransform string " + JsonUtility.ToJson(ResultTransform));
            }
            string path = Application.dataPath + "/ResultFile";
            DirectoryInfo buildOutInfo = new DirectoryInfo(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Debug.Log("output string " + JsonUtility.ToJson(resultTransforms));
            File.WriteAllText(Path.Combine(path, "ResultFile.json"), JsonUtility.ToJson(resultTransforms));
        }
    }
    #endregion

    IEnumerator MergeWhenSuccess()
    {
        yield return null;
    }

    private float ChangeAngle(float degree)
    {
        if (degree < 0) Mathf.Abs(degree %= 180);
        else if (degree >= 180) degree %= 180;
        return degree;
    }
}


[System.Serializable]
public class TargetTransform
{
    //public string Name;
    public int Degree;
    public string ResultPosition;
    public string ResultRotation;
    //public float[] ResultPosition = new float[3];
    //public float[] ResultRotation = new float[4];
    //public Vector3 ResultPosition;
    //public Quaternion ResultRotation;
};

[System.Serializable]
public class ResultTransform
{
    public List<TargetTransform> ResultTransforms = new List<TargetTransform>();
};
