using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimBlendTree1D : AnimPlayableBehaviour
    {
        private int m_clipCount;
        private AnimBlendClip1DInfo.BlendClip1D[] m_clips;
        private AnimationMixerPlayable m_mixer;
        private string m_variableKey;
        private float m_currentValue;
        private AnimBlendClip1DInfo m_blendClip1DInfo;

        public override float GetEnterTime()
        {
            return m_blendClip1DInfo.EnterTime;
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
    
            // 获取最新的变量值
            m_currentValue = m_context.GetFloat(m_variableKey);
    
            // === 新增边界检查 ===
            // 当值小于最小阈值时，只激活第一个片段
            if (m_currentValue <= m_clips[0].Threshold)
            {
                for (int i = 0; i < m_clipCount; i++)
                {
                    m_mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
                }
                return; // 直接返回，避免后续计算
            }
    
            // 当值大于最大阈值时，只激活最后一个片段
            if (m_currentValue >= m_clips[m_clipCount - 1].Threshold)
            {
                for (int i = 0; i < m_clipCount; i++)
                {
                    m_mixer.SetInputWeight(i, i == m_clipCount - 1 ? 1f : 0f);
                }
                return; // 直接返回，避免后续计算
            }
            // === 边界检查结束 ===
    
            // 找到当前值所在的区间（仅在中间值时执行）
            int lowerIndex = 0;
            int upperIndex = m_clipCount - 1;
    
            for (int i = 0; i < m_clipCount - 1; i++)
            {
                if (m_currentValue >= m_clips[i].Threshold && 
                    m_currentValue <= m_clips[i + 1].Threshold)
                {
                    lowerIndex = i;
                    upperIndex = i + 1;
                    break;
                }
            }
    
            // 计算混合权重
            float range = m_clips[upperIndex].Threshold - m_clips[lowerIndex].Threshold;
            float t = range > 0.001f ? 
                (m_currentValue - m_clips[lowerIndex].Threshold) / range : 
                0f;
    
            // 设置权重（确保只有两个片段有权重）
            for (int i = 0; i < m_clipCount; i++)
            {
                float weight = 0f;
        
                if (i == lowerIndex) weight = 1f - t;
                else if (i == upperIndex) weight = t;
        
                m_mixer.SetInputWeight(i, weight);
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
            return m_mixer.AddInput(playable, 0);
        }

        public static AnimBlendTree1D Create(
            AnimContext context, 
            AnimBlendClip1DInfo info)
        {
            Playable playable = ScriptPlayable<AnimBlendTree1D>.Create(context.Graph);
            var behaviour = ((ScriptPlayable<AnimBlendTree1D>)playable).GetBehaviour();
        
            behaviour.m_blendClip1DInfo = info;
            behaviour.m_clips = info.Clips;
            behaviour.m_clipCount = info.Clips.Length;
            behaviour.m_context = context;
            behaviour.name = info.Name;
            behaviour.m_variableKey = info.VariableKey;
            Array.Sort(behaviour.m_clips, (a, b) => a.Threshold.CompareTo(b.Threshold));
        
            for (int i = 0; i < behaviour.m_clipCount; i++)
            {
                behaviour.AddInput(AnimationClipPlayable.Create(
                    context.Graph, 
                    behaviour.m_clips[i].Clip
                ));
            }
        
            return behaviour;
        }
    

    }
}