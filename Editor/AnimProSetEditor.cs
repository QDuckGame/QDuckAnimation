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
        private Vector2 scrollPosition;
        private bool needsInitialization;

        private void OnEnable()
        {
            layerInfos = serializedObject.FindProperty("LayerInfos");
            
            // 检查是否需要初始化
            needsInitialization = layerInfos.arraySize == 0;
            
            // 如果数组为空，添加默认层
            if (needsInitialization)
            {
                AddDefaultLayer();
            }
            else
            {
                InitializeFoldouts();
            }
        }

        private void AddDefaultLayer()
        {
            // 添加一个新层
            layerInfos.arraySize = 1;
            
            // 获取新添加的元素
            SerializedProperty newLayer = layerInfos.GetArrayElementAtIndex(0);
            
            // 初始化新层
            InitializeLayerProperties(newLayer);
            
            // 更新折叠状态
            layerFoldouts = new bool[] { true };
            
            // 应用修改
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            
            // 标记已初始化
            needsInitialization = false;
        }

        private void InitializeFoldouts()
        {
            layerFoldouts = new bool[layerInfos.arraySize];
            for (int i = 0; i < layerFoldouts.Length; i++)
            {
                layerFoldouts[i] = true;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Animation Profile Set", EditorStyles.largeLabel);
            EditorGUILayout.Space();

            DrawTabsWithDeleteButtons();



            if (layerInfos.arraySize > 0)
            {
                EditorGUILayout.Space(10);
                DrawLayer(layerInfos.GetArrayElementAtIndex(selectedTab), selectedTab);
            }
            else
            {
                EditorGUILayout.HelpBox("No layers added. Click 'Add New Layer' to create one.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTabsWithDeleteButtons()
        {
            if (layerInfos.arraySize > 0)
            {
                EditorGUILayout.BeginHorizontal();
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(30));
                EditorGUILayout.BeginHorizontal();
                
                for (int i = 0; i < layerInfos.arraySize; i++)
                {
                    SerializedProperty layerProp = layerInfos.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = layerProp.FindPropertyRelative("Name");
                    
                    string tabName = string.IsNullOrEmpty(nameProp.stringValue) ? 
                        $"Layer {i}" : nameProp.stringValue;
                    
                    bool isSelected = selectedTab == i;
                    var style = isSelected ? EditorStyles.miniButtonMid : EditorStyles.miniButton;
                    
                    if (GUILayout.Button(tabName, style, GUILayout.Height(25)))
                    {
                        selectedTab = i;
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
                
                if (GUILayout.Button("DelLayer", GUILayout.Width(70), GUILayout.Height(20)))
                {
                    DeleteCurrentLayer();
                }
                if (GUILayout.Button("AddLayer", GUILayout.Width(70), GUILayout.Height(20)))
                {
                    AddNewLayer();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddNewLayer()
        {
            // 获取当前数组大小
            int currentSize = layerInfos.arraySize;
            
            // 增加数组大小
            layerInfos.arraySize = currentSize + 1;
            
            // 获取新添加的元素
            SerializedProperty newLayer = layerInfos.GetArrayElementAtIndex(currentSize);
            
            // 初始化新层
            InitializeLayerProperties(newLayer, currentSize);
            
            // 更新折叠状态数组
            Array.Resize(ref layerFoldouts, layerInfos.arraySize);
            layerFoldouts[currentSize] = true;
            
            // 选中新添加的层
            selectedTab = currentSize;
            
            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeLayerProperties(SerializedProperty layerProp, int index = 0)
        {
            // 手动设置所有属性
            SerializedProperty nameProp = layerProp.FindPropertyRelative("Name");
            nameProp.stringValue = $"Layer {index}";
            
            SerializedProperty weightProp = layerProp.FindPropertyRelative("Weight");
            weightProp.floatValue = 1.0f;
            
            SerializedProperty maskProp = layerProp.FindPropertyRelative("Mask");
            maskProp.objectReferenceValue = null;
            
            SerializedProperty additiveProp = layerProp.FindPropertyRelative("IsAdditive");
            additiveProp.boolValue = false;
            
            // 初始化动画列表
            SerializedProperty animationsProp = layerProp.FindPropertyRelative("Animations");
            animationsProp.arraySize = 0;
        }

        private void DeleteCurrentLayer()
        {
            if (layerInfos.arraySize == 0) return;
            
            int deleteIndex = selectedTab;
            layerInfos.DeleteArrayElementAtIndex(deleteIndex);
            
            if (layerInfos.arraySize > 0)
            {
                selectedTab = Mathf.Clamp(selectedTab, 0, layerInfos.arraySize - 1);
            }
            else
            {
                selectedTab = 0;
            }
            
            InitializeFoldouts();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLayer(SerializedProperty layerProp, int index)
        {
            string layerName = layerProp.FindPropertyRelative("Name").stringValue;
            if (string.IsNullOrEmpty(layerName)) layerName = $"Layer {index}";
            
            layerFoldouts[index] = EditorGUILayout.BeginFoldoutHeaderGroup(layerFoldouts[index], layerName);
            if (!layerFoldouts[index])
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("Mask"));
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("IsAdditive"));
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("Weight"));
            EditorGUILayout.PropertyField(layerProp.FindPropertyRelative("Name"));

            SerializedProperty animationsProp = layerProp.FindPropertyRelative("Animations");
            EditorGUILayout.Space();
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
                
                Type elementType = GetManagedReferenceType(elementProp);
                string typeName = elementType != null ? elementType.Name : "Unknown";
                EditorGUILayout.LabelField($"{nameProp.stringValue} ({typeName})", EditorStyles.boldLabel);
                
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    animationsProp.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
                EditorGUILayout.EndHorizontal();
                
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