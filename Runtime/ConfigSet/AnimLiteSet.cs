using UnityEngine;

namespace QDuck.Animation
{
    [CreateAssetMenu(menuName = "Animation/AnimLiteSet")]
    public class AnimLiteSet : ScriptableObject
    {
        [SerializeReference]
        public AnimLayerInfo LayerInfo;
    }
}