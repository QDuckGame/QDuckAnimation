using System;
using UnityEngine;
using UnityEngine.Playables;

namespace QDuck.Animation
{
    public interface IAnimInfo
    {
        AnimPlayableBehaviour CreateBehaviour(AnimContext context);
    }
}