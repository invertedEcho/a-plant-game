using System;
using Godot;
using Godot.Collections;

namespace agame.utils;

public static class Utils {
    public static Vector3? SnapToGround(Vector3 position, PhysicsDirectSpaceState3D space, Array<Rid> exclude) {
        Vector3 start = position + Vector3.Up * 200.0f;
        Vector3 end = position + Vector3.Down * 200.0f;

        PhysicsRayQueryParameters3D queryParameters = new() {
            From = start,
            To = end,
            Exclude = exclude
        };

        var rayCastResult = space.IntersectRay(queryParameters);

        if (rayCastResult.TryGetValue("position", out Variant position2)) {
            return (Vector3)position2;
        }
        return null;
    }

    public static float GetRandomFloatRange(Random randomDistance, float minValue, float maxValue) {
        float range = maxValue - minValue;
        double randomDouble = randomDistance.NextDouble();
        double scaledDouble = (randomDouble * range) + minValue;
        return (float)scaledDouble;
    }
}
