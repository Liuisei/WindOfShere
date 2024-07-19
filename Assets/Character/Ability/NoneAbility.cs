using System;

[Serializable]
public class NoneAbility : IAbility
{
    public string Name => "None";

    public void Ability() { }
}
