using System;
using System.Collections.Generic;
using UnityEngine;

namespace QDuck.Animation
{
    [Serializable]
    public class AnimAvatarLayerInfo : AnimLayerInfo
    {
        public AvatarMask Mask;
        public bool IsAdditive;
        public float Weight;
    }

    [Serializable]
    public class AnimLayerInfo : IAnimInfo
    {
        public string Name;
        [SerializeReference] public List<AnimInfo> Animations;

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