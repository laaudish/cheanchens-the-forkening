// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 chairbender <kwhipke1@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2022 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Commands;
using Content.Shared.Administration;
using Content.Shared.Alert;
using Robust.Shared.Console;

namespace Content.Server.Alert.Commands
{
    [AdminCommand(AdminFlags.Debug)]
    public sealed class ShowAlert : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;

        public string Command => "showalert";
        public string Description => "Shows an alert for a player, defaulting to current player";
        public string Help => "showalert <alertType> <severity, -1 if no severity> <name or userID, omit for current player>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player;
            if (player?.AttachedEntity == null)
            {
                shell.WriteLine("You cannot run this from the server or without an attached entity.");
                return;
            }

            var attachedEntity = player.AttachedEntity.Value;

            if (args.Length > 2)
            {
                var target = args[2];
                if (!CommandUtils.TryGetAttachedEntityByUsernameOrId(shell, target, player, out attachedEntity)) return;
            }

            if (!_e.TryGetComponent(attachedEntity, out AlertsComponent? alertsComponent))
            {
                shell.WriteLine("user has no alerts component");
                return;
            }

            var alertType = args[0];
            var severity = args[1];
            var alertsSystem = _e.System<AlertsSystem>();
            if (!alertsSystem.TryGet(alertType, out var alert))
            {
                shell.WriteLine("unrecognized alertType " + alertType);
                return;
            }
            if (!short.TryParse(severity, out var sevint))
            {
                shell.WriteLine("invalid severity " + sevint);
                return;
            }

            short? severity1 = sevint == -1 ? null : sevint;
            alertsSystem.ShowAlert(attachedEntity, alert.ID, severity1, null);
        }
    }
}
