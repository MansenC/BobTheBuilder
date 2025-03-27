using BobTheBuilder.Api;
using HarmonyLib;
using Sons.Items.Core;
using TheForest.Items.Special;
using TheForest.Utils;
using TheForest.Utils.Items;

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

                int itemId = __instance.__4__this.heldItemData._itemId;

                ResourceManager resourceManager = BobTheBuilder.Instance.ResourceManager;
                var registeredResources = resourceManager.Resources;
                if (!registeredResources.TryGetValue(itemId, out var resource)
                    || resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.None)
                {
                    // The resource is not registered with us.
                    return;
                }

                ItemData animatorData;
                if (resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.Rock)
                {
                    animatorData = resourceManager.RockData;
                }
                else
                {
                    animatorData = resourceManager.LogData;
                }

                ItemAnimatorHashHelper.ApplyAnimVars(
                    animatorData,
                    true,
                    LocalPlayer.Animator,
                    new Il2CppSystem.Nullable<EquipmentSlot>());
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

                ResourceManager resourceManager = BobTheBuilder.Instance.ResourceManager;
                var registeredResources = resourceManager.Resources;
                if (!registeredResources.TryGetValue(itemId, out var resource)
                    || resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.None)
                {
                    // The resource is not registered with us.
                    return;
                }

                ItemData animatorData;
                if (resource.HeldOnlyType == RegisteredResource.HeldOnlyBaseType.Rock)
                {
                    animatorData = resourceManager.RockData;
                }
                else
                {
                    animatorData = resourceManager.LogData;
                }

                ItemAnimatorHashHelper.ApplyAnimVars(
                    animatorData,
                    false,
                    LocalPlayer.Animator,
                    new Il2CppSystem.Nullable<EquipmentSlot>());
            }
        }
    }
}
