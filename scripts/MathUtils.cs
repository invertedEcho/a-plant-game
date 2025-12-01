using System;

public static class MathUtils {
    public static float GetRandomFloatRange(Random randomDistance, float minValue, float maxValue) {
        float range = maxValue - minValue;
        double randomDouble = randomDistance.NextDouble();
        double scaledDouble = (randomDouble * range) + minValue;
        return (float)scaledDouble;
    }
}
