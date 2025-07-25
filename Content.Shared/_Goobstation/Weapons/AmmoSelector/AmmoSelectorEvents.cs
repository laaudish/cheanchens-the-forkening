// SPDX-FileCopyrightText: 2024 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 John Space <bigdumb421@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Goobstation.Weapons.AmmoSelector;

[Serializable, NetSerializable]
public sealed class AmmoSelectedMessage(ProtoId<SelectableAmmoPrototype> protoId) : BoundUserInterfaceMessage
{
    public ProtoId<SelectableAmmoPrototype> ProtoId { get; } = protoId;
}

[Serializable, NetSerializable]
public enum AmmoSelectorUiKey : byte
{
    Key
}
