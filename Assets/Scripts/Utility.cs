using UnityEngine;

public enum EDirection
{
    n, e, s, w, ne, nw, se, sw, NDEF
}

public static class Utility
{
    public static Vector2 DirectionToVector2(EDirection direction)
    {
        switch (direction)
        {
            case EDirection.n:
                return new Vector2(0, 1);
            case EDirection.e:
                return new Vector2(1, 0);
            case EDirection.s:
                return new Vector2(0, -1);
            case EDirection.w:
                return new Vector2(-1, 0);
            case EDirection.ne:
                return new Vector2(1, 1);
            case EDirection.nw:
                return new Vector2(-1, 1);
            case EDirection.se:
                return new Vector2(1, -1);
            case EDirection.sw:
                return new Vector2(-1, -1);
            default:
                return Vector2.zero;
        }        
    }

    public static EDirection Vector2ToDirection(Vector2 vector)
    {
        if (vector.x == 0)
        {
            if (vector.y > 0)
            {
                return EDirection.n;
            }
            if (vector.y < 0)
            {
                return EDirection.s;
            }
        }
        if (vector.x > 0)
        {
            if (vector.y == 0)
            {
                return EDirection.e;
            }
            if (vector.y > 0)
            {
                return EDirection.ne;
            }
            if (vector.y < 0)
            {
                return EDirection.se;
            }
        }

        if (vector.x < 0)
        {
            if (vector.y == 0)
            {
                return EDirection.w;
            }
            if (vector.y > 0)
            {
                return EDirection.nw;
            }
            if (vector.y < 0)
            {
                return EDirection.sw;
            }
        }

        return EDirection.NDEF;
    }

    public static Vector2 SetTo01(Vector2 vector)
    {
        vector.x = vector.x == 0 ? 0 : vector.x < 0 ? -1 : 1;
        vector.y = vector.y == 0 ? 0 : vector.y < 0 ? -1 : 1;
        return vector;
    }
}
