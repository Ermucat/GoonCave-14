using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Silicons.StationAi;
using Content.Shared.StationAi;
using Content.Shared.Wires;

namespace Content.Server.Silicons.StationAi;

/// <summary>
/// Handles StationAiVision functionality for the attached entity.
/// </summary>
public sealed partial class AiVisionWireAction : ComponentWireAction<StationAiVisionComponent>
{
    public override string Name { get; set; } = "wire-name-ai-vision-light";
    public override Color Color { get; set; } = Color.White;
    public override object StatusKey => AirlockWireStatus.AiVisionIndicator;

    // Harmony Start
    [DataField("timeout")]
    private int _timeout = 10;
    // Harmony End

    public override StatusLightState? GetLightState(Wire wire, StationAiVisionComponent component)
    {
        return component.Enabled ? StatusLightState.On : StatusLightState.Off;
    }

    public override bool Cut(EntityUid user, Wire wire, StationAiVisionComponent component)
    {
        return EntityManager.System<SharedStationAiSystem>()
            .SetVisionEnabled((wire.Owner, component), false, announce: true);
    }

    public override bool Mend(EntityUid user, Wire wire, StationAiVisionComponent component)
    {
        return EntityManager.System<SharedStationAiSystem>()
            .SetVisionEnabled((wire.Owner, component), true);
    }

    public override void Pulse(EntityUid user, Wire wire, StationAiVisionComponent component)
    {
        // TODO: This should turn it off for a bit
        // Need timer cleanup first out of scope.
        // Harmony Start
        EntityManager.System<SharedStationAiSystem>().SetVisionEnabled((wire.Owner, component), false);
        WiresSystem.StartWireAction(wire.Owner, _timeout, PulseTimeoutKey.Key, new TimedWireEvent(AwaitPulseEnd, wire));
        // Harmony End
    }

    // Harmony Start

    private void AwaitPulseEnd(Wire wire)
    {
        if (!wire.IsCut)
        {
            if (EntityManager.TryGetComponent<StationAiVisionComponent>(wire.Owner, out var stationAiVision))
                EntityManager.System<SharedStationAiSystem>().SetVisionEnabled((wire.Owner, stationAiVision), true);
        }
    }

    private enum PulseTimeoutKey : byte
    {
        Key
    }
    // Harmony End
}
