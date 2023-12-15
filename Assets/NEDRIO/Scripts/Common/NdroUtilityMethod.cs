using UnityEngine;
using NDRO.Ruler;
public static class NdroUtilityMethod
{

    public static Vector3 AdjustScaleBasedOnDistance(float distance, DistanceScaleRange range)
    {
        // 거리를 minDistance ~ maxDistance 사이의 값으로 제한
        distance = Mathf.Clamp(distance, range.minDistance, range.maxDistance);

        // 거리에 따른 스케일 계산 (minDistance에서는 minScale, maxDistance에서는 maxScale)
        float scale = Mathf.Lerp(range.minScale, range.maxScale, (distance - range.minDistance) / (range.maxDistance - range.minDistance));

        // 스케일 적용
        return new Vector3(scale, scale, scale);
    }

    public static float AdjustValueBasedOnDistance(float distance, DistanceScaleRange range)
    {
        // 거리를 minDistance ~ maxDistance 사이의 값으로 제한
        distance = Mathf.Clamp(distance, range.minDistance, range.maxDistance);

        // 거리에 따른 스케일 계산 (minDistance에서는 minScale, maxDistance에서는 maxScale)
        float scale = Mathf.Lerp(range.minScale, range.maxScale, (distance - range.minDistance) / (range.maxDistance - range.minDistance));

        // 스케일 적용
        return scale;
    }

}
