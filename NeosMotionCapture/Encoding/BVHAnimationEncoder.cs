using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BaseX;

namespace NeosMotionCapture.Encoding
{
    /// <summary>
    /// Encodes animations into the BVH format.
    /// </summary>
    class BVHAnimationEncoder : IAnimationEncoder
    {
        public void EncodeAnimation(AnimationFile animation, Stream stream)
        {
            StreamWriter file = new StreamWriter(stream);
            WriteAnimation(animation, file);
            file.Close();
        }

        public void WriteAnimation(AnimationFile animation, StreamWriter file)
        {

            // Node name list in the order that the nodes are written.
            List<string> serializedNames = new List<string>();

            file.WriteLine("HIERARCHY");

            void writeRecursiveNodes(AnimationFrame node, int indent)
            {
                string indentStr = new String('\t', indent);

                if (indent != 0)
                {
                    file.WriteLine(indentStr + "JOINT " + node.Name);
                }
                else
                {
                    file.WriteLine(indentStr + "ROOT " + node.Name);
                }

                // TODO: Make this properly read the model's bind pose.
                float3 offset = animation.Offsets[node.Name];

                file.WriteLine(indentStr + "{");
                file.WriteLine(indentStr + "\tOFFSET " + offset.x + " " + offset.y + " " + offset.z);
                file.WriteLine(indentStr + "\tCHANNELS 6 Xposition Yposition Zposition Xrotation Yrotation Zrotation");

                if (node.Children.Count > 0)
                {
                    // Write children
                    foreach (AnimationFrame child in node.Children)
                    {
                        serializedNames.Add(child.Name);
                        writeRecursiveNodes(child, indent + 1);
                    }
                }
                else
                {
                    // Write the bone end.
                    file.WriteLine(indentStr + "\tEnd Site");
                    file.WriteLine(indentStr + "\t{");
                    file.WriteLine(indentStr + "\t\tOFFSET .25 .25 .25");
                    file.WriteLine(indentStr + "\t}");
                }
                file.WriteLine(indentStr + "}");
            }

            // Get first frame to create skeleton from.
            AnimationFrame root = animation.Frames[0];

            serializedNames.Add(root.Name);
            writeRecursiveNodes(root, 0);

            List<AnimationFrame> frames = animation.Frames;
            file.WriteLine("MOTION");
            file.WriteLine("Frames: " + frames.Count);
            file.WriteLine("Frame Time: " + animation.FrameTimeMillis / 1000);

            void writeNode(AnimationFrame node)
            {
                // Convert quaternion to euler and radians to degrees.
                float3 rotation = (float3)(node.Rotation.xyz * (180/Math.PI));
                
                file.Write(node.Position.x + " " + node.Position.y + " " + node.Position.z + " " + rotation.x + " " + rotation.y + " " + rotation.z + " ");

                if (node.Children.Count > 0)
                {
                    foreach (AnimationFrame child in node.Children)
                    {
                        writeNode(child);
                    }
                }
            }

            for (int i = 0; i < frames.Count; i++)
            {
                writeNode(frames[i]);
                file.WriteLine("");
            }
        }
    }
}
