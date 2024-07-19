using System;
using UnityEngine;

/// <summary>SerializeReferenceの項目を表示してくれるEditor拡張用クラス</summary>
[AttributeUsage(AttributeTargets.Field)]
public class SubclassSelectorAttribute : PropertyAttribute
{
    private readonly bool m_includeMono;

    public SubclassSelectorAttribute(bool includeMono = false)
    {
        m_includeMono = includeMono;
    }

    public bool IsIncludeMono()
    {
        return m_includeMono;
    }
}