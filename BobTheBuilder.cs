using BobTheBuilder.Api;
using BobTheBuilder.Assets;
using RedLoader;
using SonsSdk;
using SUI;
using System.Collections;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BobTheBuilder;

/// <summary>
///     Primary class for the mod. Contains setup functionality only.
/// </summary>
public class BobTheBuilder : SonsMod
{
    /// <summary>
    ///     Whether or not the SDK was initialized.
    /// </summary>
    private bool _sdkInitialized = false;

    /// <summary>
    ///     All actions that should run when the mod was initialized.
    /// </summary>
    private event Action<BobTheBuilder> OnInitialized;

    /// <summary>
    ///     The resource manager instance.
    /// </summary>
    private ResourceManager _resourceManager = null;

    /// <summary>
    ///     The resources registered by this mod.
    /// </summary>
    private BobResources _bobResources;

    /// <summary>
    ///     Instantiates our mod.
    /// </summary>
    public BobTheBuilder()
    {
        Instance = this;
        HarmonyPatchAll = true;
    }

    /// <summary>
    ///     Returns the resource manager instance.
    ///     The resource manager is valid after OnSdkInitialized.
    /// </summary>
    public ResourceManager ResourceManager => _resourceManager;

    /// <summary>
    ///     The resources registered by this mod.
    /// </summary>
    public BobResources BobResources => _bobResources;

    /// <summary>
    ///     The instance of the mod.
    /// </summary>
    public static BobTheBuilder Instance { get; private set; }

    /// <summary>
    ///     Registers a listener for the initialization of this mod.
    /// </summary>
    /// <param name="listener">The listener instance.</param>
    public static void AddInitializationListener(Action<BobTheBuilder> listener)
    {
        if (listener == null || Instance == null)
        {
            return;
        }

        if (Instance._sdkInitialized)
        {
            // We invoke the listener directly since we're initalized already.
            listener(Instance);
            return;
        }

        // Register the listener.
        Instance.OnInitialized += listener;
    }

    /// <summary>
    ///     Runs initialization of code for when the SDK gets initialized.
    /// </summary>
    protected override void OnSdkInitialized()
    {
        // We create the resource manager instance.
        _resourceManager = new ResourceManager();
        _bobResources = new BobResources(_resourceManager);

        SettingsRegistry.CreateSettings(this, null, typeof(BobConfig));

        // ClassInjector.RegisterTypeInIl2Cpp<KeyPressObject>();
        
        // Then we convert all asset bundles.
        BobPolishedStoneAssets.CreateGamePrefabs();

        // Finally we run all potential mods waiting on us.
        _sdkInitialized = true;

        OnInitialized?.Invoke(this);
        OnInitialized = null;

        // We subscribe on the BeforeLoadSave event. This is when we perform operations
        // that require the player/inventory/other objects in the scene.
        SdkEvents.BeforeLoadSave.Subscribe(() =>
        {
            _resourceManager.PickupManager.RegisterPickUpsInHeldItemController();
        });
    }

    /// <summary>
    ///     Initializes our mod.
    /// </summary>
    protected override void OnInitializeMod()
    {
        BobConfig.Initialize();
        BobConfig.SpawnKey.Notify(TestMethod);
    }

    /// <summary>
    ///     Called when a scene is initialized. Used for debugging purposes only to
    ///     perform tests within the main menu scene.
    /// </summary>
    /// <param name="sonsScene"></param>
    protected override void OnSonsSceneInitialized(ESonsScene sonsScene)
    {
        if (sonsScene != ESonsScene.Title)
        {
            return;
        }

        // Runs the custom update method within the main menu as well.
        IEnumerator customUpdate()
        {
            while (true)
            {
                if (Keyboard.current != null
                    && Keyboard.current[Key.O].wasPressedThisFrame
                    && !LocalPlayer.IsInWorld)
                {
                    TestMethod();
                }

                yield return new WaitForEndOfFrame();
            }
        };

        Coroutines.Start(customUpdate());
    }

    /// <summary>
    ///     Test method.
    /// </summary>
    private void TestMethod()
    {
        if (_bobResources == null)
        {
            return;
        }

        PickupManager.SpawnResourcePickup(
            _bobResources.SmoothSlab,
            GetSpawnLocation(4, 2),
            Quaternion.identity,
            false);
    }

    /// <summary>
    ///     Gets a valid spawn location depending on the scene.
    /// </summary>
    /// <param name="distance">The distance to the player.</param>
    /// <param name="height">The height above the player.</param>
    /// <returns>A valid spawn location.</returns>
    private Vector3 GetSpawnLocation(float distance, float height)
    {
        if (LocalPlayer.IsInWorld)
        {
            return SonsTools.GetPositionInFrontOfPlayer(distance, height);
        }

        return Vector3.zero;
    }
}