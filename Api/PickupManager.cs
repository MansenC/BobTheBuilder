using Sons.Ai.Vail.Inventory;
using Sons.Ai.Vail.StimuliTypes;
using Sons.Gameplay;
using Sons.Items.Core;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine.AddressableAssets;
using UnityEngine;
using ControllerItemData = TheForest.Items.Special.HeldOnlyItemController.ControllerItemData;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using RedLoader;
using Sons.Animation;
using System.Text;

namespace BobTheBuilder.Api
{
    /// <summary>
    ///     The pickup manager is a class managed by the resource manager.
    ///     Its purpose is the management of held-only resources and their registration.
    /// </summary>
    public class PickupManager
    {
        /// <summary>
        ///     The renderable object within pickup prefabs.
        /// </summary>
        private const string Renderable = "Renderable";

        /// <summary>
        ///     The layer mask for the held layer.
        /// </summary>
        private static int HeldLayer = LayerMask.NameToLayer("Held");

        /// <summary>
        ///     The associated resource manager.
        /// </summary>
        private readonly ResourceManager _resourceManager;

        /// <summary>
        ///     Constructs a new pickup manager instance.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        internal PickupManager(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        /// <summary>
        ///     Registers all new resource datas within the HeldOnlyItemController
        ///     so that they can be picked up.
        /// </summary>
        internal void RegisterPickUpsInHeldItemController()
        {
            // We get and "cast" the interface to the controller instance.
            IHeldOnlyItemController controllerInterface
                = LocalPlayer.Inventory.HeldOnlyItemController;

            // We need the precise instance since we need to modify private fields.
            HeldOnlyItemController itemController
                = new HeldOnlyItemController(controllerInterface.Pointer);

            // We store all new datas in this list so we can update the
            // heldOnlyItems reference array in one operation.
            List<ControllerItemData> newDatas = new();

            var heldOnlyItems = itemController._heldOnlyItems;
            foreach (RegisteredResource resource in _resourceManager.Resources.Values)
            {
                switch (resource.HeldOnlyType)
                {
                    case RegisteredResource.HeldOnlyBaseType.Rock:
                        ControllerItemData newData = CopyDataFor(
                            GetItemData(heldOnlyItems, ResourceManager.RockItemId),
                            resource);

                        newDatas.Add(newData);
                        break;
                    case RegisteredResource.HeldOnlyBaseType.None:
                        continue;
                    default:
                        // TODO other types.
                        RLog.Debug("Unsupported HeldOnlyType!!!!!!!!!!!!!!!!!!!!!");
                        continue;
                }
            }

            if (newDatas.Count == 0)
            {
                // Nothing to register.
                return;
            }

            RLog.Debug($"Registering {newDatas.Count} new resources.");

            int existignItems = heldOnlyItems.Length;
            Il2CppReferenceArray<ControllerItemData> allItems = new(existignItems + newDatas.Count);

            for (int index = 0; index < existignItems; index++)
            {
                allItems[index] = heldOnlyItems[index];
            }

            for (int index = 0; index < newDatas.Count; index++)
            {
                allItems[index + existignItems] = newDatas[index];
            }

            itemController._heldOnlyItems = allItems;
        }

        /// <summary>
        ///     Spawns a given resource as its pickup.
        /// </summary>
        /// <param name="resource">The resource to spawn.</param>
        /// <param name="location">The location to spawn at.</param>
        /// <param name="rotation">The rotation of the resource.</param>
        /// <param name="dynamic">
        ///     Whether or not the pickup is spawned as dynamic.
        ///     Dyamic pick ups have physics applied directly after spawning in.
        /// </param>
        /// <returns>The created instance.</returns>
        public static GameObject SpawnResourcePickup(
            RegisteredResource resource,
            Vector3 location,
            Quaternion rotation,
            bool dynamic)
        {
            GameObject prefab = resource.ItemData.PickupPrefab.gameObject;
            return prefab.GetComponent<PickUp>()
                .SpawnPickupPrefab(location, rotation, prefab, dynamic);
        }

        /// <summary>
        ///     Fixes a rock-like prefab instance. Updates all references to the item data and id,
        ///     removes rock-specific components and replaces the renderable instance.
        /// </summary>
        /// <param name="resource">The resource to fix its pickup prefab for.</param>
        /// <exception cref="InvalidOperationException">If the prefab is not registered.</exception>
        public static void FixRockLikePickup(
            RegisteredResource resource,
            string assetPath)
        {
            Transform pickupPrefab = resource.ItemData._pickupPrefab;
            if (pickupPrefab == null)
            {
                return;
            }
            else if (resource.PickupBoltId == -1)
            {
                throw new InvalidOperationException(
                    "Cannot fix prefab if not registered in prefab database!");
            }

            AssetReference reference = new AssetReference(assetPath);
            if (reference == null)
            {
                throw new InvalidOperationException($"Cannot find asset for path {assetPath}!");
            }

            // We store the reference within the resource.
            resource.RenderableAsset = reference;

            // Fix up itm id's and instance for the pickup.
            PickUp pickup = pickupPrefab.GetComponent<PickUp>();
            pickup._itemDataCached = resource.ItemData;
            pickup._itemId = resource.ItemId;

            // Fix item data on the object physics.
            var sfx = pickupPrefab.GetComponent<ObjectPhysicsInteractionSfx>();
            sfx._itemData = resource.ItemData;
            sfx._itemId = resource.ItemId;
            sfx.enabled = true;

            // Fix the multiplayer bolt entity prefab references.
            BoltEntity boltEntity = pickupPrefab.GetComponent<BoltEntity>();
            boltEntity._prefabId = resource.PickupBoltId;
            boltEntity._serializerGuid = null; // TODO?

            // Remove the stone stimuli.
            UnityEngine.Object.Destroy(pickupPrefab.GetComponent<StonePickupStimuli>());

            // Fix the item type name for the vail pickup.
            pickupPrefab.GetComponent<VailPickup>()._itemType = resource.ItemData.Name;

            // Fix the item renderable instance.
            ItemRenderable renderable = pickupPrefab.FindChild(Renderable)
                .GetComponent<ItemRenderable>();

            renderable._itemId = resource.ItemId;
            renderable._baseRenderable = reference;
        }

        /// <summary>
        ///     Creates a copy of the given controller item data for the provided registered
        ///     resource instance. The copy will have the id and item data of the resource.
        ///     If the resource is a held-only type, creates holder references within the player
        ///     for the carry system to display and sets up the animator so that everything is
        ///     valid.
        /// </summary>
        /// <param name="baseData">The data to copy.</param>
        /// <param name="resource">The resource the copy will belong to.</param>
        /// <returns>A new controller data instance.</returns>
        private static ControllerItemData CopyDataFor(
            ControllerItemData baseData,
            RegisteredResource resource)
        {
            ControllerItemData newData = new ControllerItemData
            {
                _itemId = resource.ItemId,
                _itemCache = resource.ItemData,
                _itemLength = baseData._itemLength,
                _itemMaxLength = baseData._itemMaxLength,
                _itemThickness = baseData._itemThickness,
                _dropSpawnDelay = baseData._dropSpawnDelay,
                _useDropOffset = baseData._useDropOffset,
                _dropPositionOffset = baseData._dropPositionOffset,
                _dropRotationOffset = baseData._dropRotationOffset,
                _animType = baseData._animType,
            };

            var baseHeld = baseData._held;
            if (baseHeld == null || baseHeld.Length == 0)
            {
                // We cannot associate held positions.
                return newData;
            }

            // Create and update the held item points.
            var heldObjects = CreateHeldItemPoints(baseHeld, resource);
            newData._held = heldObjects;

            // Finally we can return.
            return newData;
        }

        /// <summary>
        ///     Creates the held item reference points within the player based on the provided
        ///     base positions and the registered resource instance.
        ///     The held item positions are stored under the base held parent for the reference
        ///     resource. This means that for every rock-like resource, the rock parent will be
        ///     used. This is due to a limitation of my abilities to modify this behavior thanks
        ///     to the object being enabled within an animation clip. I really tried.
        /// </summary>
        /// <param name="baseHeld">The base held positions.</param>
        /// <param name="resource">The resource to create the positions for.</param>
        /// <returns>An array of game object instances representing the held positions.</returns>
        /// <exception cref="InvalidOperationException">If something is misconfigured.</exception>
        private static Il2CppReferenceArray<GameObject> CreateHeldItemPoints(
            Il2CppReferenceArray<GameObject> baseHeld,
            RegisteredResource resource)
        {
            // This is the root object under which the held arrays are stored.
            Transform heldRoot = baseHeld[0].transform.parent;

            // The held objects are an array of game objects that are rooted within the player.
            // It's a list of empty objects used as parent transforms for carried items.
            ItemData itemData = resource.ItemData;
            string itemName = itemData.Name;

            // The object we're currently parenting to.
            Transform currentParent = heldRoot;

            // The provider instance for setting the positions of the held items.
            Action<Transform, int> positionProvider = resource.ApplyHeldPosition;
            if (positionProvider == null && baseHeld.Length < itemData._maxAmount)
            {
                // We cannot reference the base positions A custom provider is required.
                throw new InvalidOperationException(
                    $"Max items ({itemData._maxAmount}) of item {itemName}" +
                    $" exceeds reference positions ({baseHeld.Length})!");
            }

            // We load the renderable prefab.
            GameObject renderablePrefab
                = Addressables.LoadAssetAsync<GameObject>(resource.RenderableAsset)
                .WaitForCompletion();

            if (renderablePrefab == null)
            {
                throw new InvalidOperationException(
                    $"Cannot load renderable prefab for resource {itemName}!");
            }

            // We attempt to find the appropriate mesh within the prefab.
            MeshRenderer heldItemRenderer = FindHeldItemMesh(renderablePrefab);
            if (heldItemRenderer == null)
            {
                throw new InvalidOperationException(
                    $"Cannot determine held item mesh for {itemName}!");
            }

            MeshFilter heldItemMesh = heldItemRenderer.GetComponent<MeshFilter>();

            Il2CppReferenceArray<GameObject> heldObjects = new(itemData._maxAmount);
            for (int index = 0; index < itemData._maxAmount; index++)
            {
                if (index == 1)
                {
                    // If we have at least two instances then we should instantiate
                    // an "additional visual override" instance because sons does.
                    Transform lastInstance = heldObjects[0].transform;

                    GameObject additionalOverride = new($"Addtonal{itemName}VisOverride");
                    additionalOverride.SetActive(true);
                    additionalOverride.layer = HeldLayer;

                    currentParent = additionalOverride.transform;
                    currentParent.SetParent(lastInstance, false);

                    if (positionProvider == null)
                    {
                        // We copy the positioning of the base data.
                        Transform baseAdditionalOverride = baseHeld[1].transform.parent;
                        currentParent.localPosition = baseAdditionalOverride.localPosition;
                        currentParent.localRotation = baseAdditionalOverride.localRotation;
                        currentParent.localScale = baseAdditionalOverride.localScale;
                    }
                    else
                    {
                        // We reverse the positioning so that the position provider can be absolute.
                        currentParent.localPosition = Vector3.zero;
                        if (lastInstance.transform.localScale != Vector3.zero)
                        {
                            // Handle this senseless case.
                            currentParent.localScale = new Vector3(
                                1f / lastInstance.transform.localScale.x,
                                1f / lastInstance.transform.localScale.y,
                                1f / lastInstance.transform.localScale.z);
                        }
                    }
                }

                // We create the object.
                GameObject heldObject = new GameObject($"{itemName}Held{index + 1}");
                heldObject.SetActive(false);
                heldObject.layer = HeldLayer;

                // We attach a renderer, its materials and a mesh filter for the held object.
                heldObject.AddComponent<MeshFilter>().mesh = heldItemMesh.mesh;
                heldObject.AddComponent<MeshRenderer>().materials = heldItemRenderer.materials;

                Transform targetTransform = heldObject.transform;
                targetTransform.SetParent(currentParent, false);

                // We store the new object instance.
                heldObjects[index] = heldObject;
                if (positionProvider != null)
                {
                    // We can apply the position provider.
                    positionProvider(targetTransform, index);
                    continue;
                }

                // Since we have no position provider we must apply the default positions.
                Transform referenceObject = baseHeld[index].transform;

                targetTransform.localPosition = referenceObject.localPosition;
                targetTransform.localRotation = referenceObject.localRotation;
                targetTransform.localScale = referenceObject.localScale;
            }

            return heldObjects;
        }

        /// <summary>
        ///     Creates the IkHeldRenderer instance for the held objects, registers it and
        ///     creates a reference within the player animator so that the proper animation plays.
        /// </summary>
        /// <param name="itemData">The item data to set up animations for.</param>
        /// <param name="heldRoot">The held root object instance.</param>
        /// <param name="firstBaseHeldObject">The first base held object.</param>
        /// <param name="firstHeldObject">The first new held object.</param>
        /// <exception cref="InvalidOperationException">If there's no IkHeldRenderer.</exception>
        private static void SetupIkAnimationData(
            ItemData itemData,
            Transform heldRoot,
            GameObject firstBaseHeldObject,
            GameObject firstHeldObject)
        {
            IkHeldRenderer originalIk = firstBaseHeldObject.GetComponent<IkHeldRenderer>();
            if (originalIk == null)
            {
                throw new InvalidOperationException("Base data must have IkHeldRenderer instance!");
            }

            // The first object gets associated with the "IkHeldRenderer" instance.
            IkHeldRenderer ikHeld = firstHeldObject.AddComponent<IkHeldRenderer>();
            ikHeld._limb = originalIk._limb;

            // We need to register the IK instance with the IK controller. We use the
            // IkTargetsControllerRef attached to the root's parent object to do that.
            IkTargetsController controller
                = heldRoot.parent.GetComponent<IkTargetsControllerRef>().Ref;

            // We "cast" down to the interface version here.
            controller.Register(new IIkHeldRenderer(ikHeld.Pointer));

            // Finally we need to fix the animator setup.
            Animator animator = LocalPlayer.Animator;

            int idHash;
            float floatHash;
            if (ikHeld._limb == IkLimbs.RightHand)
            {
                idHash = AnimationHashes.RightHandHeldItemIdHash;
                floatHash = AnimationHashes.RightHandHeldItemIdFloatHash;
            }
            else
            {
                idHash = AnimationHashes.LeftHandHeldItemIdHash;
                floatHash = -1;
            }

            int animationParameter = animator.GetIntegerID(idHash);
            RLog.Debug($"Id Hash {idHash}; Float hash {floatHash}; Parameter {animationParameter}");

            var builder = new StringBuilder();
            foreach (var parameter in animator.parameters)
            {
                builder.AppendLine($"{parameter.name} ({parameter.type})");
            }

            RLog.DebugBig(builder);

            RLog.Debug("===========================");
            if (itemData._equippedAnimVars == null || itemData._equippedAnimVars.Length == 0)
            {
                RLog.Debug("No anim vars!");
            }
            else
            {
                RLog.Debug($"Fuck. {itemData._equippedAnimVars.Length}");
            }

            // TODO this method cannot be patched. Game crashes. Always.
            // SlotOverride is null. Active is true.
            // _item->_equipmentSlot?
            // id is RightHandHeldItemIdHash

            // Call to SetIntegerId(animator, id, itemId)
            // Results in (animator, RightHandHeldItemIdHash, 641).
            // Also sets the ID as a float on the FloatHash for the right hand.
            // Inspect "equippedAnimVars" on item data.
            // The solution lies somewhere within the animator stuff and ApplyAnimVars...
        }

        /// <summary>
        ///     Finds the associated controller item data with the provided id.
        /// </summary>
        /// <param name="array">The array to search in.</param>
        /// <param name="id">The id to search for.</param>
        /// <returns>The controller item data instance.</returns>
        /// <exception cref="InvalidOperationException">If the data could not be found.</exception>
        private static ControllerItemData GetItemData(
            Il2CppReferenceArray<ControllerItemData> array,
            int id)
        {
            for (int index = 0; index < array.Length; index++)
            {
                ControllerItemData data = array[index];
                if (data == null)
                {
                    throw new InvalidOperationException("Encountered invalid controller data!");
                }
                else if (data._itemId != id)
                {
                    continue;
                }

                return data;
            }

            throw new InvalidOperationException($"Could not find controller data with id {id}!");
        }

        /// <summary>
        ///     Attempts to find an appropriate item mesh for the held item.
        ///     This is done by either using the object's renderer directly or assuming that
        ///     the item has an LOD group and using the LOD0 renderer object. The LOD0 instance
        ///     cannot have multiple renderers. If no LOD group exists on the object, attempts
        ///     to find a renderer within the object's children and uses the first one.
        ///     If none exists, returns null.
        /// </summary>
        /// <param name="prefab">The prefab to find the item mesh for.</param>
        /// <returns>An appropriate item mesh or null.</returns>
        private static MeshRenderer FindHeldItemMesh(GameObject prefab)
        {
            if (prefab.TryGetComponent<MeshRenderer>(out var renderer))
            {
                // If the object itself has a mesh renderer then we want that one.
                return renderer;
            }

            LODGroup lodGroup;
            if (!prefab.TryGetComponent(out lodGroup))
            {
                // We attempt to find a renderer within the children.
                return prefab.GetComponentInChildren<MeshRenderer>();
            }

            LOD bestLOD = lodGroup.GetLODs()[0];
            if (bestLOD.renderers.Length != 1)
            {
                // There are too many or too few renderers we can use.
                return null;
            }

            // We use the LOD0 renderer. We cannot assume it being a mesh renderer.
            return bestLOD.renderers[0].gameObject.GetComponent<MeshRenderer>();
        }
    }
}
