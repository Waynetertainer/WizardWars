using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorAssetSettings : MonoBehaviour
{
    public eVisibility eVisibility = eVisibility.Opaque;
    public eBlockType eBlockType = eBlockType.Blocked;

    public void Awake()
    {
        ////TODO Re-enable when levels generate navigation themselves
        //GetComponent<BoxCollider>().enabled = false;
    }
}
