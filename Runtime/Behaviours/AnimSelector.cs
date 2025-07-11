using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimSelector : AnimPlayableBehaviour
    {
        public int currentIndex { get; protected set; }
        public int clipCount { get; protected set; }
        protected AnimationMixerPlayable m_mixer;
        protected List<AnimationClipInfo> m_clipInfos;

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            m_mixer = AnimationMixerPlayable.Create(m_graph);
            playable.AddInput(m_mixer, 0, 1f);
            currentIndex = -1;
        }

        public override int AddInput(Playable playable)
        {
            base.AddInput(playable);
            m_mixer.AddInput(playable, 0);
            clipCount = m_mixer.GetInputCount();
            return m_mixer.GetInputCount() - 1;
        }

        public void AddInput(AnimationClipInfo clipInfo)
        {
            AddInput(AnimationClipPlayable.Create(m_graph, clipInfo.Clip));
            m_clipInfos ??= new List<AnimationClipInfo>();
            m_clipInfos.Add(clipInfo);
        }

        protected override void DoBehaviourStop(Playable playable)
        {
            base.DoBehaviourStop(playable);
            playable.Pause();
            Select();
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            if (m_isLoop)
            {
                var time = m_mixer.GetTime();
                if (currentIndex < 0 || time > GetAnimLength())
                {
                    Select();
                }
            }
        }

        private void SetMixerToCurrentIndex()
        {
            for (int i = 0; i < clipCount; i++)
            {
                m_mixer.SetInputWeight(i, i == currentIndex ? 1f : 0f);
            }

            m_mixer.SetTime(0f);
        }

        public int Select()
        {
            currentIndex = OnSelect();
            SetMixerToCurrentIndex();
            return currentIndex;
        }

        protected virtual int OnSelect()
        {
            return 0;
        }


        public override float GetEnterTime()
        {
            if (currentIndex < 0 || currentIndex >= m_clipInfos.Count)
                return 0f;
            return m_clipInfos[currentIndex].EnterTime;
        }

        public override float GetAnimLength()
        {
            if (currentIndex < 0 || currentIndex >= m_clipInfos.Count)
                return 0f;
            return m_clipInfos[currentIndex].Length;
        }
    }
}