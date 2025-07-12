using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace QDuck.Animation
{
    public class AnimLite : Anim
    {
        [SerializeField] private AnimLiteSet _scriptableConfig;
        private AnimLayer _animLayer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _context = new AnimContext(_animator);
            _graph = _context.Graph;

            var layerInfo = _scriptableConfig.LayerInfo;
            if (layerInfo == null)
            {
                Debug.LogError("AnimLayerInfo configuration is missing!");
                return;
            }

            AnimLayer behaviour = layerInfo.CreateBehaviour(_context) as AnimLayer;
            _context.SetSourcePlayable(behaviour.Playable);
            if (_isImmediatePlay)
            {
                Play(_defaultAnimationName);
                Play();
            }
        }

        private void Start()
        {

        }
        
        public override void Play(string animationName, int layerIndex = 0)
        {
            if (_graph.IsValid() && _animLayer != null)
            {
                _animLayer.TransitionTo(animationName);
            }
        }

    }
}