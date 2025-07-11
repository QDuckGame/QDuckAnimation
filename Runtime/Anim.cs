using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    [RequireComponent(typeof(Animator))]
    public class Anim : MonoBehaviour
    {
        [SerializeField] protected bool _isImmediatePlay;
        [SerializeField] protected string _defaultAnimationName;

        protected PlayableGraph _graph;
        protected Animator _animator;
        protected AnimContext _context;

        public virtual void Play(string animationName, int layerIndex = 0)
        {
            
        }

        public virtual void Stop(string animationName, int layerIndex = 0)
        {
            
        }

        public void Play()
        {
            if (_graph.IsValid())
            {
                _graph.Play();
            }
        }

        public void Stop()
        {
            if (_graph.IsValid())
            {
                _graph.Stop();
            }
        }

        public void SetFloat(string name, float value)
        {
            _context.SetFloat(name,value);
        }

        public void SetVector2(string name, float x, float y)
        {
            _context.SetVector2(name,x,y);
        }

        public float GetFloat(string name)
        {
            return _context.GetFloat(name);
        }

        public Vector2 GetVector2(string name)
        {
            return _context.GetVector2(name);
        }
    }
}