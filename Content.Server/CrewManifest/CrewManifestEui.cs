// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.EUI;
using Content.Shared.CrewManifest;
using Content.Shared.Eui;

namespace Content.Server.CrewManifest;

public sealed class CrewManifestEui : BaseEui
{
    private readonly CrewManifestSystem _crewManifest;

    /// <summary>
    ///     Station this EUI instance is currently tracking.
    /// </summary>
    private readonly EntityUid _station;

    /// <summary>
    ///     Current owner of this UI, if it has one. This is
    ///     to ensure that if a BUI is closed, the EUIs related
    ///     to the BUI are closed as well.
    /// </summary>
    public readonly EntityUid? Owner;

    public CrewManifestEui(EntityUid station, EntityUid? owner, CrewManifestSystem crewManifestSystem)
    {
        _station = station;
        Owner = owner;
        _crewManifest = crewManifestSystem;
    }

    public override CrewManifestEuiState GetNewState()
    {
        var (name, entries) = _crewManifest.GetCrewManifest(_station);
        return new(name, entries);
    }

    public override void Closed()
    {
        base.Closed();

        _crewManifest.CloseEui(_station, Player, Owner);
    }
}
