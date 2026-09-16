using UnityEngine;

/// <summary>
/// Contract for anything that launches the player upward (and optionally
/// sideways) on contact — trampolines, bounce pads, spring blocks, etc.
/// </summary>
public interface IBounceGiver
{
    float BounceForce { get; }
}
