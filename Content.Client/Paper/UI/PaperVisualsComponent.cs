// SPDX-FileCopyrightText: 2022 Fishfish458 <47410468+Fishfish458@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 fishfish458 <fishfish458>
// SPDX-FileCopyrightText: 2023 /ʊniɹɑː/ <onoira@psiko.zone>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Eoin Mcloughlin <helloworld@eoinrul.es>
// SPDX-FileCopyrightText: 2023 eoineoineoin <eoin.mcloughlin+gh@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 jackel234 <52829582+jackel234@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 pathetic meowmeow <uhhadd@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Shared.Utility;

namespace Content.Client.Paper.UI;

[RegisterComponent]
public sealed partial class PaperVisualsComponent : Component
{
    /// <summary>
    ///     The path to the image which will be used as a background for the paper itself
    /// </summary>
    [DataField("backgroundImagePath")]
    public string? BackgroundImagePath;

    /// <summary>
    ///     An optional patch to configure tiling stretching of the background. Used to set
    ///     the PatchMargin in a <code>StyleBoxTexture</code>
    /// </summary>
    [DataField("backgroundPatchMargin")]
    public Box2 BackgroundPatchMargin = default;

    /// <summary>
    ///     Modulate the background image by this color. Can be used to add colorful
    ///     variants of images, without having to create new textures.
    /// </summary>
    [DataField("backgroundModulate")]
    public Color BackgroundModulate = Color.White;

    /// <summary>
    ///     Should the background image tile, or be streched? Sets <code>StyleBoxTexture.StrechMode</code>
    /// </summary>
    [DataField("backgroundImageTile")]
    public bool BackgroundImageTile = false;

    /// <summary>
    ///     An additional scale to apply to the background image
    /// </summary>
    [DataField("backgroundScale")]
    public Vector2 BackgroundScale = Vector2.One;

    /// <summary>
    ///     A path to an image which will be used as a header on the paper
    /// </summary>
    [DataField("headerImagePath")]
    public string? HeaderImagePath;

    /// <summary>
    ///     Modulate the header image by this color
    /// </summary>
    [DataField("headerImageModulate")]
    public Color HeaderImageModulate = Color.White;

    /// <summary>
    ///     Any additional margin to add around the header
    /// </summary>
    [DataField("headerMargin")]
    public Box2 HeaderMargin = default;

    /// <summary>
    /// A path to an image which will be used as a footer on the paper
    /// </summary>
    [DataField]
    public ResPath? FooterImagePath;

    /// <summary>
    /// Modulate the footer image by this color
    /// </summary>
    [DataField]
    public Color FooterImageModulate = Color.White;

    /// <summary>
    /// Any additional margin to add around the footer
    /// </summary>
    [DataField]
    public Box2 FooterMargin = default;

    /// <summary>
    ///     Path to an image to use as the background to the "content" of the paper
    ///     The header and actual written text will use this as a background. The
    ///     image will be tiled vertically with the property that the bottom of the
    ///     written text will line up with the bottom of this image.
    /// </summary>
    [DataField("contentImagePath")]
    public string? ContentImagePath;

    /// <summary>
    ///     Modulate the content image by this color
    /// </summary>
    [DataField("contentImageModulate")]
    public Color ContentImageModulate = Color.White;

    /// <summary>
    ///     An additional margin around the content (including header)
    /// </summary>
    [DataField("contentMargin")]
    public Box2 ContentMargin = default;

    /// <summary>
    ///     The number of lines that the content image represents. The
    ///     content image will be vertically tiled after this many lines
    ///     of text.
    /// </summary>
    [DataField("contentImageNumLines")]
    public int ContentImageNumLines = 1;

    /// <summary>
    ///     Modulate the style's font by this color
    /// </summary>
    [DataField("fontAccentColor")]
    public Color FontAccentColor = new Color(223, 223, 213);

    /// <summary>
    ///     This can enforce that your paper has a limited area to write in.
    ///     If you wish to constrain only one direction, the other direction
    ///     can be unlimited by specifying a value of zero.
    ///     This will be scaled according to UI scale.
    /// </summary>
    [DataField("maxWritableArea")]
    public Vector2? MaxWritableArea = null;
}
