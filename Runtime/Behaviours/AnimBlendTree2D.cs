using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimBlendTree2D : AnimPlayableBehaviour
    {
        private int m_clipCount;
        private AnimBlendClip2DInfo m_blendClip2DInfo;
        private AnimBlendClip2DInfo.BlendClip2D[] m_clips;
        private AnimationMixerPlayable m_mixer;
        private string m_key;

        public override float GetEnterTime()
        {
            return m_blendClip2DInfo.EnterTime;
        }

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            m_mixer = AnimationMixerPlayable.Create(m_graph);
            playable.AddInput(m_mixer, 0, 1f);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            float total = 0;
            for (int i = 0; i < m_clipCount; i++)
            {
                m_clips[i].CalculateDistance(m_context.GetVector2(m_key));
                total += m_clips[i].Distance;
            }

            for (int i = 0; i < m_clipCount; i++)
            {
                m_mixer.SetInputWeight(i, m_clips[i].Distance / total);
            }
        }

        protected override void DoBehaviourStop(Playable playable)
        {
            base.DoBehaviourStop(playable);
            playable.Pause();
            m_mixer.SetTime(0f);
            for (int i = 0; i < m_clipCount; i++)
            {
                m_mixer.GetInput(i).SetTime(0f);
            }
        }


        public override int AddInput(Playable playable)
        {
            base.AddInput(playable);
            m_mixer.AddInput(playable, 0);
            return m_mixer.GetInputCount() - 1;
        }

        public static AnimBlendTree2D Create(AnimContext context, AnimBlendClip2DInfo info)
        {
            Playable playable = ScriptPlayable<AnimBlendTree2D>.Create(context.Graph);
            var behaviour = ((ScriptPlayable<AnimBlendTree2D>)playable).GetBehaviour();
            behaviour.m_blendClip2DInfo = info;
            behaviour.m_clips = info.Clips;
            behaviour.m_clipCount = info.Clips.Length;
            behaviour.m_context = context;
            behaviour.name = info.Name;
            behaviour.m_key = info.VariableKey;
            for (int i = 0; i < behaviour.m_clipCount; i++)
            {
                behaviour.AddInput(AnimationClipPlayable.Create(context.Graph, info.Clips[i].Clip));
            }

            return behaviour;
        }

    }
}