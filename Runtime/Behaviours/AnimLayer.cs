using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimLayer : AnimPlayableBehaviour
    {
        public int inputCount { get; private set; }
        public int currentIndex => m_currentIndex;
        public bool isTransition => m_isTransition;

        private AnimationMixerPlayable m_animMixer;
        private int m_currentIndex;
        private int m_targetIndex;
        private bool m_isTransition;
        private List<int> m_declineIndexs;
        private float m_timeToNext;
        private float m_currentSpeed;
        private float m_declineSpeed;
        private float m_declineWeight;
        private List<AnimPlayableBehaviour> m_childBehaviours = new List<AnimPlayableBehaviour>();

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            m_animMixer = AnimationMixerPlayable.Create(playable.GetGraph());
            playable.AddInput(m_animMixer, 0, 1f);
            m_currentIndex = -1;
            m_targetIndex = -1;
            m_animMixer.SetTime(0f);
            m_declineIndexs = new List<int>();
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            Debug.Log("AnimationLayer OnBehaviourPlay");
        }

        public override int AddInput(Playable playable)
        {
            base.AddInput(playable);
            m_animMixer.AddInput(playable, 0);
            inputCount = m_animMixer.GetInputCount();
            return m_animMixer.GetInputCount() - 1;
        }

        public void AddInput(AnimContext context, AnimInfo animaitonInfo)
        {
            AnimPlayableBehaviour behaviour = animaitonInfo.CreateBehaviour(context);
            int index = AddInput(behaviour.Playable);
            m_animMixer.SetInputWeight(index, 0f);
            m_childBehaviours.Add(behaviour);
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            playable.Pause();
            for (int i = 0; i < inputCount; i++)
            {
                m_animMixer.SetInputWeight(i, 0f);
            }
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            if (!m_isTransition || m_targetIndex < 0) return;
            if (m_timeToNext > 0f)
            {
                m_timeToNext -= info.deltaTime;
                m_declineWeight = 0;
                for (int i = 0; i < m_declineIndexs.Count; i++)
                {
                    var w = ModifyWeight(m_declineIndexs[i], -info.deltaTime * m_declineSpeed);
                    if (w <= 0f)
                    {
                        AnimHelper.DoStop(m_animMixer, m_declineIndexs[i]);
                        m_declineIndexs.Remove(m_declineIndexs[i]);
                    }
                    else
                    {
                        m_declineWeight += w;
                    }
                }

                m_declineWeight += ModifyWeight(m_currentIndex, -info.deltaTime * m_currentSpeed);
                SetWeight(m_targetIndex, 1 - m_declineWeight);
                return;
            }

            AnimHelper.DoStop(m_animMixer, m_currentIndex);
            m_currentIndex = m_targetIndex;
            m_targetIndex = -1;
            m_isTransition = false;
        }

        public void StopTarget(string targetName)
        {
            for (int i = 0; i < m_childBehaviours.Count; i++)
            {
                if (m_childBehaviours[i].name == targetName)
                {
                    AnimHelper.DoStop(m_animMixer, i);
                    return;
                }
            }
            
        }

        public void TransitionTo(string targetName)
        {
            for (int i = 0; i < m_childBehaviours.Count; i++)
            {
                if (m_childBehaviours[i].name == targetName)
                {
                    // 确保首次播放正确初始化 ===
                    if (m_currentIndex < 0)
                    {
                        m_currentIndex = i;
                        SetWeight(i, 1f);
                        // 确保动画从起始状态开始
                        ResetAnimationState(i);
                        return;
                    }
                    
                    // 避免重复切换到相同动画
                    if (i == m_currentIndex && !m_isTransition) 
                        return;
                    
                    // 确保目标动画已准备好 ===
                    if (m_targetIndex != i)
                    {
                        ResetAnimationState(i);
                    }
                    
                    TransitionTo(i);
                    return;
                }
            }

            Debug.LogWarning($"AnimationLayer: Target animation '{targetName}' not found.");
        }
        
        private void ResetAnimationState(int index)
        {
            var playable = m_animMixer.GetInput(index);
            playable.SetTime(0);
            playable.Play();
            
            // 调用子行为的重置逻辑
            var behaviour = GetChildBehaviour(index);
            behaviour?.ResetToStart();
        }

        public void TransitionTo(int target)
        {

            if (m_currentIndex < 0)
            {
                m_animMixer.SetInputWeight(target, 1f);
                m_currentIndex = target;
                m_animMixer.GetInput(target).Play();
                return;
            }

            if (m_isTransition && m_targetIndex >= 0)
            {
                if (target == m_targetIndex) return;
                if (m_currentIndex == target)
                {
                    m_currentIndex = m_targetIndex;
                }
                else if (GetWeight(m_currentIndex) > GetWeight(m_targetIndex))
                {
                    m_declineIndexs.Add(m_targetIndex);
                }
                else
                {
                    m_declineIndexs.Add(m_currentIndex);
                    m_currentIndex = m_targetIndex;
                }
            }
            else
            {
                if (target == m_currentIndex) return;
            }

            m_targetIndex = target;
            m_declineIndexs.Remove(m_targetIndex);
            m_animMixer.GetInput(m_targetIndex).Play();
            m_timeToNext = GetTargetEnter(m_targetIndex) * (1 - GetWeight(m_targetIndex));
            m_currentSpeed = GetWeight(m_currentIndex) / m_timeToNext;
            m_declineSpeed = 2f / m_timeToNext;
            m_isTransition = true;
        }

        public float GetWeight(int index)
        {
            return index >= 0 && index < inputCount ? m_animMixer.GetInputWeight(index) : 0f;
        }

        public void SetWeight(int index, float weight)
        {
            if (index >= 0 && index < inputCount)
            {
                m_animMixer.SetInputWeight(index, weight);
            }
        }

        private float GetTargetEnter(int index)
        {
            var enterTime = GetChildBehaviour(index)?.GetEnterTime() ?? 0;
            if (enterTime <= 0)
            {
                enterTime = 0.01f;
            }
            return enterTime;
        }

        private AnimPlayableBehaviour GetChildBehaviour(int index)
        {
            if (index < 0 || index >= m_childBehaviours.Count)
                return null;
            return m_childBehaviours[index];
        }

        private float ModifyWeight(int index, float detail)
        {
            if (index < 0 || index >= inputCount)
                return 0;
            float weight = Mathf.Clamp01(GetWeight(index) + detail);
            m_animMixer.SetInputWeight(index, weight);
            return weight;
        }

        public static AnimLayer Create(AnimContext context)
        {
            Playable playable = ScriptPlayable<AnimLayer>.Create(context.Graph);
            var behaviour = ((ScriptPlayable<AnimLayer>)playable).GetBehaviour();
            behaviour.m_context = context;
            return behaviour;
        }

    }
}