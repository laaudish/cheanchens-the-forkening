// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameStates;

namespace Content.Shared.Damage.Components;

/// <summary>
///     Applies the specified DamageModifierSets when the entity takes damage.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DamageProtectionBuffComponent : Component
{
    /// <summary>
    ///     The damage modifiers for entities with this component.
    /// </summary>
    [DataField]
    public Dictionary<string, DamageModifierSetPrototype> Modifiers = new();
}
