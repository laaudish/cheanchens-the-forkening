// SPDX-FileCopyrightText: 2023 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Shared.DeviceLinking.Events;

public sealed class LinkAttemptEvent : CancellableEntityEventArgs
{
    public readonly EntityUid Source;
    public readonly EntityUid Sink;
    public readonly EntityUid? User;
    public readonly string SourcePort;
    public readonly string SinkPort;

    public LinkAttemptEvent(EntityUid? user, EntityUid source, string sourcePort, EntityUid sink, string sinkPort)
    {
        User = user;
        Source = source;
        SourcePort = sourcePort;
        Sink = sink;
        SinkPort = sinkPort;
    }
}
