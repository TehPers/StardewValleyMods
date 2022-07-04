module internal BuildMenu

open Microsoft.Xna.Framework
open StardewValley
open TehPers.FishingOverhaul.Api.Extensions
open TehPers.Core.Api.Items
open TehPers.Core.Api.Extensions
open TehPers.Core.Api.Gui
open TehPers.Core.Api.Gui.Layouts
open TehPers.Core.Api.Extensions.Drawing

let tryToOption (success: bool, v: 'T) =
    match success with
    | true -> Some v
    | false -> None

let createItem (namespaceRegistry: INamespaceRegistry) (item: NamespacedKey) =
    item
    |> namespaceRegistry.TryGetItemFactory
    |> tryToOption
    |> Option.map (fun v -> v.Create())

let itemRenderer (size: float32) (item: Item) =
    new SimpleComponent(
        new GuiConstraints(MinSize = new GuiSize(size, size), MaxSize = new PartialGuiSize(size, size)),
        fun batch bounds ->
            item.DrawInMenuCorrected(
                batch,
                new Vector2(float32 bounds.X, float32 bounds.Y),
                size / 64f,
                1f,
                0.9f,
                StackDrawType.Hide,
                Color.White,
                false,
                new TopLeftDrawOrigin()
            )
    )

let itemView (namespaceRegistry: INamespaceRegistry) (itemKey: NamespacedKey) (weight: float) =
    itemKey
    |> createItem namespaceRegistry
    |> Option.map (fun item ->
        let itemBackground =
            new StretchedTexture(DrawUtils.WhitePixel, Color = new Color(Color.WhiteSmoke, 0.5f))

        let itemComponent =
            (itemRenderer 32f item)
                .Aligned(HorizontalAlignment.Center)

        let chanceComponent =
            (new Label(weight.ToString("P2"), Game1.smallFont))
                .Aligned(HorizontalAlignment.Center)

        VerticalLayout
            .Of(itemComponent, chanceComponent)
            .WithBackground(itemBackground)
            .WithPadding(8f))

let buildCatchableList namespaceRegistry entries =
    let blankSlots = Seq.initInfinite (fun _ -> new EmptySpace() :> IGuiComponent)
    let blankRows = Seq.initInfinite (fun _ -> blankSlots)

    entries
    |> WeightedExtensions.Normalize<NamespacedKey>
    |> Seq.sortByDescending (fun entry -> entry.Weight)
    |> Seq.map (fun entry -> itemView namespaceRegistry entry.Value entry.Weight)
    |> Seq.choose id
    |> Seq.map (fun c -> c.Constrained(maxSize = new PartialGuiSize(32f, 32f)) :> IGuiComponent)
    |> Seq.chunkBySize 4
    |> Seq.map Seq.ofArray
    |> (fun rows -> Seq.append rows blankRows)
    |> Seq.map (fun chunk -> Seq.append chunk blankSlots)
    |> Seq.map (Seq.truncate 4)
    |> Seq.map HorizontalLayout.Of
    |> Seq.truncate 4
    |> Seq.cast
    |> VerticalLayout.Of

let buildMenu namespaceRegistry entries =
    let fishList = buildCatchableList namespaceRegistry entries

    fishList
        .WithPadding(64f)
        .WithBackground(new MenuBackground())
