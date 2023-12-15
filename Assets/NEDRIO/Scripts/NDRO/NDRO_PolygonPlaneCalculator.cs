using System.Collections.Generic;
using UnityEngine;
using NDRO.Ruler;

public static class NDRO_PolygonPlaneCalculator
{



    public static List<Vector3> GetVectorsByNDRO_RulerPoints(List<NDRO_RulerPoints> rulerPoints)
    {
        List<Vector3> vectors = new List<Vector3>();
        for (int i = 0; i < rulerPoints.Count; i++)
        {
            vectors.Add(rulerPoints[i].pointA.position);
        }
        return vectors;
    }
    /// <summary>
    /// 평면의 너비와,높이, 평면의 종류 ( XY, YZ, XZ )를 반환.
    /// </summary>
    public static (float width, float height, string plane) CalculateDimensions(List<Vector3> points)
    {
        if (points.Count < 3)
        {
            return (0f, 0f, "None");
        }

        var variance = CalculateVariance(points);
        string plane = DeterminePlane(variance);
        var (width, height) = CalculatePlaneDimensions(points, plane);

        // 너비와 높이를 센티미터 단위로 변환
        width *= 100f;
        height *= 100f;

        return (width, height, plane);
    }

    private static (float xVariance, float yVariance, float zVariance) CalculateVariance(List<Vector3> points)
    {
        Vector3 mean = Vector3.zero;
        foreach (Vector3 point in points)
        {
            mean += point;
        }
        mean /= points.Count;

        Vector3 variance = Vector3.zero;
        foreach (Vector3 point in points)
        {
            variance += new Vector3(
                Mathf.Pow(point.x - mean.x, 2),
                Mathf.Pow(point.y - mean.y, 2),
                Mathf.Pow(point.z - mean.z, 2)
            );
        }
        variance /= points.Count;

        return (variance.x, variance.y, variance.z);
    }

    private static string DeterminePlane((float x, float y, float z) variance)
    {
        if (variance.x < variance.y && variance.x < variance.z)
            return "YZ";
        else if (variance.y < variance.x && variance.y < variance.z)
            return "XZ";
        else
            return "XY";
    }

    private static (float width, float height) CalculatePlaneDimensions(List<Vector3> points, string plane)
    {
        float minA = float.MaxValue, maxA = float.MinValue;
        float minB = float.MaxValue, maxB = float.MinValue;

        foreach (Vector3 point in points)
        {
            float a = 0, b = 0;
            switch (plane)
            {
                case "XY":
                    a = point.x; b = point.y;
                    break;
                case "YZ":
                    a = point.y; b = point.z;
                    break;
                case "XZ":
                    a = point.x; b = point.z;
                    break;
            }

            if (a < minA) minA = a;
            if (a > maxA) maxA = a;
            if (b < minB) minB = b;
            if (b > maxB) maxB = b;
        }

        return (maxA - minA, maxB - minB);
    }

    /// <summary>
    /// 둘레 계산
    /// </summary>
    public static float CalculatePerimeter(List<Vector3> points)
    {
        float perimeter = 0f;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % points.Count]; // 순환을 위해
            perimeter += Vector3.Distance(current, next);
        }
        return perimeter;
    }

    /// <summary>
    /// 넓이 계산
    /// </summary>
    public static float CalculateArea(List<Vector3> points, string plane)
    {
        float area = 0f;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % points.Count];

            switch (plane)
            {
                case "XY":
                    area += current.x * next.y - current.y * next.x;
                    break;
                case "YZ":
                    area += current.y * next.z - current.z * next.y;
                    break;
                case "XZ":
                    area += current.x * next.z - current.z * next.x;
                    break;
            }
        }
        return Mathf.Abs(area) / 2f;
    }
}
