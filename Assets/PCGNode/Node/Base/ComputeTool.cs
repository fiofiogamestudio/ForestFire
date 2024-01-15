using UnityEngine;
using System.Collections.Generic;

namespace PCGNode
{
    public static class ComputeTool
    {
        public static ComputeBuffer GetBuffer(List<int> data)
        {
            ComputeBuffer buffer = new ComputeBuffer(data.Count, sizeof(int));
            buffer.SetData(data);
            return buffer;
        }

        public static ComputeBuffer GetBuffer(List<float> data)
        {
            ComputeBuffer buffer = new ComputeBuffer(data.Count, sizeof(float));
            buffer.SetData(data);
            return buffer;
        }

        public static ComputeBuffer GetBuffer(int[] data)
        {
            ComputeBuffer buffer = new ComputeBuffer(data.Length, sizeof(int));
            buffer.SetData(data);
            return buffer;
        }

        public static ComputeBuffer GetBuffer(float[] data)
        {
            ComputeBuffer buffer = new ComputeBuffer(data.Length, sizeof(float));
            buffer.SetData(data);
            return buffer;
        }
    }
}
