// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Visuals;

/// <summary>
/// Used for the visualizer
/// </summary>
[Serializable, NetSerializable]
public enum PortableScrubberVisuals : byte
{
    IsFull,
    IsRunning,
    IsDraining,
}
