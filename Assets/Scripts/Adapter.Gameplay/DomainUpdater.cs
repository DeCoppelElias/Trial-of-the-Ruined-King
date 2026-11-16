using Domain.Ports.Driving;
using UnityEngine;

// Feeds the game clock real time and copies its speed into Time.timeScale, so Unity's
// scaled time follows game time. Runs before other scripts so they see this frame's state.
[DefaultExecutionOrder(-100)]
public class DomainUpdater : MonoBehaviour
{
    private ITimeService _timeService;

    public void Initialize(ITimeService timeService)
    {
        _timeService = timeService;
    }

    private void Update()
    {
        if (_timeService == null) return;

        _timeService.Advance(Time.unscaledDeltaTime);
        Time.timeScale = _timeService.Speed;
    }

    // Time.timeScale outlives the scene, so a run left at speed 0 would freeze whatever loads next.
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
