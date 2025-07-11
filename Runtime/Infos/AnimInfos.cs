using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace QDuck.Animation
{
    [Serializable]
    public class AnimInfo : IAnimInfo
    {
        public string Name;

        public virtual AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            Debug.LogError("CreateBehaviour not implemented for " + GetType().Name);
            return null;
        }
    }

    [Serializable]
    public class AnimUnitInfo : AnimInfo
    {
        public float EnterTime;
        public AnimationClip Clip;
        public bool Loop;
        public float Speed=1f;
        public float Length
        {
            get
            {
                return Clip != null ? Clip.length/ Speed : 0f;
            }
        }
        public override AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            AnimUnit unit = AnimUnit.Create(context, this);
            return unit;
        }
    }
    
    
    [Serializable]
    public class AnimEmptyInfo : AnimInfo
    {
        public override AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            AnimEmpty unit = AnimEmpty.Create(context,this);
            return unit;
        }
    }

    [Serializable]
    public class AnimBlendClip2DInfo : AnimInfo
    {
        [System.Serializable]
        public struct BlendClip2D
        {
            public AnimationClip Clip;
            public Vector2 Pos;
            public float Distance { get; private set; }

            public void CalculateDistance(Vector2 pointer)
            {
                Distance = 1 / (Mathf.Abs(pointer.x - Pos.x) + Mathf.Abs(pointer.y - Pos.y) + 1e-5f);
            }
        }

        public float EnterTime;
        public string VariableKey;
        public BlendClip2D[] Clips;

        public override AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            AnimBlendTree2D blendTree = AnimBlendTree2D.Create(context, this);
            return blendTree;
        }
    }

    [Serializable]
    public class AnimationClipInfo
    {
        public float EnterTime;
        public AnimationClip Clip;
        public float Length => Clip != null ? Clip.length : 0f;
    }

    [Serializable]
    public class AnimRandomInfo : AnimInfo
    {
        public AnimationClipInfo[] Clips;

        public override AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            AnimRandomSelector random = AnimRandomSelector.Create(context, this);
            return random;
        }
    }

    [Serializable]
    public class AnimQueueInfo : AnimInfo
    {
        public AnimationClipInfo[] Clips;

        public override AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            AnimQueueSelector queue = AnimQueueSelector.Create(context, this);
            return queue;
        }
    }

    [Serializable]
    public class AnimBlendClip1DInfo : AnimInfo
    {
        [Serializable]
        public struct BlendClip1D
        {
            public AnimationClip Clip;
            public float Threshold;
        }

        public float EnterTime;
        public string VariableKey;
        public BlendClip1D[] Clips;

        public override AnimPlayableBehaviour CreateBehaviour(AnimContext context)
        {
            return AnimBlendTree1D.Create(context, this);
        }
    }
}


