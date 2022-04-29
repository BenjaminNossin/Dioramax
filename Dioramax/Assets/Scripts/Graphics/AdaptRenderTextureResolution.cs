using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptRenderTextureResolution : MonoBehaviour
{
    [SerializeField] RenderTexture rt;
    [SerializeField] GameObject quad;
    [SerializeField] Camera cam2;


    void Start()
    {
        //rt.width = Screen.width;
        // rt.height = Screen.height;

           //en supposant qu'on a un quad fullscreen qui a une scale de 1,1,1 qui fait la hauteur de notre champ de vision,
           //�a va l'�largir pour avoir le bon aspect ratio
          // quad.transform.localScale = (new Vector3((float)Screen.width / Screen.height, 1, 1));

           //ici on lib�re, resize, et recr�e/renvoie au gpu notre renderTexture avec la bonne taille
           rt.Release();
           rt.width = Screen.width;
           rt.height = Screen.height;
           rt.Create();

           //ici on enl�ve et remet la renderTexture de la cam�ra2 pour qu'elle ajuste son champ de vision
           cam2.targetTexture = null;
           cam2.targetTexture = rt;
        /*

        rt.Release();
        rt.width = Screen.width;
        rt.height = Screen.height;
        rt.Create();*/
    }

    //void ChangeRTresolution()
    //{
    //    rt.Release();
    //    rt.width = Screen.width;
    //    rt.height = Screen.height;
    //    rt.Create();
    //}
}
