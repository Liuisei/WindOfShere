#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//SerializeReferenceを属性を持つフィールドをインスペクターで表示してサブクラスを選択できるようにする
[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    //カスタムGUIを描画するためのメソッド
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            var type = fieldInfo.FieldType;　//Fieldの型情報を取得
            //このFieldがジェネリックリストであるかどうか
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                //パラメータを取得(IAbility)
                type = type.GetGenericArguments()[0];
            }

            //このFieldが抽象クラスまたはInterfaceかを判定
            if (type.IsInterface || type.IsAbstract)
            {
                //このドロワーに関連付けされた属性を取得
                var attribute = (SubclassSelectorAttribute)base.attribute;
                //MonoBehaviorを含むか判定
                var includeMono = attribute.IsIncludeMono();

                //ボタンのラベルを設定
                var buttonLabel = "Select " + type.Name;
                
                if (property.managedReferenceFullTypename != "")
                {
                    buttonLabel = property.managedReferenceValue.GetType().Name;
                }

                //ボタンがクリックされたとき
                if (GUI.Button(position, buttonLabel))
                {
                    var menu = new GenericMenu();　//サブクラスを選択するためのメニューを作成
                    var types = TypeCache.GetTypesDerivedFrom(type);　//指定した型を実装するすべての型を取得
                    foreach (var derivedType in types)
                    {
                        //MonoBehaviourを含むかの判定
                        if (!includeMono && typeof(MonoBehaviour).IsAssignableFrom(derivedType))
                        {
                            continue;
                        }

                        //menuにアイテムを追加
                        menu.AddItem(new GUIContent(derivedType.FullName), false, () =>
                        {
                            property.managedReferenceValue = Activator.CreateInstance(derivedType);
                            property.serializedObject.ApplyModifiedProperties();　//プロパティに設定
                        });
                    }
                    menu.ShowAsContext();　//menuに表示
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
#endif
