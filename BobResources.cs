using BobTheBuilder.Api;
using UnityEngine;

namespace BobTheBuilder
{
    /// <summary>
    ///     A class containing all resources registered by the bob mod.
    /// </summary>
    public class BobResources
    {
        /// <summary>
        ///     The name Id ofa polished slab.
        /// </summary>
        public const string PolishedSlabName = "PolishedSlab";

        /// <summary>
        ///     The path to the addressable asset of a polished slab item.
        /// </summary>
        private const string PolishedSlabAsset = "PolishedStone/PolishedSlabItem.prefab";

        /// <summary>
        ///     The positions of the held transforms for polished slabs.
        /// </summary>
        private static readonly Vector3[] PolishedSlabHeldPositions = new Vector3[4]
        {
            new Vector3(0f, 0f, 0.12f),
            new Vector3(-0.1f, 0.46f, 0.1f),
            new Vector3(-0.15f, 0.92f, 0.15f),
            new Vector3(0f, 1.37f, 0f),
        };

        /// <summary>
        ///     The euler angles of the rotations of the held transforms for polished slabs.
        /// </summary>
        private static readonly Vector3[] PolishedSlabHeldRotations = new Vector3[4]
        {
            new Vector3(0f, 0f, 10f),
            new Vector3(0f, 340f, 0f),
            new Vector3(0f, 10f, 0f),
            new Vector3(0f, 3f, 0f),
        };

        /// <summary>
        ///     The scale of the polished slabs.
        /// </summary>
        private static readonly Vector3 PolishedSlabHeldScale = new Vector3(3f, 3f, 3f);

        /// <summary>
        ///     A material representing glass.
        /// </summary>
        private readonly Material _glassMaterial;

        /// <summary>
        ///     The item data for smooth stone slabs.
        /// </summary>
        private readonly RegisteredResource _smoothSlabResource;

        /// <summary>
        ///     Constructs a new instance of this class. Registers and creates all resources.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        public BobResources(ResourceManager resourceManager)
        {
            _glassMaterial = CreateGlassMaterial();

            _smoothSlabResource = resourceManager.CreateResourceItem(
                resourceManager.RockData,
                PolishedSlabName);

            _smoothSlabResource.HeldOnlyType = RegisteredResource.HeldOnlyBaseType.Rock;
            _smoothSlabResource.ApplyHeldPosition = (transform, index) =>
            {
                transform.localPosition = PolishedSlabHeldPositions[index];
                transform.localEulerAngles = PolishedSlabHeldRotations[index];
                transform.localScale = PolishedSlabHeldScale;
            };

            PickupManager.FixRockLikePickup(
                _smoothSlabResource,
                PolishedSlabAsset);
        }

        /// <summary>
        ///     The registered resource instance for smooth slab.
        /// </summary>
        public RegisteredResource SmoothSlab => _smoothSlabResource;

        /// <summary>
        ///     Creates a glass material.
        /// </summary>
        /// <returns>A glass material.</returns>
        private static Material CreateGlassMaterial()
        {
            Color targetColor = new Color(0.315f, 1f, 0.702f, 0.086f);

            Material glassMaterial = new Material(ResourceManager.LitShader);
            glassMaterial.SetColor("_BaseColor", targetColor);
            glassMaterial.SetColor("_Color", targetColor);

            // Make transparent
            glassMaterial.SetFloat("_SurfaceType", 1);

            // Set to irridescence
            glassMaterial.SetFloat("_MaterialId", 2);

            // Set refraction model to planar.
            glassMaterial.SetFloat("_RefractionModel", 1);
            glassMaterial.SetFloat("_Thickness", 0.2f);

            return glassMaterial;
        }
    }
}
