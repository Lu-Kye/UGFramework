using System;

namespace UGFramework.Utility
{
    public static class TimeUtility
    {
        public static int Timestamp 
        {
            get
            {
                return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }
    }
}