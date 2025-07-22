namespace Content.Server._Harmony.LightBreaker;


[RegisterComponent]
public sealed partial class LightBreakerComponent : Component
{
    [AutoNetworkedField]
    public float Radius = 10;
}
