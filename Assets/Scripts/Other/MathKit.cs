using UnityEngine;
using System.Collections;

public class MathKit : MonoBehaviour
{
    public static bool NormalizeRange(ref int min, ref int max)
    {
        if (min > max)
        {
            int temp = min;
            min = max;
            max = temp;
            return false;
        }

        return true;
    }


    public static int EnsureRange(int val, int min, int max)
    {
        NormalizeRange(ref min, ref max);

        if (val < min)
            val = min;
        else if (val > max)
            val = max;

        return val;
    }


    public static Vector2 EnsureVectorRectRange(Vector2 val, Rect r)
    {
        val.x = EnsureRange(val.x, r.xMin, r.xMax);
        val.y = EnsureRange(val.y, r.yMin, r.yMax);
        return val;
    }

    public static Vector2Int EnsureVectorRectRange(Vector2Int val, RectInt r)
    {
        val.x = EnsureRange(val.x, r.xMin, r.xMax);
        val.y = EnsureRange(val.y, r.yMin, r.yMax);
        return val;
    }

    public static bool NormalizeRange(ref float min, ref float max)
    {
        if (min > max)
        {
            float temp = min;
            min = max;
            max = temp;
            return false;
        }

        return true;
    }

    public static float EnsureRange(float val, float min, float max)
    {
        NormalizeRange(ref min, ref max);

        if (val < min)
            val = min;
        else if (val > max)
            val = max;

        return val;
    }

    public static float EnsureRange(float val, Vector2 range)
    {
        NormalizeRange(ref range.x, ref range.y);

        if (val < range.x)
            val = range.x;
        else if (val > range.y)
            val = range.y;

        return val;
    }

    public static bool RectIntContainsRect(RectInt who, RectInt check)
    {
        return (check.xMin >= who.xMin) &&
               (check.yMin >= who.yMin) &&
               (check.xMax <= who.xMax) &&
               (check.yMax <= who.yMax);
    }

    public static bool NumbersEquals(float n1, float n2, float eps)
    {
        return Mathf.Abs(n1 - n2) <= eps;
    }

    public static bool NumbersEquals(float n1, float n2)
    {
        return NumbersEquals(n1, n2, float.Epsilon);
    }

    public static bool NumbersEquals(double n1, double n2, double eps)
    {
        return System.Math.Abs(n1 - n2) <= eps;
    }

    public static bool NumbersEquals(double n1, double n2)
    {
        return NumbersEquals(n1, n2, double.Epsilon);
    }

    public static bool Vectors2DEquals(Vector2 vec1, Vector2 vec2, float eps)
    {
        return NumbersEquals(vec1.x, vec2.x, eps) && NumbersEquals(vec1.y, vec2.y, eps);
    }

    public static bool Vectors2DEquals(Vector2 vec1, Vector2 vec2)
    {
        return Vectors2DEquals(vec1, vec2, Vector2.kEpsilon);
    }

    public static bool Vectors2DIntEquals(Vector2Int vec1, Vector2Int vec2)
    {
        return (vec1.x == vec2.x) && (vec1.y == vec2.y);
    }

    public static bool Vectors3DEquals(Vector3 vec1, Vector3 vec2)
    {
        return NumbersEquals(vec1.x, vec2.x, Vector3.kEpsilon) &&
               NumbersEquals(vec1.y, vec2.y, Vector3.kEpsilon) &&
               NumbersEquals(vec1.z, vec2.z, Vector3.kEpsilon);
    }

    public static float Vectors2IntCos(Vector2Int vec1, Vector2Int vec2)
    {
        Vector2Int delta = vec2 - vec1;
        float scalar = vec1.x * vec2.x + vec1.y * vec2.y;
        float magn = vec1.magnitude * vec2.magnitude;

        if (NumbersEquals(magn, 0))
            return 1f;

        return scalar / magn;
    }

    //return new Vector2(vec.x * Mathf.Cos(angle) - vec.y * Mathf.Sin(angle),
    //                 vec.x * Mathf.Sin(angle) + vec.y * Mathf.Cos(angle));

    public static bool Vectors2IntIsParallel(Vector2Int vec1, Vector2Int vec2)
    {
        // 3 0
        // 3 1

        // delta 0 -1

        // 1 0
        // 0 1

        // delta 1 -1

        // 1 0
        // 1 1

        // delta 0 -1

        // 0 1
        // 1 1

        // delta -1 0

        Vector2Int delta = vec1 - vec2;

        return (delta.x == 0) || (delta.y == 0);
    }

    public static bool Vectors2IntIsOrtho(Vector2Int vec1, Vector2Int vec2)
    {
        return NumbersEquals(Vectors2IntCos(vec1, vec2), 0f);
    }

    public static Vector2 RotateVector2D(Vector2 vec, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        return new Vector2(vec.x * Mathf.Cos(angle) - vec.y * Mathf.Sin(angle),
                           vec.x * Mathf.Sin(angle) + vec.y * Mathf.Cos(angle));
    }

    public static int Vector2IntCompare(Vector2Int vec1, Vector2Int vec2)
    {
        Vector2Int delta = vec1 - vec2;

        if ((delta.x < 0) || (delta.y < 0))
            return -1;
        else if ((delta.x == 0) && (delta.y == 0))
            return 0;
        else
            return 1;
    }

    public static bool Vector2InRelativeAngleSector(Vector2 pivot, Vector2 target, float angle_sector)
    {
        float angle = Vector2.Angle(pivot, target);

        return angle <= angle_sector;
    }

    public static bool intInRange(int value, int min, int max)
    {
        if (min > max)
        {
            int tmp = min;
            min = max;
            max = min;
        }

        return (value >= min) && (value <= max);
    }


    public static bool InVector2Int(int value, Vector2Int vector)
    {
        return intInRange(value, vector.x, vector.y);
    }

    public static Vector2 RotatePointAroundPoint(Vector2 pivot, Vector2 point, Vector2 direction)
    {
        return pivot + new Vector2(point.x * direction.x - point.y * direction.y, point.x * direction.y + point.y * direction.x);
    }

    public static Vector2Int RotatePointAroundPoint(Vector2Int pivot, Vector2Int point, Vector2 direction)
    {
        return (Vector2Int.RoundToInt(RotatePointAroundPoint((Vector2)pivot, point, direction)));
    }
}
