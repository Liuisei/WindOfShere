using System;
using UnityEngine;

/// <summary>分身スキル</summary>
[Serializable]
public class CloneAbility : IAbility
{
    public string Name => "Clone";

    public void Ability()
    {
        Debug.Log("分身を発動した");
    }
}