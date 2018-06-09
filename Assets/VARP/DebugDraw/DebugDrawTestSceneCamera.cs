using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VARP.DebugDraw;


public class DebugDrawTestSceneCamera : MonoBehaviour {

    public bool renderInEditor;
    public bool renderInPostRender;
    public bool renderInEndOfFrame;

    private void Awake ( )
    {
        DebugDraw.Init ( );
    }

    /**
     * To render objects in the GameView */
    IEnumerator OnPostRender ( )
    {
        if (renderInPostRender)
            DebugDraw.Render ( );
        yield return new WaitForEndOfFrame ( );
        if ( renderInEndOfFrame )
            DebugDraw.Render (  );
    }

    void OnRenderObject()
    {
        if ( renderInEditor && Camera.current != null && Camera.current.name == "SceneCamera")
            DebugDraw.Render ( );
    }

    private void OnApplicationQuit ( )
    {
        DebugDraw.DeInit ( );
    }
}
