// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Wieldable;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Modifies the speed when an entity with this component is wielded.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedWieldableSystem)), AutoGenerateComponentState]
public sealed partial class SpeedModifiedOnWieldComponent : Component
{
    /// <summary>
    /// How much the wielder's sprint speed is modified when the component owner is wielded.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float SprintModifier = 1f;

    /// <summary>
    /// How much the wielder's walk speed is modified when the component owner is wielded.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float WalkModifier = 1f;
}
