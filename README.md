# Bob The Builder

Welcome. I'm just doing this for fun because I really wanna build more and better stuff in the game, without "cheaty" mods
like force-remove. Because if there's a mod for brute-forcing something like that then players feel like things can be better.
And they can. That's what I'm addressing in this mod.

## Planned content of this mod

- New building resources:
  - Smooth stone for less chaotic stone-based builds.
  - Stripped logs for a more soothing texture.
  - Concrete for supporting larger structures, like bridges or halls.
  - Glass because it's glass.

- New items for more crafting recipes:
  - Cement Bags for concrete structures which can be found besides bunkers. Bags can be transported via log sled en masse.
  - Quartz Sand Bags that are just retextured cement bags for smelting glass.
  - A sawblade that can be printed for 500 resin that unlocks the circular saw.
  - A chisel that can be printed for 250 resin that unlocks the masonry table.
  - Rebar that can be used for supporting concrete structures.
  
- New structures:
  - A versatile log holder that can hold any type of log, be it halves, planks or stripped logs, one type at a time.
  - A processed stone holder that can hold 56 polished slabs.
  - Holders for glass, concrete/quartz sand bags and rebar
  - A circular saw that can be built for automatically processing wooden resources. Supports multiple modes, like cutting
    full logs into planks or half logs into quarters. Used to create stripped logs.
  - A masonry table that automatically processes rocks into polished slabs.
  - The smeltery that can process rocks and polished slabs automatically into rebar. Let's just say they're containing a lot of iron.
  - The glass smelter that processes quartz sand bags into glass automatically.
  
- Construction additions:
  - Smooth stone and strpped log analogs of their original resource.
  - Diagonal logs and walls for any resource.
  - Double doors.
  - Glass windows in any size, fences and stairs.
  - Concrete forms made from logs.
  - Rebar support for beam concrete forms.
  - Concrete walls, beams and windows.

### Notes on concrete

Concrete can only be supported by either itself (solid foundation on the ground) or stone.
A default concrete beam can span the distancee of 6 logs without support whereas a beam supported
with rebar can span 10 logs. Concrete can also be used to cantilever, without support by 2 logs and
with rebar support for 4 logs. Rebar can only be placed inside concrete forms for beams.

Concrete cannot be recovered, however it can be destroyed using an axe. For enemies, **concrete is
indestructible.**

### Notes on glass

Glass can be cut using an axe because the character is just that good. Glass can only be used as a wall or fence
and **breaks immediately** if an enemy lands a hit.

## API for other modders

This mod is written in a way so that other modders can use it as an API for registering their own resources and structures.
The primary access point is the singleton property `BobTheBuilder.BobTheBuilder.Instance`.

### Waiting for Bob to load

I'm not quite sure if RedLoader has a dependency system. If so, add `BobTheBuilder` as a dependency and use any API after
`OnSdkInitialized` where the initialization of most Bob classes is done.
Otherwise, Bob provides the static `AddInitializationListener` method that takes in an action as a parameter. When Bob
was initialized, any action provided is invoked with the Bob instance as its parameter.

### Adding new resources

Resources can be added via the `ResourceManager` found within `BobTheBuilder.Instance.ResourceManager`. The first thing
you want to do is creat a new `RegisteredResource` using `ResourceManager.CreateResourceItem(ItemData, string, bool = true)`.
This method accepts an `ItemData` instance from the game as its base. The new provided name and a new id are assigned to the
item and, if likely desired, the associated prefabs like the pickup or held prefabs are cloned. Use the item data for
rock or log provided by the resource manager if you want to bas your resource on them.

If the resource should be registered as a held-only type, provide the appropriate base type in `RegisteredResource.HeldOnlyType`.
If not further specified, Bob will use the held display positions of the base type. If custom positions for the item should be
specified, set an action on the `ApplyHeldPosition` property. This action is called for every instance of a held position that
was created and should apply position/rotation/scale to the transform.

When creating a held-only item, the resource needs to be properly configured. For rock-like pickups, this can be done by
calling `PickupManager.FixRockLikePickup(RegisteredResource, string)` and analogously for logs, `FixLogLikePickup`. These methods
take in the created resource instance and the path to an addressable asset containing a prefab for the item model to display in world.

The rest of the work is performed by Bob. You can spawn the resource into the world by simply calling
`PickupManager.SpawnResourcePickup(RegisteredResource, Vector3, Quaternion, bool)`. This method takes in the resource to spawn,
the in-world position, its initial rotation and whether or not to apply physics immediately to the object, which is likely false.

## Credits

`entropy.cgassets` for their [cement bag model](https://sketchfab.com/3d-models/cement-bag-7ad243de1c8c4a45823c4eb6109c7e96)
