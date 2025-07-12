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
        private bool needsInitialization;

        private void OnEnable()
        {
            layerInfoProp = serializedObject.FindProperty("LayerInfo");
            
            // 检查是否需要初始化
            needsInitialization = layerInfoProp.managedReferenceValue == null;
            
            // 如果为空，添加默认层
            if (needsInitialization)
            {
                InitializeLayer();
            }
        }

        private void InitializeLayer()
        {
            // 创建一个新的 AnimLayerInfo 实例并分配
            layerInfoProp.managedReferenceValue = new AnimLayerInfo();
            
            // 设置默认名称
            SerializedProperty nameProp = layerInfoProp.FindPropertyRelative("Name");
            nameProp.stringValue = "Main Layer";
            
            // 初始化动画列表
            SerializedProperty animationsProp = layerInfoProp.FindPropertyRelative("Animations");
            animationsProp.arraySize = 0;
            
            // 应用修改
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            
            // 标记已初始化
            needsInitialization = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            layerFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(layerFoldout, "Layer Settings");
            if (layerFoldout)
            {
                EditorGUI.indentLevel++;
                
                // 绘制基础属性
                EditorGUILayout.PropertyField(layerInfoProp.FindPropertyRelative("Name"));
                
                // 绘制Animations列表
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
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void ShowAddAnimationMenu(SerializedProperty listProp)
        {
            GenericMenu menu = new GenericMenu();
            
            // 添加所有AnimInfo的子类
            menu.AddItem(new GUIContent("Single Clip"), false, () => AddAnimation(listProp, typeof(AnimUnitInfo)));
            menu.AddItem(new GUIContent("Empty"), false, () => AddAnimation(listProp, typeof(AnimEmptyInfo)));
            menu.AddItem(new GUIContent("2D Blend Tree"), false, () => AddAnimation(listProp, typeof(AnimBlendClip2DInfo)));
            menu.AddItem(new GUIContent("Random Selector"), false, () => AddAnimation(listProp, typeof(AnimRandomInfo)));
            menu.AddItem(new GUIContent("Sequence"), false, () => AddAnimation(listProp, typeof(AnimQueueInfo)));
            menu.AddItem(new GUIContent("1D Blend Tree"), false, () => AddAnimation(listProp, typeof(AnimBlendClip1DInfo)));
            
            menu.ShowAsContext();
        }

        private void AddAnimation(SerializedProperty listProp, Type type)
        {
            int index = listProp.arraySize;
            listProp.arraySize = index + 1;
            
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
            
            int splitIndex = typeName.IndexOf(' ');
            if (splitIndex < 0) return null;
            
            string assemblyName = typeName.Substring(0, splitIndex);
            string className = typeName.Substring(splitIndex + 1);
            
            return Type.GetType($"{className}, {assemblyName}");
        }
    }
}