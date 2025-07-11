using UnityEngine;
using System;

namespace QDuck.Animation
{
    [Serializable]
    public class AnimProConfigWrapper
    {
        public AnimConfigType configType = AnimConfigType.ScriptableObject;

        [SerializeField] private AnimProSet _scriptableConfig;

        [SerializeField] private AnimAvatarLayerInfo[] _inlineLayers;

        public AnimAvatarLayerInfo[] GetLayers()
        {
            switch (configType)
            {
                case AnimConfigType.ScriptableObject:
                    return _scriptableConfig?.LayerInfos;

                case AnimConfigType.Inline:
                    return _inlineLayers;

                default:
                    Debug.LogError("Unsupported config type");
                    return null;
            }
        }
    }
}