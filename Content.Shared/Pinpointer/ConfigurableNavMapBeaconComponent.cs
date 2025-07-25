// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer;

/// <summary>
/// This is used for a <see cref="NavMapBeaconComponent"/> that can be configured with a UI.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedNavMapSystem))]
public sealed partial class ConfigurableNavMapBeaconComponent : Component
{

}

[Serializable, NetSerializable]
public sealed class NavMapBeaconConfigureBuiMessage : BoundUserInterfaceMessage
{
    public string? Text;
    public bool Enabled;
    public Color Color;

    public NavMapBeaconConfigureBuiMessage(string? text, bool enabled, Color color)
    {
        Text = text;
        Enabled = enabled;
        Color = color;
    }
}

[Serializable, NetSerializable]
public enum NavMapBeaconUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public enum NavMapBeaconVisuals : byte
{
    Enabled
}
