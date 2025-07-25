// SPDX-FileCopyrightText: 2022 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Plays a sound when its non-hard fixture collides with a player.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FlyBySoundComponent : Component
{
    /// <summary>
    /// Probability that the sound plays
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("prob")]
    public float Prob = 0.10f;

    [ViewVariables(VVAccess.ReadWrite), DataField("sound")]
    [AutoNetworkedField]
    public SoundSpecifier Sound = new SoundCollectionSpecifier("BulletMiss")
    {
        Params = AudioParams.Default,
    };

    [DataField("range")]
    [AutoNetworkedField]
    public float Range = 1.5f;
}
