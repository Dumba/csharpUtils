using System.Linq;

namespace test_console
{
    public class Overlap
    {
        public static Range[] Test()
        {
            var list = new Range[]
            {
                new Range(-1, 1),
                new Range(1, 5),
                new Range(6, 15),
                new Range(-10, 100),
                new Range(-10, -3),
                new Range(50, 100)
            };
            var filter = new Range(0, 10);

            var result = list.Where(i => In(filter, i)).ToArray();
            return result;
        }

        public static bool In(Range filter, Range item)
            => (filter.From < item.To) && (filter.To > item.From);
    }

    public struct Range
    {
        public Range(int? from, int? to)
        {
            From = from;
            To = to;
        }

        public int? From { get; }
        public int? To { get; }
    }
}