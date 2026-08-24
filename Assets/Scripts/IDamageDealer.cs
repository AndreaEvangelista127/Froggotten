using UnityEngine;

public interface IDamageDealer
{
    /// <summary>
    /// The amount of damage this object deals on contact.
    /// </summary>
    float Damage { get; }
}
