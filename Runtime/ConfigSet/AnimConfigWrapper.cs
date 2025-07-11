// AnimConfigWrapper.cs
using UnityEngine;
using System;

namespace QDuck.Animation
{
    [Serializable]
    public class AnimConfigWrapper
    {
        public AnimConfigType configType = AnimConfigType.ScriptableObject;

        [SerializeField] private AnimLiteSet _scriptableConfig;

        [SerializeField] private AnimLayerInfo _inlineConfig;

        public AnimLayerInfo GetConfig()
        {
            switch (configType)
            {
                case AnimConfigType.ScriptableObject:
                    return _scriptableConfig?.LayerInfo;

                case AnimConfigType.Inline:
                    return _inlineConfig;

                default:
                    Debug.LogError("Unsupported config type");
                    return null;
            }
        }
    }
}