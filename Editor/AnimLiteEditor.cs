#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace QDuck.Animation
{
    [CustomEditor(typeof(AnimLite))]
    public class AnimLiteEditor : Editor
    {
        private SerializedProperty _configWrapperProp;
        private SerializedProperty _configTypeProp;
        private SerializedProperty _scriptableConfigProp;
        private SerializedProperty _inlineConfigProp;

        // 添加基类属性的引用
        private SerializedProperty _isImmediatePlayProp;
        private SerializedProperty _defaultAnimationNameProp;

        private void OnEnable()
        {
            _configWrapperProp = serializedObject.FindProperty("_configWrapper");
            _configTypeProp = _configWrapperProp.FindPropertyRelative("configType");
            _scriptableConfigProp = _configWrapperProp.FindPropertyRelative("_scriptableConfig");
            _inlineConfigProp = _configWrapperProp.FindPropertyRelative("_inlineConfig");

            // 获取基类属性
            _isImmediatePlayProp = serializedObject.FindProperty("_isImmediatePlay");
            _defaultAnimationNameProp = serializedObject.FindProperty("_defaultAnimationName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 绘制配置类型选择
            EditorGUILayout.PropertyField(_configTypeProp);

            // 根据选择类型显示相应配置
            var configType = (AnimConfigType)_configTypeProp.enumValueIndex;

            switch (configType)
            {
                case AnimConfigType.ScriptableObject:
                    EditorGUILayout.PropertyField(_scriptableConfigProp);
                    break;

                case AnimConfigType.Inline:
                    EditorGUILayout.PropertyField(_inlineConfigProp);
                    break;
            }

            // 绘制基类属性（只绘制一次）
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Base Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_isImmediatePlayProp);
            EditorGUILayout.PropertyField(_defaultAnimationNameProp);

            // 绘制其他属性
            DrawRemainingProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRemainingProperties()
        {
            // 跳过已经手动绘制的属性
            string[] skipProperties =
            {
                "_configWrapper",
                "_isImmediatePlay",
                "_defaultAnimationName",
                "m_Script"
            };

            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            bool first = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                // 跳过已经手动绘制的属性
                bool shouldSkip = false;
                foreach (var skip in skipProperties)
                {
                    if (iterator.name == skip)
                    {
                        shouldSkip = true;
                        break;
                    }
                }

                if (shouldSkip) continue;

                // 绘制属性
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }


    }
#endif