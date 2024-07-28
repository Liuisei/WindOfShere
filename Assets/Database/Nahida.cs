using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nahida", menuName = "Nahida")]
public class Nahida : ScriptableObject
{
    [SerializeField] CharacterDataBase[] _characterDataBase;

    public CharacterDataBase[] CharacterDataBase => _characterDataBase;
}