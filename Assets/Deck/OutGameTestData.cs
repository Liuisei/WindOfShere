using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "OutGameTestData")]
public class OutGameTestData : ScriptableObject
{
    [SerializeField] CharacterDataBase[] _characterDataBase;
    public CharacterDataBase[] CharacterDataBase => _characterDataBase;
}
