using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMove : MonoBehaviour
{
    public void MoveScene(string sceneName)
    {
        GameSceneManager.Instance.SceneMove(sceneName);
    }
}