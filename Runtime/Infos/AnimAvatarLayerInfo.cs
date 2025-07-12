using System;
using System.Collections.Generic;
using UnityEngine;

namespace QDuck.Animation
{
    [Serializable]
    public class AnimAvatarLayerInfo:AnimLayerInfo
    {
        public AvatarMask Mask;
        public bool IsAdditive;
        public float Weight = 1.0f; // 添加默认值
    }
    
    [Serializable]
    public class AnimLayerInfo :IAnimInfo
    {
        public string Name;
        [SerializeReference] 
        public List<AnimInfo> Animations = new List<AnimInfo>(); // 初始化列表

        public AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            AnimLayer layer = AnimLayer.Create(context);
            foreach (var animation in Animations)
            {
                layer.AddInput(context, animation);
            }
            return layer;
        }
    }
}