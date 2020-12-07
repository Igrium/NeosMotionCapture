using System;
using System.Collections.Generic;
using System.Text;

using FrooxEngine;
using BaseX;

namespace NeosMotionCapture
{
    /// <summary>
    /// Represents an animation that's in the process of being recorded.
    /// </summary>
    class AnimationFile
    {
        /// <summary>
        /// The root slot of this animation.
        /// </summary>
        public readonly Slot Slot;

        /// <summary>
        /// The slot this animation is being recorded relative to.
        /// </summary>
        public readonly Slot Parent;

        /// <summary>
        /// All the frames in this animation.
        /// </summary>
        public readonly List<AnimationFrame> Frames = new List<AnimationFrame>();

        /// <summary>
        /// A map mapping each slot to its name in the animation.
        /// Used to deal with duplicate slot names.
        /// </summary>
        public readonly Dictionary<Slot, string> NameMap = new Dictionary<Slot, string>();

        /// <summary>
        /// The offset positions of each slot.
        /// </summary>
        public readonly Dictionary<string, float3> Offsets = new Dictionary<string, float3>();

        public double FrameTimeMillis = 33.3333;

        public AnimationFile(Slot slot)
        {
            this.Slot = slot;
            InitSlot(slot, true);
        }

        public AnimationFile(Slot slot, Slot parent)
        {
            this.Slot = slot;
            this.Parent = parent;
            InitSlot(slot, true);
        }

        protected void InitSlot(Slot slot, bool isRoot)
        {
            string name = GenerateUniqueName(slot.Name);
            NameMap.Add(slot, name);
            
            if (isRoot)
            {
                Offsets.Add(name, new float3(0, 0, 0))
;           }
            else
            {
                Offsets.Add(name, slot.LocalPosition);
            }

            foreach(Slot child in slot.Children)
            {
                InitSlot(child, false);
            }
        }

        public void Capture()
        {
            AnimationFrame frame = null;
            if (Parent != null)
            {
                frame = new AnimationFrame(Slot, Parent, NameMap);
            } 
            else
            {
                frame = new AnimationFrame(Slot, new BaseX.float3(0, 0, 0), NameMap);
            }
            frame.LoadChildren(Slot, NameMap);
            Frames.Add(frame);
        }

        private string GenerateUniqueName(string name)
        {
            if (!NameMap.ContainsValue(name))
            {
                return name;
            }
            else
            {
                int num = 2;
                string newName = name + num;
                while (NameMap.ContainsValue(newName))
                {
                    num++;
                    newName = name + num;
                }
                return newName;
            }
        }
    }
}
