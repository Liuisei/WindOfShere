using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMove : MonoBehaviour
{
    public void MoveScene(string sceneName)
    {
        Debug.Log(sceneName);
        GameSceneManager.Instance.SceneMove(sceneName);
    }
}
