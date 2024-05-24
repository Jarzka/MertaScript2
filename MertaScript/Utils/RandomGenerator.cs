namespace MertaScript.Utils;

public static class RandomGenerator {
  public static int RandomNumber(int max) {
    var random = new Random();
    return random.Next(max);
  }

  public static double RandomNumber(double min, double max) {
    if (min > max) throw new ArgumentException("min should be less than or equal to max");

    var random = new Random();
    var randomValue = random.NextDouble();
    return min + randomValue * (max - min);
  }
}