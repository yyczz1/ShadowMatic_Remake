using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCamera : MonoBehaviour
{
    private RenderTexture _renderTexture;
    private Camera _camera;
    public Shader BlackAndWhiteShader;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _renderTexture = _camera.targetTexture;
        //_camera.RenderWithShader(BlackAndWhiteShader,null);
        //_camera.SetReplacementShader(BlackAndWhiteShader, null);
    }


    public RenderTexture GetRenderTarget()
    {
        if (!_renderTexture)
        {
            Debug.LogWarning("There is no projector camera render target!", this);
        }
        return _renderTexture;
    }
}
