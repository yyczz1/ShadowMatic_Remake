using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
