using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using InTheShadow;

public class SnapShotComparasion : MonoBehaviour
{
    [Serializable]
    public struct SnapshotData
    {
        public int width;
        public int height;
        public TextureFormat textureFormat;
        public byte[] rawTextureData;
        public List<Quaternion> rotations;
    }

    public CaptureCamera projectorCamera;

    public ComputeShader comparisonShader;
    public ComputeShader rotateShader;
    public float startDegree = 0.0f;
    public float endDegree = 180.0f;
    public float stepDegree = 5.0f;

    private RenderTexture _cameraRT;
    private ComputeBuffer _comparisonShaderBuffer;
    public float[] _comparisonResult;
    private int _kernel;
    private uint _threadsX;
    private uint _threadsY;

    private void Start()
    {
        if (!projectorCamera)
        {
            Debug.LogWarning("Projection camera render texture isn't set!", this);
        }

        _kernel = comparisonShader.FindKernel("CSMain");
        comparisonShader.GetKernelThreadGroupSizes(_kernel, out _threadsX, out _threadsY, out _);

        _cameraRT = projectorCamera.GetRenderTarget();
        Debug.Log("_threadsX" + _threadsX+"cameraRT"+_cameraRT);

        _comparisonShaderBuffer = new ComputeBuffer(_cameraRT.width * _cameraRT.height, sizeof(float));
        comparisonShader.SetBuffer(_kernel, "Result", _comparisonShaderBuffer);
        comparisonShader.SetInt("Size", _cameraRT.width);

        comparisonShader.SetTexture(_kernel, "Camera", _cameraRT);

        _comparisonResult = new float[_cameraRT.width * _cameraRT.height];
    }

    public float CompareSnapshotWithProjection(Texture2D snapshot)
    {
        comparisonShader.SetTexture(_kernel, "Sample", snapshot);

        comparisonShader.Dispatch(
            _kernel,
            (int)(_cameraRT.width / _threadsX),
            (int)(_cameraRT.height / _threadsY),
            1);


        _comparisonShaderBuffer.GetData(_comparisonResult);

        float sum = 0.0f;
        foreach (var value in _comparisonResult)
        {
            sum += value;
        }

        return 1.0f - sum / (_cameraRT.width * _cameraRT.height);
    }

    public (List<Texture2D>, List<List<Quaternion>>) LoadSnapshots(string relatedPathToFile)
    {
        List<Texture2D> snapshots = new List<Texture2D>();
        List<List<Quaternion>> rotations = new List<List<Quaternion>>();

        int i = 1;
        string filePathWithNumber = relatedPathToFile + "_" + i;
        while (File.Exists(filePathWithNumber))
        {

            using Stream stream = new FileStream(filePathWithNumber, FileMode.Open, FileAccess.Read, FileShare.Read);
            ISerializer binarySerialize = new BinaryJsonSerializer();
            SnapshotData snapshotData = binarySerialize.Deserialize<SnapshotData>(stream);

            snapshots.Add(new Texture2D(snapshotData.width, snapshotData.height, snapshotData.textureFormat, false));
            snapshots.Last().LoadRawTextureData(snapshotData.rawTextureData);
            snapshots.Last().Apply();

            rotations.Add(new List<Quaternion>());
            rotations.Last().AddRange(snapshotData.rotations);

            i++;
            filePathWithNumber = relatedPathToFile + "_" + i;
        }

        return (snapshots, rotations);
    }

    public void MakeSnapshot()
    {
        Texture2D snapshot = ConvertToTexture2D(projectorCamera.GetRenderTarget());

        string filename = $"{SceneManager.GetActiveScene().name}_snapshot";
        string path = "Assets/Resources/Snapshots";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        List<Quaternion> rotations = InputManager.Instance.GetAllRotations();

        RenderTexture rotationResult = new RenderTexture(snapshot.width, snapshot.height, 0)
        {
            enableRandomWrite = true
        };
        rotationResult.Create();

        rotateShader.SetTexture(_kernel, "Result", rotationResult);
        rotateShader.SetTexture(_kernel, "Texture", snapshot);
        rotateShader.SetFloat("Width", snapshot.width);
        rotateShader.SetFloat("Height", snapshot.height);

        for (float degree = Mathf.Min(startDegree, endDegree); degree <= Mathf.Max(startDegree, endDegree); degree += stepDegree)
        {
            rotateShader.SetFloat("Sin", Mathf.Sin(degree));
            rotateShader.SetFloat("Cos", Mathf.Cos(degree));
            rotateShader.SetFloat("Rad", Mathf.Deg2Rad * degree);
            Debug.Log("width" + (float)snapshot.width+ "_threadsX"+ _threadsX);
            rotateShader.Dispatch(
                _kernel,
                Mathf.CeilToInt((float)snapshot.width / _threadsX),
                Mathf.CeilToInt((float)snapshot.height / _threadsY),
                1);

            Texture2D rotatedTexture = ConvertToTexture2D(rotationResult);

            SaveSnapshotAsRawData(rotatedTexture, rotations, path, filename);
        }
    }

    private static Texture2D ConvertToTexture2D(RenderTexture renderTexture)
    {
        RenderTexture temp = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, snapshot.width, snapshot.height), 0, 0);
        snapshot.Apply();
        RenderTexture.active = temp;
        return snapshot;
    }

    private static void SaveSnapshotToPNG(Texture2D snapshot, string filePath)
    {
        byte[] bytes = snapshot.EncodeToPNG();
        File.WriteAllBytes(filePath + ".png", bytes);
    }

    private static void SaveSnapshotAsRawData(Texture2D snapshot, List<Quaternion> rotations, string relatedPath, string filename)
    {
        SnapshotData snapshotData;
        snapshotData.width = snapshot.width;
        snapshotData.height = snapshot.height;
        snapshotData.textureFormat = snapshot.format;
        snapshotData.rawTextureData = snapshot.GetRawTextureData();
        snapshotData.rotations = rotations;

        string filePathWithNumber = GenerateFileNumber(Path.Combine(relatedPath, filename));
        using Stream stream = new FileStream(filePathWithNumber, FileMode.CreateNew, FileAccess.Write, FileShare.None);

        ISerializer binarySerialize = new BinaryJsonSerializer();
        binarySerialize.Serialize(stream, snapshotData);

        SaveSnapshotToPNG(snapshot, filePathWithNumber);
    }

    private static string GenerateFileNumber(string filePath)
    {
        int i = 1;
        string filePathWithNumber = filePath + "_" + i;
        while (File.Exists(filePathWithNumber))
        {
            i++;
            filePathWithNumber = filePath + "_" + i;
        }

        return filePathWithNumber;
    }

    private void OnDestroy()
    {
        _comparisonShaderBuffer.Release();
    }
}
