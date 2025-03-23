using BobTheBuilder.Api;
using HarmonyLib;
using Sons.Animation;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;

using LiftCoroutine = TheForest.Items.Special.HeldOnlyItemController.__c__DisplayClass32_0.ObjectCompilerGeneratedNPrivateSealedIEnumerator1ObjectIEnumeratorIDisposableInObGacuObObUnique;

namespace BobTheBuilder.Patches
{
    /// <summary>
    ///     Contain patches regarding held-only items.
    /// </summary>
    internal static class HeldOnlyItemControllerPatches
    {
        /// <summary>
        ///     A note on this: I'd like to pach ItemAnimationHashHelper$$ApplyAnimVars
        ///     however for some reason this crashes the runtime every time it's invoked.
        ///     So this is our best workaround here.
        /// </summary>
        [HarmonyPatch(typeof(LiftCoroutine), nameof(LiftCoroutine.MoveNext))]
        internal static class HeldCoroutinePlayPickupAnimationFix
        {
            /// <summary>
            ///     The animation state index that starts the held animation pickup.
            /// </summary>
            private const int HeldAnimationStartStateIndex = 3;

            /// <summary>
            ///     Appends to the original method so that instead of the resource item id
            ///     that's expected to be picked up, the stone/log pickup animation is played
            ///     under which our instances are present.
            /// </summary>
            /// <param name="__instance">The coroutine instance.</param>
            public static void Postfix(LiftCoroutine __instance)
            {
                if (__instance.__1__state != HeldAnimationStartStateIndex)
                {
                    return;
                }

                var heldItemData = __instance.__4__this.heldItemData;
                int itemId = heldItemData._itemId;

                var registeredResources = BobTheBuilder.Instance.ResourceManager.Resources;
                if (!registeredResources.TryGetValue(itemId, out var resource)
                    || resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.None)
                {
                    // The resource is not registered with us.
                    return;
                }

                int animatorItemId;
                if (resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.Rock)
                {
                    animatorItemId = ResourceManager.RockItemId;
                }
                else
                {
                    animatorItemId = ResourceManager.LogItemId;
                }

                // We set the animator variables required.
                Animator animator = LocalPlayer.Animator;
                animator.SetIntegerID(
                    AnimationHashes.RightHandHeldItemIdHash,
                    animatorItemId);

                animator.SetFloatID(
                    AnimationHashes.RightHandHeldItemIdFloatHash,
                    animatorItemId);

                // TODO if the item data would have any "equipped animator variables" then
                // we should apply them here.
                if (resource.ItemData.EquippedAnimVars != null
                    && resource.ItemData.EquippedAnimVars.Length != 0)
                {
                    throw new InvalidOperationException("Need to handle equipped anim vars.");
                }
            }
        }

        /// <summary>
        ///     A note on this: I'd like to pach ItemAnimationHashHelper$$ApplyAnimVars
        ///     however for some reason this crashes the runtime every time it's invoked.
        ///     So this is our best workaround here.
        /// </summary>
        [HarmonyPatch(
            typeof(HeldOnlyItemController),
            nameof(HeldOnlyItemController.RemoveHeldItem))]
        internal static class HeldOnlyItemControllerRemoveHeldItemAnimationFix
        {
            /// <summary>
            ///     Appends to the original method so that instead of the resource item id
            ///     that's expected to be dropped, the stone/log item id is used.
            /// </summary>
            /// <param name="__instance">The coroutine instance.</param>
            public static void Postfix(
                HeldOnlyItemController __instance,
                bool equipPrevious,
                bool drop)
            {
                if (__instance._heldCount != 0)
                {
                    // We do not care about drops that have items remaining.
                    return;
                }

                int itemId = __instance._heldItemId;

                var registeredResources = BobTheBuilder.Instance.ResourceManager.Resources;
                if (!registeredResources.TryGetValue(itemId, out var resource)
                    || resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.None)
                {
                    // The resource is not registered with us.
                    return;
                }

                int animatorItemId;
                if (resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.Rock)
                {
                    animatorItemId = ResourceManager.RockItemId;
                }
                else
                {
                    animatorItemId = ResourceManager.LogItemId;
                }

                // Note that the integer id must be -1, but the float id must be the item.
                Animator animator = LocalPlayer.Animator;
                animator.SetIntegerID(
                    AnimationHashes.RightHandHeldItemIdHash,
                    -1);

                animator.SetFloatID(
                    AnimationHashes.RightHandHeldItemIdFloatHash,
                    animatorItemId);

                // TODO if the item data would have any "equipped animator variables" then
                // we should apply them here.
                if (resource.ItemData.EquippedAnimVars != null
                    && resource.ItemData.EquippedAnimVars.Length != 0)
                {
                    throw new InvalidOperationException("Need to handle equipped anim vars.");
                }
            }
        }
    }
}
