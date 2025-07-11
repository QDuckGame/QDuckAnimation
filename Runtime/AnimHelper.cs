using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public class AnimHelper
    {

        public static void DoStop(AnimationMixerPlayable mixer, int index)
        {
            DoStop(mixer.GetInput(index));
        }

        public static void DoStop(Playable playable)
        {
            GetAnimationPlayableBehaviour(playable)?.DoStop();
        }

        public static AnimPlayableBehaviour GetAnimationPlayableBehaviour(Playable playable)
        {
            if (typeof(AnimPlayableBehaviour).IsAssignableFrom(playable.GetPlayableType()))
            {
                return ((ScriptPlayable<AnimPlayableBehaviour>)playable).GetBehaviour();
            }

            return null;
        }

    }
}