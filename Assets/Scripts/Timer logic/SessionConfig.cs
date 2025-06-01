using UnityEngine;

public static class SessionConfig
{
    public const int k_workIntervalMinutes = 25;
    public const int k_breakIntervalMinutes = 5;
    public const int k_cycleDurationMinutes = k_workIntervalMinutes + k_breakIntervalMinutes; // 30 minutes per cycle
}
