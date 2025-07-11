using UnityEngine;
using UnityEngine.Playables;

namespace QDuck.Animation
{
      public class AnimRandomSelector : AnimSelector
      {
            protected override int OnSelect()
            {
                  return Random.Range(0, clipCount);
            }

            public static AnimRandomSelector Create(AnimContext context, AnimRandomInfo info)
            {
                  AnimationClipInfo[] aniamtionInfos = info.Clips;
                  Playable playable = ScriptPlayable<AnimRandomSelector>.Create(context.Graph);
                  var behaviour = ((ScriptPlayable<AnimRandomSelector>)playable).GetBehaviour();
                  behaviour.m_context = context;
                  behaviour.name = info.Name;
                  foreach (var aniamtionInfo in aniamtionInfos)
                  {
                        behaviour.AddInput(aniamtionInfo);
                  }

                  behaviour.Select();
                  return behaviour;
            }


      }
}
