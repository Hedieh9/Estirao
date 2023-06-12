using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width -= 10;
        EditorGUI.DrawRect(r, color);
    }

    /// <summary>
    /// Returns target Object of Serialized Property
    /// </summary>
    /// <param name="serializedProperty"></param>
    /// <returns></returns>
    public static UnityEngine.Object GetObject(this SerializedProperty serializedProperty)
    {
        return GetTargetObjectOfProperty(serializedProperty) as UnityEngine.Object;
    }

    #region GetObjectOfSerializedPropertyLogic

    /// <summary>
    ///     Gets the object the property represents.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        string path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        string[] elements = path.Split('.');
        foreach (string element in elements)
            if (element.Contains("["))
            {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                    .Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }

        return obj;
    }

    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        Type type = source.GetType();

        while (type != null)
        {
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            PropertyInfo p = type.GetProperty(name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }

        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
        if (enumerable == null) return null;
        IEnumerator enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= index; i++)
            if (!enm.MoveNext())
                return null;
        return enm.Current;
    }

    #endregion
}
