﻿using System.Reflection;
using UnityEditor;

[PropertyDrawer(typeof(HideIfAttribute))]
public class HideIfPropertyDrawer : PropertyDrawer
{
    protected override void DrawPropertyImplementation(SerializedProperty property)
    {
        HideIfAttribute hideIfAttribute = PropertyUtility.GetAttributes<HideIfAttribute>(property)[0];
        UnityEngine.Object target = PropertyUtility.GetTargetObject(property);

        FieldInfo conditionField = target.GetType().GetField(hideIfAttribute.ConditionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (conditionField != null)
        {
            if (!(bool)conditionField.GetValue(target))
            {
                EditorGUILayout.PropertyField(property);
            }

            return;
        }

        MethodInfo conditionMethod = target.GetType().GetMethod(hideIfAttribute.ConditionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (conditionMethod != null &&
            conditionMethod.ReturnType == typeof(bool) &&
            conditionMethod.GetParameters().Length == 0)
        {
            if (!(bool)conditionMethod.Invoke(target, null))
            {
                EditorGUILayout.PropertyField(property);
            }

            return;
        }

        EditorGUILayout.PropertyField(property);
        UnityEngine.Debug.LogWarning(hideIfAttribute.GetType().Name + " needs a valid condition field or method name");
    }
}