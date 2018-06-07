using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VARP.DebugDraw;


public class DebugDrawTestSceneCamera : MonoBehaviour {


    private void Awake ( )
    {
        DebugDraw.Init ( );
    }

    /**
     * To render objects in the GameView */
    IEnumerator OnPostRender ( )
    {
            yield return new WaitForEndOfFrame ( );
            DebugDraw.Render (  );
    }

    void OnRenderObject()
    {
        DebugDraw.Render ( );
    }

    private void OnApplicationQuit ( )
    {
        DebugDraw.DeInit ( );
    }
}
