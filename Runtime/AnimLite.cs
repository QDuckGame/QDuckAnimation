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
        [SerializeField] private AnimConfigWrapper _configWrapper = new AnimConfigWrapper();
        private AnimLayer _animLayer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _context = new AnimContext(_animator);
            _graph = _context.Graph;

            var layerInfo = _configWrapper.GetConfig();
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

        // 编辑器辅助方法（可选）
#if UNITY_EDITOR
        public void SwitchConfigType(AnimConfigType newType)
        {
            _configWrapper.configType = newType;
        }
#endif

    }
}