using System;
using ScrollViewExtension.Scripts.Common;
using ScrollViewExtension.Scripts.Entity.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Entity
{
    public class GroupLayoutEntity : IGroupLayoutEntity
    {
        private static GroupLayoutEntity instance;

        private readonly bool isVertical;

        public static GroupLayoutEntity CreateInstance(bool isVertical,
            bool reverseArrangement,
            bool childForceExpandWidth,
            bool childForceExpandHeight,
            bool childControlWidth,
            bool childControlHeight,
            TextAnchor childAlignment,
            Vector2 minAnchor,
            Vector2 maxAnchor)
        {
            return instance ?? new GroupLayoutEntity(isVertical,
                reverseArrangement,
                childForceExpandWidth,
                childForceExpandHeight,
                childControlWidth,
                childControlHeight,
                childAlignment,
                minAnchor,
                maxAnchor);
        }

        private bool reverseArrangement;
        public bool ReverseArrangement
        {
            get => reverseArrangement;
            set
            {
                if(value)
                    throw  new Exception("reverse arrangement can not be true");
                
                reverseArrangement = false;
            }
        }

        private bool childForceExpandWidth;
        public bool ChildForceExpandWidth
        {
            get => childForceExpandWidth;
            set
            {
                if(value)
                    throw  new Exception("child force expand width can not be true");
                
                childForceExpandWidth = false;
            }
        }

        private bool childForceExpandHeight;
        public bool ChildForceExpandHeight
        {
            get => childForceExpandHeight;
            set
            {
                if(value)
                    throw  new Exception("child force expand height can not be true");
                
                childForceExpandHeight = false;
            }
        }

        private bool childControlWidth;
        public bool ChildControlWidth
        {
            get => childControlWidth;
            set
            {
                if (isVertical)
                {
                    childControlWidth = value;
                }
                else
                {
                    if (value)
                        throw new Exception("child control width can not be true");
                    
                    childControlWidth = false;
                }
                
            }
        }

        private bool childControlHeight;
        public bool ChildControlHeight
        {
            get => childControlHeight;
            set
            {
                if (isVertical)
                {
                    if(value)
                        throw  new Exception("child control height can not be true");
                
                    childControlHeight = false;
                }
                else
                {
                    childControlHeight = value;
                }
            }
        }

        public bool ChildScaleWidth { get; set; }

        public bool ChildScaleHeight { get; set; }

        private TextAnchor childAlignment;
        public  TextAnchor ChildAlignment
        {
            get => childAlignment;
            set
            {
                if (value is not (TextAnchor.UpperLeft or TextAnchor.UpperCenter or TextAnchor.UpperRight))
                    throw new Exception("child alignment only can be upper");
                
                childAlignment = value;
            }
        }

        private Vector2 minAnchor;
        public Vector2 MinAnchor
        {
            get => minAnchor;
            set
            {
                if (value.x != 0 || Math.Abs(value.y - 1) > Const.Epsilon)
                    throw new Exception("content min anchor must be 0,1");
                
                minAnchor = value;
            }
        }

        private Vector2 maxAnchor;
        public Vector2 MaxAnchor
        {
            get => maxAnchor;
            set
            {
                if (Math.Abs(value.x - 1) > Const.Epsilon || Math.Abs(value.y - 1) > Const.Epsilon)
                    throw new Exception("content max anchor must be 1,1");
                
                maxAnchor = value;
            }
        }

        public void Dispose()
        {
            instance = null;
        }

        private GroupLayoutEntity(bool isVertical,
            bool reverseArrangement,
            bool childForceExpandWidth,
            bool childForceExpandHeight,
            bool childControlWidth,
            bool childControlHeight,
            TextAnchor childAlignment, 
            Vector2 minAnchor,
            Vector2 maxAnchor)
        {
            this.isVertical = isVertical;
            ReverseArrangement = reverseArrangement;
            ChildForceExpandWidth = childForceExpandWidth;
            ChildForceExpandHeight = childForceExpandHeight;
            ChildControlWidth = childControlWidth;
            ChildControlHeight = childControlHeight;
            ChildAlignment = childAlignment;
            MinAnchor = minAnchor;
            MaxAnchor = maxAnchor;
        }
    }
}