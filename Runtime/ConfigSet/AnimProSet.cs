using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace QDuck.Animation
{
    [CreateAssetMenu(menuName = "Animation/AnimProSet")]
    [Serializable]
    public class AnimProSet : ScriptableObject
    {
        public AnimAvatarLayerInfo[] LayerInfos;
        
        
    }
}