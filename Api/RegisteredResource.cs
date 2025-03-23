using Sons.Items.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BobTheBuilder.Api
{
    /// <summary>
    ///     Represents a resource that is registered with the resource manager
    ///     and sons in a compatible way.
    /// </summary>
    public sealed class RegisteredResource
    {
        /// <summary>
        ///     Defines the base type for a held-only resource, like rocks or logs.
        /// </summary>
        public enum HeldOnlyBaseType
        {
            None,
            Rock,
            Log
        }

        /// <summary>
        ///     The item's ID within sons.
        /// </summary>
        private readonly int _itemId;

        /// <summary>
        ///     The item data as registered within the game.
        /// </summary>
        private readonly ItemData _itemData;

        /// <summary>
        ///     The bolt prefab id for the pickup prefab.
        /// </summary>
        private int _pickupBoltId = -1;

        /// <summary>
        ///     The bolt prefab id for the pickup bundle prefab.
        /// </summary>
        private int _pickupBundleBoltId = -1;

        /// <summary>
        ///     The bolt prefab id for the held prefab.
        /// </summary>
        private int _heldBoltId = -1;

        /// <summary>
        ///     The bolt prefab id for the prop prefab.
        /// </summary>
        private int _propBoltId = -1;

        /// <summary>
        ///     Creates a registered resource instance.
        /// </summary>
        /// <param name="itemData"></param>
        internal RegisteredResource(ItemData itemData)
        {
            _itemId = itemData.Id;
            _itemData = itemData;
        }

        /// <summary>
        ///     The item's ID within sons.
        /// </summary>
        public int ItemId => _itemId;

        /// <summary>
        ///     The item data as registered within the game.
        /// </summary>
        public ItemData ItemData => _itemData;

        /// <summary>
        ///     The bolt prefab id for the pickup prefab.
        /// </summary>
        public int PickupBoltId
        {
            get => _pickupBoltId;
            internal set => _pickupBoltId = value;
        }

        /// <summary>
        ///     The bolt prefab id for the pickup bundle prefab.
        /// </summary>
        public int PickupBundleBoltId
        {
            get => _pickupBundleBoltId;
            internal set => _pickupBundleBoltId = value;
        }

        /// <summary>
        ///     The bolt prefab id for the held prefab.
        /// </summary>
        public int HeldBoltId
        {
            get => _heldBoltId;
            internal set => _heldBoltId = value;
        }

        /// <summary>
        ///     The bolt prefab id for the prop prefab.
        /// </summary>
        public int PropBoltId
        {
            get => _propBoltId;
            internal set => _propBoltId = value;
        }

        /// <summary>
        ///     The held-only type of this resource.
        ///     Configure this property so that the resource gets registered as a held-only
        ///     item within the inventory system. The type defines on which resource the
        ///     definition of the item should be based on.
        /// </summary>
        public HeldOnlyBaseType HeldOnlyType { get; set; } = HeldOnlyBaseType.None;

        /// <summary>
        ///     Applies only for HeldOnly types.
        ///     An action that can be set if the held position of the resource should be customized.
        ///     If null, the held positions of the base type are used, if the max carry amount is
        ///     less than or equal to the base type, otherwise throws an exception.
        ///     <br></br>
        ///     If set, the action is called for each position that needs to be instantiated, once
        ///     for every index below the max carry amount (four times for rocks, for example).
        ///     The provided parameters are the transform of the held position instance and the
        ///     current index. The action must modify at least the position, but may as well
        ///     modify rotation or scale. Any other action will most likely result in unintended
        ///     behavior.
        /// </summary>
        public Action<Transform, int> ApplyHeldPosition { get; set; }

        /// <summary>
        ///     The asset reference for the renderable of the pickup item.
        ///     May get loaded for held-only types.
        /// </summary>
        internal AssetReference RenderableAsset { get; set; }
    }
}
