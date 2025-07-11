using UnityEngine;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimPlayableBehaviour : PlayableBehaviour
    {
        protected PlayableGraph m_graph;
        protected bool m_isLoop;
        public Playable Playable { get; private set; }
        protected AnimContext m_context;
        public string name { get; protected set; }

        public override void OnPlayableCreate(Playable playable)
        {
            Playable = playable;
            m_graph = playable.GetGraph();
        }

        public virtual int AddInput(Playable playable)
        {
            return 0;
        }

        public void DoStop()
        {
            DoBehaviourStop(Playable);
        }

        protected virtual void DoBehaviourStop(Playable playable)
        {
        }


        public virtual float GetEnterTime()
        {
            return 0;
        }

        public virtual float GetAnimLength()
        {
            return 0;
        }

    }
}