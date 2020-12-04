using System;
using System.Collections.Generic;
using System.Text;

using FrooxEngine;

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
        public readonly List<AnimationFrame> Frames;

        /// <summary>
        /// A map mapping each slot to its name in the animation.
        /// Used to deal with duplicate slot names.
        /// </summary>
        public readonly Dictionary<Slot, string> NameMap = new Dictionary<Slot, string>();

        public AnimationFile(Slot slot)
        {
            this.Slot = slot;
            AddToNameMap(slot);
        }

        public AnimationFile(Slot slot, Slot parent)
        {
            this.Slot = slot;
            this.Parent = parent;
            AddToNameMap(slot);
        }

        protected void AddToNameMap(Slot slot)
        {
            NameMap.Add(slot, GenerateUniqueName(slot.Name));
            foreach(Slot child in slot.Children)
            {
                AddToNameMap(child);
            }
        }

        public void Capture()
        {
            if (Parent != null)
            {
                Frames.Add(new AnimationFrame(Slot, Parent, NameMap));
            } 
            else
            {
                Frames.Add(new AnimationFrame(Slot, new BaseX.float3(0, 0, 0), NameMap));
            }
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
