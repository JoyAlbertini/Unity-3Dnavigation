using UnityEngine;
using System;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace Octree.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string ButtonText { get; }
        public string MethodName { get; }

        public ButtonAttribute(string buttonText, string methodName = null)
        {
            ButtonText = buttonText;
            MethodName = methodName;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer
    {
        private MethodInfo cachedMethodInfo;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)  {
            return 27.0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            ButtonAttribute buttonAttribute = attribute as ButtonAttribute;

            if (property.propertyType == SerializedPropertyType.Boolean)
            {
                DrawButton(position, property, buttonAttribute);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Button with a boolean property.");
            }
        }

        private void DrawButton(Rect position, SerializedProperty property, ButtonAttribute buttonAttribute)
        {
            float originalWidth = position.width;
            position.height = 25.0f;
            position.width = 250.0f;
            position.x = (originalWidth - 250.0f) / 2;

            if (GUI.Button(position, buttonAttribute.ButtonText))
            {
                InvokeButtonMethod(property, buttonAttribute);
            }

            if (GUI.changed)
            {
                MarkObjectAsDirty(property.serializedObject.targetObject);
            }
        }

        
        private void InvokeButtonMethod(SerializedProperty property, ButtonAttribute buttonAttribute)
        {
            if (!string.IsNullOrEmpty(buttonAttribute.MethodName))
            {
                if (cachedMethodInfo == null)
                {
                    cachedMethodInfo = property.serializedObject.targetObject.GetType()
                        .GetMethod(buttonAttribute.MethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                }

                if (cachedMethodInfo != null)
                {
                    cachedMethodInfo.Invoke(property.serializedObject.targetObject, null);
                }
                else
                {
                    Debug.LogError($"Method '{buttonAttribute.MethodName}' not found on {property.serializedObject.targetObject.GetType()}.");
                }
            }
        }


        private static void MarkObjectAsDirty(Object targetObject)
        {
            if (Application.isPlaying || targetObject == null)
            {
                return;
            }

            EditorUtility.SetDirty(targetObject);
        }
    }
#endif
}
