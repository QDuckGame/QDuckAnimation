using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace QDuck.Animation
{
    [CustomEditor(typeof(AnimProSet))]
    public class AnimProSetEditor : UnityEditor.Editor
    {
        private SerializedProperty layerInfos;
        private int selectedTab = 0;
        private bool[] layerFoldouts;

        private void OnEnable()
        {
            layerInfos = serializedObject.FindProperty("LayerInfos");
            InitializeFoldouts();
        }

        private void InitializeFoldouts()
        {
            layerFoldouts = new bool[layerInfos.arraySize];
            for (int i = 0; i < layerFoldouts.Length; i++)
            {
                layerFoldouts[i] = true; // 默认展开所有层
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 绘制层选项卡和删除按钮
            DrawTabsWithDeleteButtons();



            // 绘制当前选中的层
            if (layerInfos.arraySize > 0)
            {
                DrawLayer(layerInfos.GetArrayElementAtIndex(selectedTab), selectedTab);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTabsWithDeleteButtons()
        {
            if (layerInfos.arraySize > 0)
            {
                EditorGUILayout.BeginHorizontal();
                
                // 绘制选项卡
                string[] tabNames = new string[layerInfos.arraySize];
                for (int i = 0; i < layerInfos.arraySize; i++)
                {
                    SerializedProperty layerProp = layerInfos.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = layerProp.FindPropertyRelative("Name");
                    tabNames[i] = string.IsNullOrEmpty(nameProp.stringValue) ? $"Layer {i}" : nameProp.stringValue;
                }
                selectedTab = GUILayout.Toolbar(selectedTab, tabNames, GUILayout.ExpandWidth(true));
                
                // 删除当前层按钮
                if (GUILayout.Button("DelLayer", GUILayout.Width(65), GUILayout.Height(20)))
                {
                    DeleteCurrentLayer();
                }
                
                // 添加层按钮
                if (GUILayout.Button("AddLayer", GUILayout.Width(65), GUILayout.Height(20)))
                {
                    AddNewLayer();
                  //  return;
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddNewLayer()
        {
            int newIndex = layerInfos.arraySize;
            layerInfos.arraySize++;
            
            // 初始化新层
            SerializedProperty newLayer = layerInfos.GetArrayElementAtIndex(newIndex);
            newLayer.FindPropertyRelative("Name").stringValue = $"New Layer {newIndex}";
            newLayer.FindPropertyRelative("Weight").floatValue = 1.0f;
            
            // 初始化动画列表
            SerializedProperty animationsProp = newLayer.FindPropertyRelative("Animations");
            animationsProp.arraySize = 0;
            
            // 更新折叠状态数组
            Array.Resize(ref layerFoldouts, layerInfos.arraySize);
            layerFoldouts[newIndex] = true;
            
            // 选中新添加的层
            selectedTab = newIndex;
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DeleteCurrentLayer()
        {
            if (layerInfos.arraySize == 0) return;
            
            int deleteIndex = selectedTab;
            layerInfos.DeleteArrayElementAtIndex(deleteIndex);
            
            // 调整选中索引
            if (layerInfos.arraySize > 0)
            {
                selectedTab = Mathf.Clamp(selectedTab, 0, layerInfos.arraySize - 1);
            }
            else
            {
                selectedTab = 0;
            }
            
            // 更新折叠状态数组
            InitializeFoldouts();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLayer(SerializedProperty layerProp, int index)
        {
            // 绘制层折叠区域
            EditorGUILayout.Space();
            layerFoldouts[index] = EditorGUILayout.BeginFoldoutHeaderGroup(layerFoldouts[index], "Layer Settings");
            if (!layerFoldouts[index])
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            
            EditorGUI.indentLevel++;
            
            // 绘制基础属性
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("Name"));
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("Mask"));
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("IsAdditive"));
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("Weight"));
            
            // 绘制Animations列表
            SerializedProperty animationsProp = layerProp.FindPropertyRelative("Animations");
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
                if (GUILayout.Button("Remove"))
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
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
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