// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 Whisper <121047731+QuietlyWhisper@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Part; // Shitmed Change
using Content.Shared.Inventory;
using Content.Shared.Popups;
using JetBrains.Annotations;
using Robust.Server.GameObjects;

namespace Content.Server.Destructible.Thresholds.Behaviors;

[UsedImplicitly]
[DataDefinition]
public sealed partial class BurnBodyBehavior : IThresholdBehavior
{

    public void Execute(EntityUid bodyId, DestructibleSystem system, EntityUid? cause = null)
    {
        var transformSystem = system.EntityManager.System<TransformSystem>();
        var inventorySystem = system.EntityManager.System<InventorySystem>();
        var sharedPopupSystem = system.EntityManager.System<SharedPopupSystem>();

        if (system.EntityManager.TryGetComponent<InventoryComponent>(bodyId, out var comp))
        {
            foreach (var item in inventorySystem.GetHandOrInventoryEntities(bodyId))
            {
                transformSystem.DropNextTo(item, bodyId);
            }
        }

        if (system.EntityManager.TryGetComponent<BodyPartComponent>(bodyId, out var bodyPart))
        {
            if (bodyPart.CanSever
                && system.BodySystem.BurnPart(bodyId, bodyPart))
                sharedPopupSystem.PopupCoordinates(Loc.GetString("bodyburn-text-others", ("name", bodyId)), transformSystem.GetMoverCoordinates(bodyId), PopupType.LargeCaution);
        }
        else
        {
            sharedPopupSystem.PopupCoordinates(Loc.GetString("bodyburn-text-others", ("name", bodyId)), transformSystem.GetMoverCoordinates(bodyId), PopupType.LargeCaution);
            system.EntityManager.QueueDeleteEntity(bodyId);
        }
    }
}
