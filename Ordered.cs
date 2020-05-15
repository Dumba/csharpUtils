namespace test_console
{
    public static class Ordered
    {
        public static int CountHigher(int[] orderedList, int item)
        {
            var iStart = 0;
            var iEnd = orderedList.Length;

            while (true)
            {
                var i = (iEnd - iStart) / 2 + iStart;
                if (orderedList[i] == item)
                {
                    return i;
                }
                if (orderedList[i] < item)
                {
                    if (iEnd == i)
                        return i;

                    iEnd = i;
                }
                else
                {
                    if (iStart == i)
                        return i + 1;

                    iStart = i;
                }
            }
        }
    }
}