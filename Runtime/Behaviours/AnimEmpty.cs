using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
namespace QDuck.Animation
{
    public class AnimEmpty: AnimPlayableBehaviour
    {
        public static AnimEmpty Create(AnimContext context,AnimEmptyInfo info)
        {
            Playable playable = ScriptPlayable<AnimEmpty>.Create(context.Graph);
            var behaviour = ((ScriptPlayable<AnimEmpty>)playable).GetBehaviour();
            behaviour.m_context = context;
            behaviour.name = info.Name;
            return behaviour;
        }
    }
}