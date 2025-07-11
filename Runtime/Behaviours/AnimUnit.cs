using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimUnit : AnimPlayableBehaviour
    {
        private AnimationClipPlayable m_anim;
        private AnimUnitInfo m_info;

        public override float GetAnimLength()
        {
            return m_info.Length;
        }

        public override float GetEnterTime()
        {
            return m_info.EnterTime;
        }

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            m_anim.SetSpeed(m_info.Speed);
            m_anim.Play();
        }


        protected override void DoBehaviourStop(Playable playable)
        {
            base.DoBehaviourStop(playable);
            playable.Pause();
            this.m_anim.SetTime(0);
            
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            if (m_isLoop)
            {
                var time = this.m_anim.GetTime();
                if (time >= GetAnimLength())
                {
                    this.m_anim.SetTime(time - GetAnimLength());
                }
            }
        }

        public static AnimUnit Create(AnimContext context, AnimUnitInfo info)
        {
            Playable playable = ScriptPlayable<AnimUnit>.Create(context.Graph);
            var behaviour = ((ScriptPlayable<AnimUnit>)playable).GetBehaviour();
            behaviour.m_context = context;
            behaviour.m_info = info;
            behaviour.m_anim = AnimationClipPlayable.Create(behaviour.m_graph, info.Clip);
            playable.AddInput(behaviour.m_anim, 0, 1);
            behaviour.name = info.Name;
            return behaviour;
        }

    }
}