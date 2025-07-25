# SPDX-FileCopyrightText: 2021 20kdc <asdd2808@gmail.com>
# SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
# SPDX-FileCopyrightText: 2021 Galactic Chimp <63882831+GalacticChimp@users.noreply.github.com>
# SPDX-FileCopyrightText: 2021 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
# SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
# SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
# SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 Plykiya <plykiya@protonmail.com>
# SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
# SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
# SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
#
# SPDX-License-Identifier: MIT

## UI

injector-draw-text = Draw
injector-inject-text = Inject
injector-invalid-injector-toggle-mode = Invalid
injector-volume-label = Volume: [color=white]{$currentVolume}/{$totalVolume}[/color]
    Mode: [color=white]{$modeString}[/color] ([color=white]{$transferVolume}u[/color])

## Entity

injector-component-drawing-text = Now drawing
injector-component-injecting-text = Now injecting
injector-component-cannot-transfer-message = You aren't able to transfer to {THE($target)}!
injector-component-cannot-draw-message = You aren't able to draw from {THE($target)}!
injector-component-cannot-inject-message = You aren't able to inject to {THE($target)}!
injector-component-inject-success-message = You inject {$amount}u into {THE($target)}!
injector-component-transfer-success-message = You transfer {$amount}u into {THE($target)}.
injector-component-draw-success-message = You draw {$amount}u from {THE($target)}.
injector-component-target-already-full-message = {CAPITALIZE(THE($target))} is already full!
injector-component-target-is-empty-message = {CAPITALIZE(THE($target))} is empty!
injector-component-cannot-toggle-draw-message = Too full to draw!
injector-component-cannot-toggle-inject-message = Nothing to inject!

## mob-inject doafter messages

injector-component-drawing-user = You start drawing the needle.
injector-component-injecting-user = You start injecting the needle.
injector-component-drawing-target = {CAPITALIZE(THE($user))} is trying to use a needle to draw from you!
injector-component-injecting-target = {CAPITALIZE(THE($user))} is trying to inject a needle into you!
