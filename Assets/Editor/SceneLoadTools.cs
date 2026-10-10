
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;

public class SceneLoadTools : MonoBehaviour
{
    // Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
    [MenuItem("Scenes/LoadingScene")]
    static void OpenLoadingScene()
    {
        string scenePath = "Assets/Scenes/LoadingScene.unity";
       //Open the Scene in the Editor (do not enter Play Mode)
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/HomeScene")]
    static void OpenMenuScene()
    {
        string scenePath = "Assets/Scenes/HomeScene.unity";
        //Open the Scene in the Editor (do not enter Play Mode)
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
    }

    [MenuItem("Scenes/PlayScene")]
    static void OpenPlayScene()
    {
        string scenePath = "Assets/Scenes/PlayScene.unity";
        //Open the Scene in the Editor (do not enter Play Mode)
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
    }
}
