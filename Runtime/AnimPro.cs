using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimPro : Anim
    {
        [SerializeField] private AnimProSet _scriptableConfig;

        private AnimationLayerMixerPlayable _layerMixerPlayable;
        private List<AnimLayer> _layers = new List<AnimLayer>();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _context = new AnimContext(_animator);
            _graph = _context.Graph;
            var layerInfos = _scriptableConfig.LayerInfos;
            if (layerInfos == null || layerInfos.Length == 0)
            {
                Debug.LogError("Animation layers configuration is missing!");
                return;
            }

            _layerMixerPlayable = AnimationLayerMixerPlayable.Create(_graph);
            for (int i = 0; i < layerInfos.Length; i++)
            {
                var layerInfo = layerInfos[i];
                AnimLayer behaviour = layerInfo.CreateBehaviour(_context) as AnimLayer;
                _layers.Add(behaviour);
                _layerMixerPlayable.AddInput(behaviour.Playable, 0, 1);
                _layerMixerPlayable.SetLayerMaskFromAvatarMask((uint)i, layerInfo.Mask);
                _layerMixerPlayable.SetLayerAdditive((uint)i, layerInfo.IsAdditive);
                _layerMixerPlayable.SetInputWeight(i, layerInfo.Weight);
                behaviour.Playable.Play();
            }

            _context.SetSourcePlayable(_layerMixerPlayable);

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
            if (_graph.IsValid() && layerIndex < _layers.Count)
            {
                var layer = _layers[layerIndex];
                layer.TransitionTo(animationName);
            }
        }

        public override void Stop(string animationName, int layerIndex = 0)
        {
            base.Stop(animationName, layerIndex);
            if (_graph.IsValid() && layerIndex < _layers.Count)
            {
                var layer = _layers[layerIndex];
                layer.StopTarget(animationName);
            }
        }
        

    }
}