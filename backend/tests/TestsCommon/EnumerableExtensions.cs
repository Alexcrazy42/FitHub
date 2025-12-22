namespace FitHub.TestsCommon;

public static class EnumerableExtensions
{
    private static readonly Random Random = Random.Shared;

    public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> source, Random? random = null, int? sampleSize = null)
    {
        random ??= Random;

        var buffer = source.ToList();
        var count = buffer.Count;

        sampleSize ??= random.Next(buffer.Count + 1);

        if (sampleSize == 0 || count == 0)
        {
            return [];
        }

        sampleSize = Math.Min(sampleSize.Value, count);

        for (var i = 0; i < sampleSize; i++)
        {
            var j = random.Next(i, count);
            (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
        }

        return buffer.Take(sampleSize.Value);
    }

    public static T RandomSingle<T>(this IEnumerable<T> source, Random? random = null)
        => source.RandomSample(random, 1).Single();
}
