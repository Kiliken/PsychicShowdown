using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class SceneViewFollower : MonoBehaviour
{
    void Update()
    {
        
        
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView.camera != null)
            {
                transform.position = sceneView.camera.transform.position;
                transform.rotation = sceneView.camera.transform.rotation;
            }
        
    }
}
