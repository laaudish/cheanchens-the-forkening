// SPDX-FileCopyrightText: 2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events;

[NetSerializable, Serializable]
public sealed class OpenSlotStorageNetworkMessage : EntityEventArgs
{
    public readonly string Slot;

    public OpenSlotStorageNetworkMessage(string slot)
    {
        Slot = slot;
    }
}
