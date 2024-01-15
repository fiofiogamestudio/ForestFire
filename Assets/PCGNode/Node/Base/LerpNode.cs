namespace PCGNode
{
    public static class LerpNode
    {
        public static float[] Lerp(float[] a, float[] b, float t)
        {
            float[] c = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                c[i] = a[i] * (1 - t) + b[i] * t;
            }
            return c;
        }

    }


    public static class MappingNode
    {
        public delegate float MappingEvent(float x);
        public static float[] Mapping(float[] a, MappingEvent mapping)
        {
            float[] b = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                b[i] = mapping(a[i]);
            }
            return b;
        }
    }

    public static class NormalizeNode
    {
        public static float[] Normalize(float[] a)
        {
            float[] b = new float[a.Length];
            float min = 1.0f;
            float max = 0.0f;
            for (int i = 0; i < a.Length; i++)
            {
                min = a[i] < min ? a[i] : min;
                max = a[i] > max ? a[i] : max;
            }
            for (int i = 0; i < a.Length; i++)
            {
                b[i] = (a[i] - min) / (max - min);
            }
            return b;
        }
    }

    public static class MathNode
    {
        public static float[] Influence(float[] a, float[] b, float t)
        {
            float[] c = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                c[i] = a[i] + b[i] * t;
            }
            return c;
        }
    }
}