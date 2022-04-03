namespace Game.Utils
{
    public static class Angle
    {
        public static float Normalize(float angle)
        {
            angle %= 360;
            if (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }
    }
}