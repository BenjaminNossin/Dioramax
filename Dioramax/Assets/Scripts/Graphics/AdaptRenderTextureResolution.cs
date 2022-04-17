using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptRenderTextureResolution : MonoBehaviour
{
    [SerializeField] RenderTexture rt;

    void Start()
    {
        rt.width = Screen.width;
        rt.height = Screen.height;
    }

    //void ChangeRTresolution()
    //{
    //    rt.Release();
    //    rt.width = Screen.width;
    //    rt.height = Screen.height;
    //    rt.Create();
    //}
}
