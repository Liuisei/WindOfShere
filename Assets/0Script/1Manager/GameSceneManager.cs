using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    //MasterSceneで管理されているこのSceneManagerを使いゲームシーンを遷移する。
    public static GameSceneManager Instance;
    public string OpeningSceneName = "null";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.sceneCount == 1)
        {
            OpeningSceneName = "1Scenes/Title";
            SceneManager.LoadScene(OpeningSceneName, LoadSceneMode.Additive);
        }
        else
        {
            OpeningSceneName = SceneManager.GetSceneAt(1).name;
        }
    }

    public void SceneMove(string loadSceneName)
    {
        SceneManager.UnloadSceneAsync(OpeningSceneName);
        SceneManager.LoadScene(loadSceneName, LoadSceneMode.Additive);
        OpeningSceneName = loadSceneName;
    }
}