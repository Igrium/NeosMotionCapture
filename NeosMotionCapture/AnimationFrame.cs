using System;
using System.Collections.Generic;
using System.Text;

using FrooxEngine;
using BaseX;

namespace NeosMotionCapture
{
    /// <summary>
    /// Represents a single frame in a captured animation.
    /// </summary>
    class AnimationFrame
    {
        public readonly List<AnimationFrame> Children = new List<AnimationFrame>();
        public readonly float3 Position;
        public readonly floatQ Rotation;
        public string Name { get; private set; }

        /// <summary>
        /// Create an animation frame from a slot relative to its parent.
        /// </summary>
        /// <param name="slot">The slot to create the frame from.</param>
        public AnimationFrame(Slot slot, Dictionary<Slot, string> nameMap)
        {
            Position = slot.LocalPosition;
            Rotation = slot.LocalRotation;

            LoadName(slot, nameMap);
        }

        /// <summary>
        /// Create an animation frame from a slot in global space. 
        /// </summary>
        /// <param name="slot">The slot to create the frame from.</param>
        /// <param name="basePosition">The position to create it relative to.</param>
        public AnimationFrame(Slot slot, float3 basePosition, Dictionary<Slot, string> nameMap)
        {
            Position = slot.GlobalPosition - basePosition;
            Rotation = slot.GlobalRotation;

            LoadName(slot, nameMap);
        }

        /// <summary>
        /// Create an animation frame from a slot relative to another slot.
        /// </summary>
        /// <param name="slot">The slot to create the frame from.</param>
        /// <param name="parent">The slot to create it relative to.</param>
        public AnimationFrame(Slot slot, Slot parent, Dictionary<Slot, string> nameMap)
        {
            Position = slot.GlobalPosition - parent.GlobalPosition;
            Rotation = slot.GlobalRotation - parent.GlobalRotation;

            LoadName(slot, nameMap);
        }

        public AnimationFrame(float3 position, floatQ rotation, string name)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Name = name;
        }


        private void LoadName(Slot slot, Dictionary<Slot, string> nameMap)
        {
            if (nameMap.ContainsKey(slot))
            {
                Name = nameMap[slot];
            } 
            else
            {
                Name = slot.Name;
            }

        }

        /// <summary>
        /// Load all the children of this frame, determining if any of them need to be captured.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="nameMap"></param>
        /// <returns>Whether this or any of its children need to be captured.</returns>
        public bool LoadChildren(Slot slot, Dictionary<Slot, string> nameMap)
        {
            bool childrenCaptured = ShouldSlotBeCaptured(slot);
            foreach (Slot child in slot.Children)
            {
                AnimationFrame frame = new AnimationFrame(child, nameMap);
                if (frame.LoadChildren(child, nameMap))
                {
                    childrenCaptured = true;
                    // Only add the child to the list if it should be captured.
                    Children.Add(frame);
                }
            }
            return childrenCaptured;
        }

        /// <summary>
        /// Determine whether a slot should be recorded.
        /// Useful for sorting out pure-logic slots.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool ShouldSlotBeCaptured(Slot slot)
        {
            foreach (Component component in slot.Components)
            {
                if (component.GetType().IsAssignableFrom(typeof(MeshRenderer)))
                {
                    return true;
                }
                else if (component.GetType().IsAssignableFrom(typeof(Comment)))
                {
                    Comment comment = (Comment)component;
                    if (comment.Text.Value == "CaptureMotion") { return true; }
                }
            }

            return false;
        }
    }
}
