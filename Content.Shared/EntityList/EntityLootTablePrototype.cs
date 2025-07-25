// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Immutable;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.EntityList;

[Prototype]
public sealed partial class EntityLootTablePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("entries")]
    public ImmutableList<EntitySpawnEntry> Entries = ImmutableList<EntitySpawnEntry>.Empty;

    /// <inheritdoc cref="EntitySpawnCollection.GetSpawns"/>
    public List<string> GetSpawns(IRobustRandom random)
    {
        return EntitySpawnCollection.GetSpawns(Entries, random);
    }
}
