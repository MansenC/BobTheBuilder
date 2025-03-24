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
        ///     The name Id of a polished slab.
        /// </summary>
        public const string PolishedSlabName = "PolishedSlab";

        /// <summary>
        ///     The name Id of a cement bag.
        /// </summary>
        public const string CementBagName = "CementBag";

        /// <summary>
        ///     The path to the addressable asset of a polished slab item.
        /// </summary>
        private const string PolishedSlabAsset = "PolishedStone/PolishedSlabItem.prefab";

        /// <summary>
        ///     The path to the addressable asset of a cement bag.
        /// </summary>
        private const string CementBagAsset = "Items/CementBag.prefab";

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
        ///     The held positions for the cement item.
        /// </summary>
        private static readonly Vector3[] CementHeldPositions = new Vector3[4]
        {
            new Vector3(0f, -0.2f, 0.2f),
            new Vector3(-0.1f, 0f, 0.5f),
            new Vector3(-0.15f, 0f, 0.87f),
            new Vector3(0f, 0f, 1.2f),
        };

        /// <summary>
        ///     The held rotations for the cement item.
        /// </summary>
        private static readonly Vector3[] CementHeldRotations = new Vector3[4]
        {
            new Vector3(270f, 103f, 0f),
            new Vector3(0f, 0f, 15f),
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 3f, 0f),
        };

        /// <summary>
        ///     The held scales for the cement item.
        /// </summary>
        private static readonly Vector3[] CementHeldScales = new Vector3[4]
        {
            new Vector3(2.5f, 2.5f, 4.5f),
            new Vector3(2.5f, 2.5f, 3.5f),
            new Vector3(2.5f, 2.5f, 3.2f),
            new Vector3(2.5f, 2.5f, 3f),
        };

        /// <summary>
        ///     A material representing glass.
        /// </summary>
        private readonly Material _glassMaterial;

        /// <summary>
        ///     The resource data for smooth stone slabs.
        /// </summary>
        private readonly RegisteredResource _smoothSlabResource;

        /// <summary>
        ///     The resource data for cement bags.
        /// </summary>
        private readonly RegisteredResource _cementBagResource;

        /// <summary>
        ///     Constructs a new instance of this class. Registers and creates all resources.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        public BobResources(ResourceManager resourceManager)
        {
            _glassMaterial = CreateGlassMaterial();

            _smoothSlabResource = resourceManager.CreateResource(
                resourceManager.RockData,
                PolishedSlabName,
                RegisteredResource.HeldOnlyBaseType.Rock,
                PolishedSlabAsset,
                (transform, index) =>
                {
                    transform.localPosition = PolishedSlabHeldPositions[index];
                    transform.localEulerAngles = PolishedSlabHeldRotations[index];
                    transform.localScale = PolishedSlabHeldScale;
                });

            _cementBagResource = resourceManager.CreateResource(
                resourceManager.RockData,
                CementBagName,
                RegisteredResource.HeldOnlyBaseType.Rock,
                CementBagAsset,
                (transform, index) =>
                {
                    transform.localPosition = CementHeldPositions[index];
                    transform.localEulerAngles = CementHeldRotations[index];
                    transform.localScale = CementHeldScales[index];
                });
        }

        /// <summary>
        ///     The registered resource instance for smooth slab.
        /// </summary>
        public RegisteredResource SmoothSlab => _smoothSlabResource;

        /// <summary>
        ///     The cement bag resource.
        /// </summary>
        public RegisteredResource CementBag => _cementBagResource;

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
