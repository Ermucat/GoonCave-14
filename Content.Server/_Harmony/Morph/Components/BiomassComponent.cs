namespace Content.Server._Harmony.Morph.Components;

[RegisterComponent]
public sealed partial class BiomassComponent : Component
{
    /// <summary>
    /// The amount of biomass a creature gives to a morph
    /// </summary>
    [DataField]
    public float Amount;
}
