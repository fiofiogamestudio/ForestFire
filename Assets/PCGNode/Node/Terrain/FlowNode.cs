using UnityEngine;

using System.Collections.Generic;

namespace PCGNode
{
    public static class FlowNode
    {
        public static Texture2D CalcFlowmapSimple(Texture2D tex, bool DebugFlowMap = false)
        {
            // float max = 0.0f;
            Texture2D flowMap = new Texture2D(tex.width, tex.height);
            // for (int i = 0; i < tex.width; i++) flowMap.SetPixel(i,)
            for (int i = 1; i < tex.width - 1; i++)
            {
                for (int j = 1; j < tex.height - 1; j++)
                {
                    float dx =
                        +tex.GetPixel(i + 1, j).r
                        + tex.GetPixel(i + 1, j - 1).r
                        + tex.GetPixel(i + 1, j + 1).r
                        - tex.GetPixel(i - 1, j).r
                        - tex.GetPixel(i - 1, j - 1).r
                        - tex.GetPixel(i - 1, j + 1).r
                        ;
                    float dy =
                        +tex.GetPixel(i, j + 1).r
                        + tex.GetPixel(i - 1, j + 1).r
                        + tex.GetPixel(i + 1, j + 1).r
                        - tex.GetPixel(i, j - 1).r
                        - tex.GetPixel(i - 1, j - 1).r
                        - tex.GetPixel(i + 1, j - 1).r
                        ;
                    if (DebugFlowMap)
                    {
                        if (dx != 0.0f && dy != 0.0f)
                        {
                            if (Mathf.Abs(dx) > Mathf.Abs(dy))
                            {
                                if (dx > 0.0f)
                                    flowMap.SetPixel(i, j, Color.red);
                                else
                                    flowMap.SetPixel(i, j, Color.blue);
                            }
                            else
                            {
                                if (dy > 0.0f)
                                    flowMap.SetPixel(i, j, Color.red);
                                else
                                    flowMap.SetPixel(i, j, Color.blue);
                            }
                        }
                        else
                            flowMap.SetPixel(i, j, Color.black);
                    }
                    else
                    {
                        if (dx != 0.0f && dy != 0.0f)
                        {
                            if (Mathf.Abs(dx) > Mathf.Abs(dy))
                            {
                                if (dx < 0.0f)
                                {

                                    flowMap.SetPixel(i, j, new Color(1, 0.5f, 0));
                                    flowMap.SetPixel(i, j + 1, new Color(1, 0.5f, 0));
                                    flowMap.SetPixel(i, j - 1, new Color(1, 0.5f, 0));
                                }
                                else
                                {
                                    flowMap.SetPixel(i, j, new Color(0, 0.5f, 0));
                                    flowMap.SetPixel(i, j + 1, new Color(0, 0.5f, 0));
                                    flowMap.SetPixel(i, j - 1, new Color(0, 0.5f, 0));

                                }
                            }
                            else
                            {
                                if (dy < 0.0f)
                                {
                                    flowMap.SetPixel(i, j, new Color(0.5f, 1, 0));
                                    flowMap.SetPixel(i + 1, j, new Color(0.5f, 1, 0));
                                    flowMap.SetPixel(i - 1, j + 1, new Color(0.5f, 1, 0));

                                }
                                else
                                {

                                    flowMap.SetPixel(i, j, new Color(0.5f, 0, 0));
                                    flowMap.SetPixel(i + 1, j, new Color(0.5f, 0, 0));
                                    flowMap.SetPixel(i - 1, j, new Color(0.5f, 0, 0));
                                }
                            }
                        }
                        else
                            flowMap.SetPixel(i, j, new Color(0.5f, 0.5f, 0));
                    }
                }



            }




            flowMap.Apply();
            return flowMap;
        }

    }


    public static class TextureAlgo
    {
        public static int PixelCount = 0;
        public static int PixelWidth = 0;
        public static Texture2D CalcFlowMapCPU_WaterShed(Texture2D tex, int level = 20, bool only_slope = false)
        {
            Debug.Log("Start WaterShed " + level + " slope " + only_slope);

            Texture2D flowMap = new Texture2D(tex.width, tex.height);
            PixelCount = tex.width * tex.height;
            PixelWidth = tex.width;
            // Divide
            int[] dividedResult = DivideTex(tex, level);

            // Debug Draw RResult
            {
                // for (int i = 0; i < PixelCount; i++)
                // {
                //     flowMap.SetPixel(i % tex.width, i / tex.width, new Color((float)dividedResult[i] / level, (float)dividedResult[i] / level, (float)dividedResult[i] / level, 1));
                // }        
            }

            // Grourp
            SortedDictionary<int, List<int>> groupedResult = GroupResult(ref dividedResult);

            // Debug Group Count
            foreach (var pair in groupedResult)
            {
                Debug.Log("Height : " + pair.Key + ", Count : " + pair.Value.Count);
            }

            // Debug Draw Lowest Area
            float step = 1.0f / level;
            // {
            // List<int> lowestSet = new List<int>();
            // foreach(var key in groupedResult.Keys)
            // {
            //     lowestSet = groupedResult[key];
            //     for (int i = 0; i < lowestSet.Count; i++)
            //     {

            //         flowMap.SetPixel(lowestSet[i] % tex.width, lowestSet[i] / tex.width, new Color(step * key, step * key, step * key));
            //     }
            // }
            // }

            // Watershed
            bool trackOrArea = true;
            if (trackOrArea)
            {
                float[] result = CalcWatershed_Track(groupedResult, only_slope, true);
                level = 5;
                for (int i = 0; i < PixelCount; i++)
                {
                    Color color = new Color(
                        ((float)result[i * 2] / level + 1) / 2,
                        ((float)result[i * 2 + 1] / level + 1) / 2,
                        0,
                        1
                    );
                    flowMap.SetPixel(
                        i % PixelWidth,
                        i / PixelWidth,
                        color
                    );
                }
            }
            else
            {
                // float[] result = CalcWatershed_WaterLevel(groupedResult, false, 1.0f, 10.0f);
                float[] result = CalcWatershed_Lowset(groupedResult, 1.0f, 16, 10.0f);
                for (int i = 0; i < PixelCount; i++)
                {
                    Color color = new Color(
                        1 - result[i], 1 - result[i], 1
                    );
                    flowMap.SetPixel(i % PixelWidth, i / PixelWidth, color);
                }
            }


            flowMap.Apply();
            return flowMap;
        }

        /// <summary>
        /// Divide Tex to R value Array
        /// </summary>
        /// <param name="toDivide"></param>
        /// <param name="level"></param>
        public static int[] DivideTex(Texture2D toDivide, int level)
        {
            int[] result = new int[toDivide.width * toDivide.height];
            Color[] pixels = toDivide.GetPixels();
            for (int i = 0; i < pixels.Length; i++) result[i] = Mathf.FloorToInt(pixels[i].r * level);
            return result;
        }

        public static SortedDictionary<int, List<int>> GroupResult(ref int[] dividedResult)
        {
            SortedDictionary<int, List<int>> groupedResult = new SortedDictionary<int, List<int>>();
            for (int i = 0; i < dividedResult.Length; i++)
            {
                if (!groupedResult.ContainsKey(dividedResult[i]))
                {
                    groupedResult.Add(dividedResult[i], new List<int>() { i });
                }
                else
                {
                    groupedResult[dividedResult[i]].Add(i);
                }
            }
            // Sort

            return groupedResult;
        }

        public static float[] CalcWatershed_Track(SortedDictionary<int, List<int>> groupedResult, bool only_slope, bool should_lerp)
        {
            float[] result = new float[PixelCount * 2];
            int[] record = new int[PixelCount];
            for (int i = 0; i < record.Length; i++) record[i] = 0;

            int lastLevel = -1;
            float adj_x = -1, adj_y = -1;
            int count = 0;
            int jumpLoop = 1000000;
            foreach (var pair in groupedResult)
            {
                int level = pair.Key;
                List<int> pixels = pair.Value;
                List<int> temp = new List<int>();
                // check
                bool newAdd = true;
                while (pixels.Count != 0 && newAdd)
                {
                    newAdd = false;
                    foreach (var pixel in pixels)
                    {
                        if (searchAroundTrack(pixel, level, ref record, ref result, ref adj_x, ref adj_y, only_slope, should_lerp))
                        {
                            result[2 * pixel] = adj_x;
                            result[2 * pixel + 1] = adj_y;
                            newAdd = true;
                            temp.Add(pixel);
                        }
                    }

                    // Debug.Log("Level " + level + " Add " + temp.Count);
                    foreach (var pixel in temp)
                    {
                        record[pixel] = level;
                        pixels.Remove(pixel);
                    }

                    temp.Clear();

                    if (jumpLoop-- < 0) break;
                }

                foreach (var pixel in pixels)
                {
                    result[2 * pixel] = 0;
                    result[2 * pixel + 1] = 0;
                    record[pixel] = level;
                }


                lastLevel = level;
                // if (count > 1) break;
                count++;
                // write
            }

            return result;
        }
        private static bool searchAroundTrack(int index, int level, ref int[] record, ref float[] result, ref float adj_x, ref float adj_y, bool only_slope, bool should_lerp)
        {
            bool find = false;
            adj_x = 0;
            adj_y = 0;
            if (index - 1 > -1 && index + 1 < PixelCount && index - PixelWidth > -1 && index + PixelWidth < PixelCount)
            {
                // left and right
                if (record[index - 1] != 0)
                {
                    adj_x = -(level - record[index - 1]);
                    find = true;
                }
                else if (record[index + 1] != 0)
                {
                    adj_x = (level - record[index + 1]);
                    find = true;
                }
                // up and down
                if (record[index - PixelWidth] != 0)
                {
                    adj_y = -(level - record[index - PixelWidth]);
                    find = true;
                }
                else if (record[index + PixelWidth] != 0)
                {
                    adj_y = (level - record[index + PixelWidth]);
                    find = true;
                }
            }
            if (!only_slope && find && adj_x == 0 && adj_y == 0)
            {
                int rec_x = 0, rec_y = 0;
                if (record[index - 1] != 0) rec_x = -1;
                else if (record[index + 1] != 0) rec_x = 1;
                if (record[index - PixelWidth] != 0) rec_y = -1;
                else if (record[index + PixelWidth] != 0) rec_y = 1;

                if (should_lerp)
                {
                    if (rec_x != 0 && rec_y != 0)
                    {
                        adj_x = (result[(index + rec_x) * 2] + result[(index + rec_y * PixelWidth) * 2]) / 2;
                        adj_y = (result[(index + rec_x) * 2 + 1] + result[(index + rec_y * PixelWidth) * 2 + 1]) / 2;
                    }
                    else if (rec_x != 0 && rec_y == 0)
                    {
                        // adj_x = (float) rec_x;
                        adj_x = result[(index + rec_x) * 2];
                    }
                    else if (rec_y != 0 && rec_x == 0)
                    {
                        adj_y = result[(index + rec_y * PixelWidth) * 2 + 1];
                    }
                }
                else
                {
                    adj_x = rec_x;
                    adj_y = rec_y;
                }

            }
            return find;
        }

        private static float[] CalcWatershed_WaterLevel(SortedDictionary<int, List<int>> groupedResult, bool use_local, float color_step, float water_level)
        {
            float[] result = new float[PixelCount];
            for (int i = 0; i < result.Length; i++) result[i] = 0.0f;
            int[] record = new int[PixelCount];
            for (int i = 0; i < record.Length; i++) record[i] = -1;
            int searched_index = 0;
            int local_height = 0;
            int count = 0;
            foreach (var pair in groupedResult)
            {
                int level = pair.Key;
                List<int> pixels = pair.Value;
                List<int> temp = new List<int>();
                // check
                bool newAdd = true;
                while (pixels.Count != 0 && newAdd)
                {
                    newAdd = false;
                    foreach (var pixel in pixels)
                    {
                        if (searchAroundArea(pixel, level, ref record, ref searched_index, ref local_height))
                        {
                            if (level > water_level)
                            {
                                // float target = result[searched_index] - ((local_height == 1) ? color_step : color_step / 1.41f);
                                float target = result[searched_index] - color_step;
                                target = target < 0 ? 0 : target;
                                result[pixel] = target;
                            }
                            else
                            {
                                result[pixel] = 1;
                            }
                            newAdd = true;
                            temp.Add(pixel);
                        }
                    }
                    Debug.Log("Add " + temp.Count);
                    foreach (var pixel in temp)
                    {
                        record[pixel] = level;
                        pixels.Remove(pixel);
                    }
                    temp.Clear();
                }
                foreach (var pixel in pixels)
                {
                    if (level < water_level) result[pixel] = 1;
                    // if (use_local)
                    // {
                    //     result[pixel] = 1;
                    // }
                    record[pixel] = level;
                }
                count++;
                // if (count > 10) break;
            }
            return result;
        }

        private static bool searchAroundArea(int index, int level, ref int[] record, ref int searched_index, ref int local_height)
        {
            bool find = false;
            // int left = 0, right = 0, up = 0, down = 0, min = 99999999;
            local_height = 0;
            if (index - 1 > -1 && index + 1 < PixelCount && index - PixelWidth > -1 && index + PixelWidth < PixelCount)
            {
                if (record[index - 1] != -1)
                {
                    searched_index = index - 1;
                    local_height++;
                    find = true;
                }
                if (record[index + 1] != -1)
                {
                    searched_index = index + 1;
                    local_height++;
                    find = true;
                }
                if (record[index - PixelWidth] != -1)
                {
                    searched_index = index - PixelWidth;
                    local_height++;
                    find = true;
                }
                if (record[index + PixelWidth] != -1)
                {
                    searched_index = index + PixelWidth;
                    local_height++;
                    find = true;
                }
                // if (record[index + 1] != -1) 
                // {
                //     right = level - record[index + 1];
                //     min = right < min ? right : min;
                //     find = true;
                // }
                // if (record[index - PixelWidth] != -1) 
                // {
                //     down = level - record[index - PixelWidth];
                //     min = down < min ? down : min;
                //     find = true;
                // }
                // if (record[index + PixelWidth] != -1) 
                // {
                //     up = level - record[index + PixelWidth];
                //     min = up < min ? up : min;
                //     find = true;
                // }
                // if (find)
                // {
                //     if (left <= min)
                //     {
                //         searched_index = index - 1;
                //     }
                //     else if (right <= min)
                //     {
                //         searched_index = index + 1;
                //     }
                //     else if (down <= min)
                //     {
                //         searched_index = index - PixelWidth;
                //     }
                //     else if (up <= min)
                //     {
                //         searched_index = index + PixelWidth;
                //     }
                //     local_height = min;
                //     Debug.Log(local_height);
                // }
            }

            return find;
        }

        private static float[] CalcWatershed_Lowset(SortedDictionary<int, List<int>> groupedResult, float color_step, int min_lake_area, float min_water_level)
        {
            float[] result = new float[PixelCount];
            for (int i = 0; i < result.Length; i++) result[i] = 0.0f;
            int[] record = new int[PixelCount];
            for (int i = 0; i < record.Length; i++) record[i] = -1;
            int searched = 0;
            foreach (var pair in groupedResult)
            {
                int level = pair.Key;
                List<int> pixels = pair.Value;
                List<int> temp = new List<int>();
                // check
                bool newAdd = true;
                while (pixels.Count != 0 && newAdd)
                {
                    newAdd = false;
                    foreach (var pixel in pixels)
                    {
                        if (searchAround(pixel, ref record, ref searched))
                        {
                            newAdd = true;
                            float value = result[searched] - color_step;
                            result[pixel] = value < 0 ? 0 : value;
                            temp.Add(pixel);
                        }
                    }
                    foreach (var pixel in temp)
                    {
                        record[pixel] = 1;
                        pixels.Remove(pixel);
                    }
                    temp.Clear();
                }
                // handle lake
                if (level < min_water_level)
                {
                    foreach (var pixel in pixels)
                    {
                        record[pixel] = 1;
                    }
                }
                else
                {
                    List<int> lake = new List<int>();
                    while (pixels.Count != 0)
                    {
                        Debug.Log(level + "before " + pixels.Count);
                        lake.Clear();
                        lake.Add(pixels[0]);
                        record[pixels[0]] = 1;
                        pixels.RemoveAt(0);
                        newAdd = true;
                        while (newAdd)
                        {
                            newAdd = false;
                            temp.Clear();
                            foreach (var pixel in pixels)
                            {
                                if (searchAroundSet(pixel, ref lake))
                                {
                                    temp.Add(pixel);
                                    newAdd = true;
                                }
                            }
                            foreach (var pixel in temp)
                            {
                                record[pixel] = 1;
                                lake.Add(pixel);
                                pixels.Remove(pixel);
                            }
                            // Debug.Log(level + " find" + lake.Count + " " + newAdd); 
                        }
                        if (lake.Count > min_lake_area)
                        {
                            Debug.Log("Lake Area " + lake.Count);
                            foreach (var pixel in lake)
                            {
                                result[pixel] = 1;
                                //    if (temp.Count < 100)
                                //    Debug.Log((pixel % PixelWidth) + " " + (pixel / PixelWidth));
                            }
                        }
                        Debug.Log(level + "after " + pixels.Count);
                    }
                }
            }
            return result;
        }

        public static bool searchAround(int index, ref int[] record, ref int searched)
        {
            if (index - 1 > -1 && index + 1 < PixelCount && index - PixelWidth > -1 && index + PixelWidth < PixelCount)
            {
                if (record[index - 1] != -1)
                {
                    searched = index - 1;
                    return true;
                }
                else if (record[index + 1] != -1)
                {
                    searched = index + 1;
                    return true;
                }
                else if (record[index - PixelWidth] != -1)
                {
                    searched = index - PixelWidth;
                    return true;
                }
                else if (record[index + PixelWidth] != -1)
                {
                    searched = index + PixelWidth;
                    return true;
                }
            }
            return false;
        }

        public static bool searchAroundSet(int index, ref List<int> set)
        {
            return set.Contains(index - 1) || set.Contains(index + 1) || set.Contains(index - PixelWidth) || set.Contains(index + PixelWidth);
        }

        public static bool searchAroundSet8(int index, ref List<int> set)
        {
            return
                set.Contains(index - 1) ||
                set.Contains(index + 1) ||
                set.Contains(index - PixelWidth) ||
                set.Contains(index + PixelWidth) ||
                set.Contains(index - 1 - PixelWidth) ||
                set.Contains(index + 1 + PixelWidth) ||
                set.Contains(index - 1 + PixelWidth) ||
                set.Contains(index + 1 - PixelWidth);
        }
    }
}