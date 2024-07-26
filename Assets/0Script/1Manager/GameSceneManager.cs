using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    //MasterSceneで管理されているこのSceneManagerを使いゲームシーンを遷移する。
    public static GameSceneManager Instance;
    private string _openingSceneName;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _openingSceneName = "1Scenes/Title";
        SceneManager.LoadScene(_openingSceneName,LoadSceneMode.Additive);
        Debug.Log("Start");
    }

    public void SceneMove(string loadSceneName)
    {
        SceneManager.UnloadSceneAsync(_openingSceneName);
        SceneManager.LoadScene(loadSceneName,LoadSceneMode.Additive);
        _openingSceneName = loadSceneName;
    }
}
