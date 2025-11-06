using UnityEngine;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static void SendFunnelStepEvent(int stepNumber)
    {
        var customEvent = new CustomEvent("FUNNEL_EVENT");
        customEvent["Funnel_Step_Number"] = stepNumber;
        AnalyticsService.Instance.RecordEvent(customEvent);
        Debug.Log($"[Analytics] Event Sent: FUNNEL_EVENT Step={stepNumber}");
    }
}
