using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimContext
    {
        public PlayableGraph Graph { get; private set; }
        public Animator Animator { get; private set; }
        private Dictionary<string, ShareVariable> _shareVariables;
        private string _name;

        public AnimContext(Animator animator)
        {
            Animator = animator;
            _name = animator.gameObject.name;
            Graph = PlayableGraph.Create($"Anim_{_name}{Animator.gameObject.GetHashCode()}");
            Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _shareVariables = new Dictionary<string, ShareVariable>();
        }

        public void SetSourcePlayable(Playable playable)
        {
            var output = AnimationPlayableOutput.Create(Graph, $"Anim_{_name}", Animator);
            output.SetSourcePlayable(playable);
        }

        #region variable

        public void SetFloat(string name, float value)
        {
            if (!_shareVariables.TryGetValue(name, out var variable))
            {
                variable = new ShareVariable();
                _shareVariables[name] = variable;
            }

            variable.SetFloat(value);
        }

        public void SetVector2(string name, float x, float y)
        {
            if (!_shareVariables.TryGetValue(name, out var variable))
            {
                variable = new ShareVariable();
                _shareVariables[name] = variable;
            }

            variable.SetVector2(x, y);
        }

        public float GetFloat(string name)
        {
            if (_shareVariables.TryGetValue(name, out var variable))
            {
                return variable.GetFloat();
            }

            return 0f;
        }

        public Vector2 GetVector2(string name)
        {
            if (_shareVariables.TryGetValue(name, out var variable))
            {
                return variable.GetVector2();
            }

            return Vector2.zero;
        }



        private class ShareVariable
        {
            private float floatValue1;
            private float floatValue2;

            public float GetFloat()
            {
                return floatValue1;
            }

            public Vector2 GetVector2()
            {
                return new Vector2(floatValue1, floatValue2);
            }

            public void SetFloat(float value)
            {
                floatValue1 = value;
            }

            public void SetVector2(float x, float y)
            {
                floatValue1 = x;
                floatValue2 = y;
            }
        }

        #endregion


    }
}