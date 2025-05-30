
namespace TradeSimulator.Shared.Extensions
{
    public static class ListExtensions
    {
        // Based on Fisher–Yates shuffle algorithm
        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random random = new Random();
            int n = list.Count;

            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }

            return list;
        }
    }
}
