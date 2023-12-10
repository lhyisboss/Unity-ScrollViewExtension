using UnityEngine;

namespace ScrollViewExtension.Scripts.Configuration
{
    [CreateAssetMenu(fileName = "GroupLayoutConfig", menuName = "Configuration/GroupLayoutConfig")]
    public class GroupLayoutConfig : ScriptableObject
    {
        public bool reverseArrangement;
        public bool childForceExpandWidth;
        public bool childForceExpandHeight;
        public bool childControlWidth;
        public bool childControlHeight;
        public TextAnchor childAlignment;
        public Vector2 minAnchor;
        public Vector2 maxAnchor;
        public Vector2 pivot;
    }
}