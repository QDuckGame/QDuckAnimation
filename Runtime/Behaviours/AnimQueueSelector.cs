using UnityEngine;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimQueueSelector : AnimSelector
    {
        protected override int OnSelect()
        {
            return (currentIndex + 1) % clipCount;
        }

        public static AnimQueueSelector Create(AnimContext context, AnimQueueInfo info)
        {
            AnimationClipInfo[] aniamtionInfos = info.Clips;
            Playable playable = ScriptPlayable<AnimQueueSelector>.Create(context.Graph);
            var behaviour = ((ScriptPlayable<AnimQueueSelector>)playable).GetBehaviour();
            behaviour.m_context = context;
            behaviour.name = info.Name;
            foreach (var aniamtionInfo in aniamtionInfos)
            {
                behaviour.AddInput(aniamtionInfo);
            }

            behaviour.Select();
            return behaviour;
        }
    }
}