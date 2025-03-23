using Bolt;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using RedLoader;
using Sons.Items.Core;
using SonsSdk;
using SonsSdk.Attributes;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace BobTheBuilder.Api
{
    /// <summary>
    ///     The resource manager can be used to instantiate prefabs for all resource types.
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        ///     A lit shader instance.
        /// </summary>
        public static Shader LitShader { get; private set; }

        /// <summary>
        ///     The global item ID of logs.
        /// </summary>
        public const int LogItemId = 78;

        /// <summary>
        ///     The global item ID of rocks.
        /// </summary>
        public const int RockItemId = 640;

        /// <summary>
        ///     The largest item id we consider to be valid.
        /// </summary>
        private const int MaxValidItemId = short.MaxValue;

        /// <summary>
        ///     The prefab base object.
        /// </summary>
        private static GameObject _prefabBase;

        /// <summary>
        ///     A mapping of item ids to their registered resources.
        /// </summary>
        private readonly Dictionary<int, RegisteredResource> _resources = new();

        /// <summary>
        ///     The pickup manager instance.
        /// </summary>
        private readonly PickupManager _pickupManager;

        /// <summary>
        ///     The item data for a wooden log as chopped down from a tree.
        /// </summary>
        private readonly ItemData _logData;

        /// <summary>
        ///     The item data for a stone that can be found within the world.
        /// </summary>
        private readonly ItemData _rockData;

        /// <summary>
        ///     Instantiates the resource manager.
        /// </summary>
        internal ResourceManager()
        {
            _pickupManager = new PickupManager(this);

            // Create the prefab base, keep it in memory and set it as non-active.
            _prefabBase = new GameObject("Prefab Base");
            UnityEngine.Object.DontDestroyOnLoad(_prefabBase);
            _prefabBase.SetActive(false);

            LitShader = Shader.Find(ShaderAssetMap.SonsHDRPLit);

            _logData = ItemDatabaseManager.ItemById(LogItemId);
            _rockData = ItemDatabaseManager.ItemById(RockItemId);
        }

        /// <summary>
        ///     Returns the prefab instance of a log.
        /// </summary>
        public ItemData LogData => _logData;

        /// <summary>
        ///     Returns the prefab instance of a rock.
        /// </summary>
        public ItemData RockData => _rockData;

        /// <summary>
        ///     Returns the pickup manager of this instance.
        /// </summary>
        internal PickupManager PickupManager => _pickupManager;

        /// <summary>
        ///     An internal reference to all registered resources.
        /// </summary>
        internal Dictionary<int, RegisteredResource> Resources => _resources;

        /// <summary>
        ///     Creates an item data based on the provided instance.
        ///     If desired, creates clones of all of its prefabs that are ready to be modified.
        ///     Will register every prefab clone to the network.
        /// </summary>
        /// <param name="baseData">The data to base this item data on.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="clonePrefabs">Whether or not prefab instances should be cloned.</param>
        /// <returns>A newly created item data instance in form of a registered resource.</returns>
        /// <exception cref="ArgumentNullException">If baseData or name is null.</exception>
        public RegisteredResource CreateResourceItem(
            ItemData baseData,
            string name,
            bool clonePrefabs = true)
        {
            if (baseData == null)
            {
                throw new ArgumentNullException(nameof(baseData));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            // Copy the base item data.
            ItemData newData = UnityEngine.Object.Instantiate(baseData);
            newData.SetId(FindNextValidItemId(baseData.Id));

            newData.name = name;
            newData.SetName(name);

            // Fix the UI data.
            ItemUiData uiData = newData._uiData;
            uiData._itemId = newData.Id;
            uiData._title = name;
            uiData._titlePlural = $"{name}{{TitlePlural}}";
            uiData._translationKey = name.ToUpperInvariant();

            // Register the item with the database manager.
            ItemDatabaseManager._itemsCache.Add(newData.Id, newData);

            RegisteredResource resource = new RegisteredResource(newData);
            _resources[newData.Id] = resource;

            // We now fix up the item data's prefabs. We create a clone of the prefabs that are
            // present so that they can be safely modified.
            if (!clonePrefabs)
            {
                return resource;
            }

            // We use the private field properties here since they give us access to a) update
            // the prefabs and b) are the backing types of the public getter properties.
            Transform pickupPrefab = newData._pickupPrefab;
            if (pickupPrefab != null)
            {
                Transform clone = CopyPrefab(pickupPrefab);
                clone.hideFlags = HideFlags.HideAndDontSave;
                clone.name = $"{name}Pickup";

                newData._pickupPrefab = clone;
                resource.PickupBoltId = RegisterNetworkPrefab(clone.gameObject);
            }

            Transform pickupBundlePrefab = newData._pickupBundlePrefab;
            if (pickupBundlePrefab != null)
            {
                Transform clone = CopyPrefab(pickupBundlePrefab);
                clone.hideFlags = HideFlags.HideAndDontSave;
                clone.name = $"{name}PickupBundle";

                newData._pickupBundlePrefab = clone;
                resource.PickupBundleBoltId = RegisterNetworkPrefab(clone.gameObject);
            }

            Transform heldPrefab = newData._heldPrefab;
            if (heldPrefab != null)
            {
                Transform clone = CopyPrefab(heldPrefab);
                clone.hideFlags = HideFlags.HideAndDontSave;
                clone.name = $"{name}Held";

                newData._heldPrefab = clone;
                resource.HeldBoltId = RegisterNetworkPrefab(clone.gameObject);
            }

            Transform propPrefab = newData._propPrefab;
            if (propPrefab != null)
            {
                Transform clone = CopyPrefab(propPrefab);
                clone.hideFlags = HideFlags.HideAndDontSave;
                clone.name = $"{name}Prop";

                newData._propPrefab = clone;
                resource.PropBoltId = RegisterNetworkPrefab(clone.gameObject);
            }

            return resource;
        }

        /// <summary>
        ///     Creates a copy of the given prefab that can be modified at runtime.
        ///     Instantiates the prefab within a game object that is deactivated as to
        ///     not run any behaviors.
        /// </summary>
        /// <typeparam name="T">The prefab type.</typeparam>
        /// <param name="prefab">The prefab to copy.</param>
        /// <returns>A copy of the prefab.</returns>
        /// <exception cref="InvalidOperationException">If the prefab base is null.</exception>
        public static T CopyPrefab<T>(T prefab)
            where T : UnityEngine.Object
        {
            if (_prefabBase == null)
            {
                throw new InvalidOperationException("Cannot instantiate prefab without base.");
            }

            _prefabBase.SetActive(false);
            return UnityEngine.Object.Instantiate(prefab, _prefabBase.transform);
        }

        /// <summary>
        ///     Converts all game object properties within the given type
        ///     that are attributed with an asset reference into game prefabs.
        /// </summary>
        /// <param name="assetBundle">The type containing the prefabs.</typeparam>
        /// <param name="targetMaterial">The material to use on the prefabs.</param>
        /// <exception cref="ArgumentNullException">
        ///     If assetBundle or targetMaterial is null.
        /// </exception>
        public static void CreateGamePrefabs(Type assetBundle, Material targetMaterial)
        {
            if (assetBundle == null)
            {
                throw new ArgumentNullException(nameof(assetBundle));
            }

            if (targetMaterial == null)
            {
                throw new ArgumentNullException(nameof(targetMaterial));
            }

            PropertyInfo[] properties = assetBundle.GetProperties(
                BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<AssetReferenceAttribute>() == null
                    || property.PropertyType != typeof(GameObject))
                {
                    // We only care about GameObject asset references.
                    continue;
                }

                // We update the prefab.
                GameObject asset = (GameObject)property.GetValue(null);
                GameObject prefab = CreateResourcePrefab(asset, targetMaterial);

                property.SetValue(null, prefab);
            }
        }

        /// <summary>
        ///     Fixes the given material so that it's visible ingame.
        /// </summary>
        /// <param name="material">The material to fix.</param>
        /// <exception cref="ArgumentNullException">If material is null.</exception>
        public static void FixMaterial(Material material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            material.shader = LitShader;
        }

        /// <summary>
        ///     Creates a new material that can be used in-game from the provided arguments.
        /// </summary>
        /// <param name="color">The default color.</param>
        /// <param name="baseTexture">The albedo texture.</param>
        /// <param name="normalMap">The normal map.</param>
        /// <param name="mask">The HDRP mask texture.</param>
        /// <returns>A lit material instance.</returns>
        public static Material NewMaterial(
            Color color = default,
            Texture2D baseTexture = null,
            Texture2D normalMap = null,
            Texture2D mask = null)
        {
            Material result = new Material(LitShader);
            if (color != default)
            {
                result.SetColor("_Color", color);
                result.SetColor("_BaseColor", color);
            }

            if (baseTexture != null)
            {
                result.SetTexture("_MainTex", baseTexture);
                result.SetTexture("_BaseColorMap", baseTexture);
            }

            if (normalMap != null)
            {
                result.SetTexture("_NormalMap", normalMap);
            }

            if (mask != null)
            {
                result.SetTexture("_MaskMap", mask);
            }

            return result;
        }

        /// <summary>
        ///     Creates a prefab instance from the provided value that complied with the game.
        ///     If the prefab contains LODs, copies the lowest resolution LOD and disables shadow
        ///     casting for all LODs and sets the copied instance as a shadow caster.
        ///     Updates the primary material of all assets to the given material instance.
        /// </summary>
        /// <param name="prefab">The prefab to make an in-game resource.</param>
        /// <param name="targetMaterial">The material of the prefab.</param>
        /// <returns>A new prefab instance set up for the game.</returns>
        /// <exception cref="ArgumentNullException">If prefab or targetMaterial is null.</exception>
        public static GameObject CreateResourcePrefab(GameObject prefab, Material targetMaterial)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            if (targetMaterial == null)
            {
                throw new ArgumentNullException(nameof(targetMaterial));
            }

            GameObject gamePrefab = CopyPrefab(prefab);
            gamePrefab.hideFlags = HideFlags.HideAndDontSave;

            bool isLodded = gamePrefab.TryGetComponent<LODGroup>(out _);

            // We set the primary material.
            Renderer[] childRenderers = gamePrefab.transform
                .GetComponentsInChildren<Renderer>(true);

            // We also compute the lowest LOD resolution of the prefab.
            Transform lowestLOD = null;
            foreach (Renderer renderer in childRenderers)
            {
                // We set up common properties.
                renderer.material = targetMaterial;
                renderer.rayTracingMode = RayTracingMode.DynamicTransform;
                renderer.motionVectors = true;
                renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;

                if (!isLodded)
                {
                    continue;
                }

                // We check if the renderer is part of lodding.
                Transform transform = renderer.transform;
                string name = transform.name;
                if (!Regex.IsMatch(name, @"_LOD\d$"))
                {
                    continue;
                }

                // Disable shadow casting for lodded prefabs.
                renderer.castShadows = false;
                renderer.shadowCastingMode = ShadowCastingMode.Off;

                if (lowestLOD == null)
                {
                    lowestLOD = transform;
                    continue;
                }

                // We compare last digits.
                char lastDigit = name[^1];
                char currentLowestDigit = lowestLOD.name[^1];
                if (currentLowestDigit >= lastDigit)
                {
                    // The renderer LOD is a higher LOD. Remember, LOD3 is lower than LOD2.
                    continue;
                }

                lowestLOD = transform;
            }

            if (!isLodded || lowestLOD == null)
            {
                // We're done.
                return gamePrefab;
            }

            // We create the shadow caster.
            GameObject shadowCaster = UnityEngine.Object.Instantiate(
                lowestLOD.gameObject,
                gamePrefab.transform);

            shadowCaster.name = "Shadow";

            Renderer shadowRenderer = shadowCaster.GetComponent<Renderer>();
            shadowRenderer.castShadows = true;
            shadowRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

            return gamePrefab;
        }

        /// <summary>
        ///     Finds the next valid item ID after the provided initial ID.
        /// </summary>
        /// <param name="initialId">The id to start searching after.</param>
        /// <returns>The next item ID that can be used.</returns>
        private static int FindNextValidItemId(int initialId)
        {
            var itemCache = ItemDatabaseManager._itemsCache;
            for (int itemId = initialId + 1; itemId <= MaxValidItemId; itemId++)
            {
                // We check if this id is contained within the cache. If so,
                // we continue.
                if (itemCache.ContainsKey(itemId))
                {
                    continue;
                }

                // This is the first valid item id.
                return itemId;
            }

            // We give up.
            return -1;
        }

        /// <summary>
        ///     Registers a given prefab with the bolt network prefab database.
        /// </summary>
        /// <param name="prefab">The prefab to register.</param>
        /// <returns>The id under which the prefab was registered.</returns>
        private static int RegisterNetworkPrefab(GameObject prefab)
        {
            PrefabDatabase database = PrefabDatabase.Instance;
            var prefabs = database.Prefabs;

            // There's probably a better way to do this than index-based copying.
            int nextPrefabId = prefabs.Length;
            var newPrefabs = new Il2CppReferenceArray<GameObject>(nextPrefabId + 1);
            for (int index = 0; index < nextPrefabId; index++)
            {
                newPrefabs[index] = prefabs[index];
            }

            newPrefabs[nextPrefabId] = prefab;
            database.Prefabs = newPrefabs;

            // We store the prefab instance in the cache as well.
            var lookup = PrefabDatabase._lookup;
            if (lookup != null)
            {
                lookup[new PrefabId(nextPrefabId)] = prefab;
            }

            return nextPrefabId;
        }

        /// <summary>
        ///     Prints the object tree of a given transform.
        /// </summary>
        /// <param name="builder">The string builder to print to.</param>
        /// <param name="current">The current transform.</param>
        /// <param name="depth">The depth of the current transform.</param>
        private static void PrintObjectTree(
            StringBuilder builder,
            Transform current,
            int depth)
        {
            // We start with the name of the object.
            builder.Append(new string(' ', depth)).Append(current.name).AppendLine(":");

            string indent = new string(' ', depth + 1);
            // Then we print all components.
            foreach (Component component in current.GetComponents<Component>())
            {
                builder.Append(indent)
                    .Append('<')
                    .Append(component.GetIl2CppType().Name)
                    .AppendLine(">");
            }

            // Append all children.
            for (int child = 0; child < current.childCount; child++)
            {
                Transform childTransform = current.GetChild(child);
                PrintObjectTree(builder, childTransform, depth + 1);
            }

            if (depth == 0)
            {
                RLog.Debug(builder.ToString());
            }
        }
    }
}
