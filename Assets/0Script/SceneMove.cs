using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    public void MoveScene()
    {
        GameSceneManager.Instance.SceneMove(_sceneName);
    }
}
