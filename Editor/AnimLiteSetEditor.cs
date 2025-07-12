using UnityEditor;
using UnityEngine;
using System;

namespace QDuck.Animation
{
    [CustomEditor(typeof(AnimLiteSet))]
    public class AnimLiteSetEditor : UnityEditor.Editor
    {
        private SerializedProperty layerInfoProp;
        private bool layerFoldout = true;

        private void OnEnable()
        {
            layerInfoProp = serializedObject.FindProperty("LayerInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            layerFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(layerFoldout, "Layer Settings");
            if (layerFoldout)
            {
                EditorGUI.indentLevel++;
                
                // 检查 LayerInfo 是否已初始化
                if (layerInfoProp.managedReferenceValue == null)
                {
                    if (GUILayout.Button("Initialize Layer"))
                    {
                        InitializeLayer();
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
                else
                {
                    DrawLayer();
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeLayer()
        {
            // 创建一个新的 AnimLayerInfo 实例并分配
            layerInfoProp.managedReferenceValue = new AnimLayerInfo();
            
            // 设置默认名称
            serializedObject.ApplyModifiedProperties();
            var layerInfo = layerInfoProp.managedReferenceValue as AnimLayerInfo;
            if (layerInfo != null)
            {
                layerInfo.Name = "Main Layer";
            }
        }

        private void DrawLayer()
        {
            // 绘制基础属性
            EditorGUILayout.PropertyField(layerInfoProp.FindPropertyRelative("Name"));
            
            // 获取 Animations 列表属性
            SerializedProperty animationsProp = layerInfoProp.FindPropertyRelative("Animations");
            EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
            
            if (animationsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No animations added. Click 'Add Animation' to create one.", MessageType.Info);
            }
            
            for (int i = 0; i < animationsProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginHorizontal();
                
                SerializedProperty elementProp = animationsProp.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = elementProp.FindPropertyRelative("Name");
                
                // 显示名称和类型
                Type elementType = GetManagedReferenceType(elementProp);
                string typeName = elementType != null ? elementType.Name : "Unknown";
                EditorGUILayout.LabelField($"{nameProp.stringValue} ({typeName})", EditorStyles.boldLabel);
                
                // 删除按钮
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    animationsProp.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
                EditorGUILayout.EndHorizontal();
                
                // 绘制具体属性
                if (elementType != null)
                {
                    EditorGUILayout.PropertyField(elementProp, true);
                }
                else
                {
                    EditorGUILayout.HelpBox("Unknown animation type. Please re-add this animation.", MessageType.Warning);
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            // 添加动画按钮
            if (GUILayout.Button("Add Animation", GUILayout.Height(25)))
            {
                ShowAddAnimationMenu(animationsProp);
            }
        }

        private void ShowAddAnimationMenu(SerializedProperty listProp)
        {
            GenericMenu menu = new GenericMenu();
            
            // 添加所有AnimInfo的子类
            menu.AddItem(new GUIContent("AnimUnitInfo (Single Clip)"), false, () => AddAnimation(listProp, typeof(AnimUnitInfo)));
            menu.AddItem(new GUIContent("AnimEmptyInfo (Empty)"), false, () => AddAnimation(listProp, typeof(AnimEmptyInfo)));
            menu.AddItem(new GUIContent("AnimBlendClip2DInfo (2D Blend Tree)"), false, () => AddAnimation(listProp, typeof(AnimBlendClip2DInfo)));
            menu.AddItem(new GUIContent("AnimRandomInfo (Random Selector)"), false, () => AddAnimation(listProp, typeof(AnimRandomInfo)));
            menu.AddItem(new GUIContent("AnimQueueInfo (Sequence)"), false, () => AddAnimation(listProp, typeof(AnimQueueInfo)));
            menu.AddItem(new GUIContent("AnimBlendClip1DInfo (1D Blend Tree)"), false, () => AddAnimation(listProp, typeof(AnimBlendClip1DInfo)));
            
            menu.ShowAsContext();
        }

        private void AddAnimation(SerializedProperty listProp, Type type)
        {
            int index = listProp.arraySize;
            listProp.arraySize++;
            SerializedProperty element = listProp.GetArrayElementAtIndex(index);
            element.managedReferenceValue = Activator.CreateInstance(type);
            
            // 设置默认名称
            var nameProp = element.FindPropertyRelative("Name");
            if (nameProp != null)
            {
                nameProp.stringValue = $"{type.Name.Replace("Info", "")} {index}";
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private Type GetManagedReferenceType(SerializedProperty property)
        {
            string typeName = property.managedReferenceFullTypename;
            if (string.IsNullOrEmpty(typeName)) return null;
            
            string[] split = typeName.Split(' ');
            if (split.Length < 2) return null;
            
            string assemblyName = split[0];
            string className = split[1];
            
            return Type.GetType($"{className}, {assemblyName}");
        }
    }
}