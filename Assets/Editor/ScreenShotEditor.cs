using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ScreenShotEditor : EditorWindow
{
    private static bool isFindCams = false;
    public static Camera CaptureCam;
    public static List<Camera> AllCams;

    private GUIStyle _buttonStyle;
    private static bool _isMakingScreenshotsNow;
    private bool _hasErrors;

    [MenuItem("Tools/Screen Taker/Show Window")]
    protected static void ShowWindow()
    {
        var window = (ScreenShotEditor)GetWindow(typeof(ScreenShotEditor));
        window.autoRepaintOnSceneChange = true;
        window.titleContent = new GUIContent("Screen Taker");
        window.Show();
    }
    
    protected void OnGUI()
    {
        _hasErrors = false;

        OnGUICameraInput();
        OnGUITakeButton();
    }

    private void OnGUICameraInput()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Camera", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        CaptureCam = (Camera)EditorGUILayout.ObjectField(CaptureCam, typeof(Camera), true);
        if (CaptureCam == null)
        {
            EditorGUILayout.HelpBox("Camera is not selected.", MessageType.Error);
            _hasErrors = true;
            if (isFindCams)
            {
                isFindCams = false;
            }
        }
        else
        {
            if (!isFindCams)
            {
                isFindCams = true;
                AllCams = GameObject.FindObjectsOfType<Camera>().ToList();
                foreach (var cam in AllCams)
                {
                    if (cam != null) 
                    {
                        cam.gameObject.SetActive(false);
                    }
                }
                CaptureCam.gameObject.SetActive(true);
            }
        }
        EditorGUILayout.Space();
    }

    private void OnGUITakeButton()
    {
        if (_buttonStyle == null)
        {
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = Color.green },
                active = { textColor = Color.red }
            };
        }

        GUI.enabled = !_hasErrors && !_isMakingScreenshotsNow;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("TakeScreenShot", _buttonStyle, GUILayout.Width(200f)))
        {
            TakeScreenshots();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    [MenuItem("Tools/Screen Taker/Take Screenshots &#s")]
    private static void TakeScreenshotOnHotkey()
    {
        TakeScreenshots();
    }

    private static void TakeScreenshots()
    {
        _isMakingScreenshotsNow = true;
        CaptureCamera(CaptureCam);
        _isMakingScreenshotsNow = false;
    }

    /// <summary>
    /// 对相机截图
    /// </summary>
    /// <param name="camera">Camera.要被截屏的相机</param>
    /// <param name="rect">Rect.截屏的区域</param>
    /// <returns>The screenshot2.</returns>
    static Texture2D CaptureCamera(Camera camera)
    {
        ScreenshotConfig screenshotConfig = new ScreenshotConfig("snapShot",720,1280,ScreenshotConfig.Format.PNG);
        Rect rect = new Rect(0, 0, (int)screenshotConfig.Width, (int)screenshotConfig.Height);
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);//创建一个RenderTexture对象
        camera.targetTexture = rt;//临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
        //ps: camera2.targetTexture = rt;
        //ps: camera2.Render();
        //ps: -------------------------------------------------------------------

        RenderTexture.active = rt;//激活这个rt, 并从中中读取像素。
        Texture2D screenShot = new Texture2D((int)screenshotConfig.Width, (int)screenshotConfig.Height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);//注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();

        //重置相关参数，以使用camera继续在屏幕上显示
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;
        RenderTexture.active = null; //JC: added to avoid errors
        //GameObject.Destroy(rt);

        SaveTextureAsFile(screenShot, Application.dataPath + "/screenShot", "_", DateTime.Now.ToString("yyyyMMddfff"), screenshotConfig);
        //byte[] bytes = screenShot.EncodeToPNG();//最后将这些纹理数据，成一个png图片文件
        //string filename = Application.dataPath + "/Screenshot.png";
        //System.IO.File.WriteAllBytes(filename, bytes);
        //Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        return screenShot;
    }

    public static void SaveTextureAsFile(Texture2D texture, string folder, string prefix, string suffix, ScreenshotConfig screenshotConfig)
    {
        byte[] bytes;
        string extension;

        switch (screenshotConfig.Type)
        {
            case ScreenshotConfig.Format.PNG:
                bytes = texture.EncodeToPNG();
                extension = ".png";
                break;
            case ScreenshotConfig.Format.JPG:
                bytes = texture.EncodeToJPG();
                extension = ".jpg";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var fileName = prefix + screenshotConfig.Name + "." + screenshotConfig.Width + "x" + screenshotConfig.Height + suffix;
        var imageFilePath = folder + "/" + MakeValidFileName(fileName + extension);

        // ReSharper disable once PossibleNullReferenceException
        (new FileInfo(imageFilePath)).Directory.Create();
        File.WriteAllBytes(imageFilePath, bytes);

        Debug.Log("Image saved to: " + imageFilePath);
    }

    private static string MakeValidFileName(string name)
    {
        var invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
    }
}

[Serializable]
public class ScreenshotConfig
{
    public string Name;
    public int Width;
    public int Height;
    public Format Type;

    public ScreenshotConfig() { }

    public ScreenshotConfig(string name, int width, int height, Format type)
    {
        Name = name;
        Width = width;
        Height = height;
        Type = type;
    }

    //---------------------------------------------------------------------
    // Nested
    //---------------------------------------------------------------------

    public enum Format
    {
        PNG,
        JPG
    }
}
