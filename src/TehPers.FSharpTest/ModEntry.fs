namespace TehPers.FSharpTest

open StardewModdingAPI
open StardewValley
open TehPers.Core.Api.DI
open TehPers.Core.Api.Gui
open TehPers.Core.Api.Items
open TehPers.FishingOverhaul.Api
open Ninject
open TehPers.FishingOverhaul.Api.Extensions

type internal MaybeBuilder() =
    static member Maybe = MaybeBuilder()

    member _.Bind(v: 'T1 option, f: 'T1 -> 'T2 option) = Option.bind f v
    member _.Return(value: 'T) = Some value
    member _.ReturnFrom(value: 'T option) = value
    member _.Delay(f: unit -> 'T) = f
    member _.Run(f: unit -> 'T option) = f ()

    member _.Combine(v: 'T1 option, f: unit -> 'T2 option) =
        match v with
        | Some _ -> f ()
        | None -> None

    member _.Zero() = Some()

type internal FishMenu(namespaceRegistry: INamespaceRegistry, fishingApi: IFishingApi) =
    inherit ManagedMenu(false)

    override _.CreateRoot() =
        let fishingInfo = new FishingInfo(Game1.player)

        let fish =
            fishingApi
                .GetFishChances(fishingInfo)
                .ToWeighted((fun entry -> entry.Weight), (fun entry -> entry.Value.FishKey))

        let trash =
            fishingApi
                .GetTrashChances(fishingInfo)
                .ToWeighted((fun entry -> entry.Weight), (fun entry -> entry.Value.ItemKey))

        let items = Seq.append fish trash
        BuildMenu.buildMenu namespaceRegistry items

type ModEntry() =
    inherit Mod()

    override this.Entry(helper) =
        MaybeBuilder.Maybe {
            // Register bindings
            let! factory = Option.ofObj ModServices.Factory
            let kernel = factory.GetKernel(this)

            kernel
                .Bind<FishMenu>()
                .ToSelf()
                .InSingletonScope()
            |> ignore

            // Open menu on button press
            helper.Events.Input.ButtonPressed.AddHandler (fun _ e ->
                if e.Button = SButton.K
                   && Game1.activeClickableMenu = null then
                    Game1.activeClickableMenu <- kernel.Get<FishMenu>())
        }
        |> ignore

        ()
