namespace PCGNode
{
    public static class BlurNode
    {
        public static float[] BlurSimple(float[] heightmap, int width)
        {
            float[] bluredHeightmap = new float[width * width];
            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    float sum = 0;
                    int count = 0;
                    // current pixel
                    sum += heightmap[j * width + i];
                    count++;
                    if (i > 0)
                    {
                        sum += heightmap[j * width + i - 1];
                        count++;
                    }
                    if (i < width - 1)
                    {
                        sum += heightmap[j * width + i + 1];
                        count++;
                    }
                    if (j > 0)
                    {
                        sum += heightmap[j * width + i - width];
                        count++;
                    }
                    if (j < width - 1)
                    {
                        sum += heightmap[j * width + i + width];
                        count++;
                    }
                    // blur
                    bluredHeightmap[j * width + i] = sum /= count;
                }
            }
            return bluredHeightmap;
        }
    }
}