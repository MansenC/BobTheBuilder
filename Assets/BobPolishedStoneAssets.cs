using BobTheBuilder.Api;
using SonsSdk.Attributes;
using UnityEngine;

namespace BobTheBuilder.Assets
{
    /// <summary>
    ///     Contains all asset references for the polished stone buildables.
    ///     Every game object is a prefab with LODs and Colliders, but no materials.
    /// </summary>
    [AssetBundle("bob_polishedstone")]
    public static class BobPolishedStoneAssets
    {
        /// <summary>
        ///     The smooth stone Material.
        /// </summary>
        public static Material MaterialSmoothStone { get; private set; }

        [AssetReference("SmoothStone.png")]
        public static Texture2D TextureSmoothStone { get; set; }

        [AssetReference("SmoothStone_Normal.png")]
        public static Texture2D TextureSmoothStoneNormal { get; set; }

        [AssetReference("SmoothStone_Mask.png")]
        public static Texture2D TextureSmoothStoneMask { get; set; }

        #region Beam

        /// <summary>
        ///     The object for a full beam.
        /// </summary>
        [AssetReference("PolishedSlabBeamCombined")]
        public static GameObject BeamCombined { get; set; }

        /// <summary>
        ///     The chunk element A of a beam.
        /// </summary>
        [AssetReference("PolishedSlabBeamChunkElementA")]
        public static GameObject BeamChunkElementA { get; set; }

        /// <summary>
        ///     The chunk element B of a beam.
        /// </summary>
        [AssetReference("PolishedSlabBeamChunkElementB")]
        public static GameObject BeamChunkElementB { get; set; }

        /// <summary>
        ///     The chunk element C of a beam.
        /// </summary>
        [AssetReference("PolishedSlabBeamChunkElementC")]
        public static GameObject BeamChunkElementC { get; set; }

        /// <summary>
        ///     The chunk element D of a beam.
        /// </summary>
        [AssetReference("PolishedSlabBeamChunkElementD")]
        public static GameObject BeamChunkElementD { get; set; }

        /// <summary>
        ///     The wire element for an electric beam.
        /// </summary>
        [AssetReference("PolishedSlabBeamElectricWire")]
        public static GameObject BeamElectricWire { get; set; }

        #endregion

        #region Floor

        /// <summary>
        ///     The object for a full floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorCombined")]
        public static GameObject FloorCombined { get; set; }

        /// <summary>
        ///     The object for the element A part of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementA")]
        public static GameObject FloorElementA { get; set; }

        /// <summary>
        ///     The object for element A chunk A of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementAChunkA")]
        public static GameObject FloorElementAChunkA { get; set; }

        /// <summary>
        ///     The object for element A chunk B of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementAChunkB")]
        public static GameObject FloorElementAChunkB { get; set; }

        /// <summary>
        ///     The object for element A chunk C of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementAChunkC")]
        public static GameObject FloorElementAChunkC { get; set; }

        /// <summary>
        ///     The object for element A chunk D of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementAChunkD")]
        public static GameObject FloorElementAChunkD { get; set; }

        /// <summary>
        ///     The object for the element B part of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementB")]
        public static GameObject FloorElementB { get; set; }

        /// <summary>
        ///     The object for element B chunk A of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementBChunkA")]
        public static GameObject FloorElementBChunkA { get; set; }

        /// <summary>
        ///     The object for element B chunk B of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementBChunkB")]
        public static GameObject FloorElementBChunkB { get; set; }

        /// <summary>
        ///     The object for element B chunk C of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementBChunkC")]
        public static GameObject FloorElementBChunkC { get; set; }

        /// <summary>
        ///     The object for element B chunk D of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementBChunkD")]
        public static GameObject FloorElementBChunkD { get; set; }

        /// <summary>
        ///     The object for the element C part of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementC")]
        public static GameObject FloorElementC { get; set; }

        /// <summary>
        ///     The object for element C chunk A of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementCChunkA")]
        public static GameObject FloorElementCChunkA { get; set; }

        /// <summary>
        ///     The object for element C chunk B of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementCChunkB")]
        public static GameObject FloorElementCChunkB { get; set; }

        /// <summary>
        ///     The object for element C chunk C of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementCChunkC")]
        public static GameObject FloorElementCChunkC { get; set; }

        /// <summary>
        ///     The object for element C chunk D of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementCChunkD")]
        public static GameObject FloorElementCChunkD { get; set; }

        /// <summary>
        ///     The object for the element D part of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementD")]
        public static GameObject FloorElementD { get; set; }

        /// <summary>
        ///     The object for element D chunk A of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementDChunkA")]
        public static GameObject FloorElementDChunkA { get; set; }

        /// <summary>
        ///     The object for element D chunk B of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementDChunkB")]
        public static GameObject FloorElementDChunkB { get; set; }

        /// <summary>
        ///     The object for element D chunk C of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementDChunkC")]
        public static GameObject FloorElementDChunkC { get; set; }

        /// <summary>
        ///     The object for element D chunk D of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementDChunkD")]
        public static GameObject FloorElementDChunkD { get; set; }

        /// <summary>
        ///     The object for the element E part of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementE")]
        public static GameObject FloorElementE { get; set; }

        /// <summary>
        ///     The object for element E chunk A of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementEChunkA")]
        public static GameObject FloorElementEChunkA { get; set; }

        /// <summary>
        ///     The object for element E chunk B of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementEChunkB")]
        public static GameObject FloorElementEChunkB { get; set; }

        /// <summary>
        ///     The object for element E chunk C of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementEChunkC")]
        public static GameObject FloorElementEChunkC { get; set; }

        /// <summary>
        ///     The object for element E chunk D of a floor.
        /// </summary>
        [AssetReference("PolishedSlabFloorElementEChunkD")]
        public static GameObject FloorElementEChunkD { get; set; }

        #endregion

        #region Pillar

        /// <summary>
        ///     The object for a full pillar.
        /// </summary>
        [AssetReference("PolishedSlabPillarCombined")]
        public static GameObject PillarCombined { get; set; }

        /// <summary>
        ///     The chunk element A of a pillar.
        /// </summary>
        [AssetReference("PolishedSlabPillarChunkElementA")]
        public static GameObject PillarChunkElementA { get; set; }

        /// <summary>
        ///     The chunk element B of a pillar.
        /// </summary>
        [AssetReference("PolishedSlabPillarChunkElementB")]
        public static GameObject PillarChunkElementB { get; set; }

        /// <summary>
        ///     The chunk element C of a pillar.
        /// </summary>
        [AssetReference("PolishedSlabPillarChunkElementC")]
        public static GameObject PillarChunkElementC { get; set; }

        /// <summary>
        ///     The chunk element D of a pillar.
        /// </summary>
        [AssetReference("PolishedSlabPillarChunkElementD")]
        public static GameObject PillarChunkElementD { get; set; }

        #endregion

        #region Wall

        /// <summary>
        ///     The object for a full wall.
        /// </summary>
        [AssetReference("PolishedSlabWallCombined")]
        public static GameObject WallCombined { get; set; }

        /// <summary>
        ///     The object for the element A part of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementA")]
        public static GameObject WallElementA { get; set; }

        /// <summary>
        ///     The object for element A chunk A of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementAChunkA")]
        public static GameObject WallElementAChunkA { get; set; }

        /// <summary>
        ///     The object for element A chunk B of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementAChunkB")]
        public static GameObject WallElementAChunkB { get; set; }

        /// <summary>
        ///     The object for element A chunk C of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementAChunkC")]
        public static GameObject WallElementAChunkC { get; set; }

        /// <summary>
        ///     The object for element A chunk D of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementAChunkD")]
        public static GameObject WallElementAChunkD { get; set; }

        /// <summary>
        ///     The object for the element B part of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementB")]
        public static GameObject WallElementB { get; set; }

        /// <summary>
        ///     The object for element B chunk A of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementBChunkA")]
        public static GameObject WallElementBChunkA { get; set; }

        /// <summary>
        ///     The object for element B chunk B of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementBChunkB")]
        public static GameObject WallElementBChunkB { get; set; }

        /// <summary>
        ///     The object for element B chunk C of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementBChunkC")]
        public static GameObject WallElementBChunkC { get; set; }

        /// <summary>
        ///     The object for element B chunk D of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementBChunkD")]
        public static GameObject WallElementBChunkD { get; set; }

        /// <summary>
        ///     The object for the element C part of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementC")]
        public static GameObject WallElementC { get; set; }

        /// <summary>
        ///     The object for element C chunk A of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementCChunkA")]
        public static GameObject WallElementCChunkA { get; set; }

        /// <summary>
        ///     The object for element C chunk B of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementCChunkB")]
        public static GameObject WallElementCChunkB { get; set; }

        /// <summary>
        ///     The object for element C chunk C of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementCChunkC")]
        public static GameObject WallElementCChunkC { get; set; }

        /// <summary>
        ///     The object for element C chunk D of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementCChunkD")]
        public static GameObject WallElementCChunkD { get; set; }

        /// <summary>
        ///     The object for the element D part of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementD")]
        public static GameObject WallElementD { get; set; }

        /// <summary>
        ///     The object for element D chunk A of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementDChunkA")]
        public static GameObject WallElementDChunkA { get; set; }

        /// <summary>
        ///     The object for element D chunk B of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementDChunkB")]
        public static GameObject WallElementDChunkB { get; set; }

        /// <summary>
        ///     The object for element D chunk C of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementDChunkC")]
        public static GameObject WallElementDChunkC { get; set; }

        /// <summary>
        ///     The object for element D chunk D of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementDChunkD")]
        public static GameObject WallElementDChunkD { get; set; }

        /// <summary>
        ///     The object for the element E part of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementE")]
        public static GameObject WallElementE { get; set; }

        /// <summary>
        ///     The object for element E chunk A of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementEChunkA")]
        public static GameObject WallElementEChunkA { get; set; }

        /// <summary>
        ///     The object for element E chunk B of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementEChunkB")]
        public static GameObject WallElementEChunkB { get; set; }

        /// <summary>
        ///     The object for element E chunk C of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementEChunkC")]
        public static GameObject WallElementEChunkC { get; set; }

        /// <summary>
        ///     The object for element E chunk D of a wall.
        /// </summary>
        [AssetReference("PolishedSlabWallElementEChunkD")]
        public static GameObject WallElementEChunkD { get; set; }

        #endregion

        #region Window

        /// <summary>
        ///     The object window element A.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementA")]
        public static GameObject WindowElementA { get; set; }

        /// <summary>
        ///     The object window element A chunk A.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementAChunkA")]
        public static GameObject WindowElementAChunkA { get; set; }

        /// <summary>
        ///     The object window element A chunk D.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementAChunkD")]
        public static GameObject WindowElementAChunkD { get; set; }

        /// <summary>
        ///     The object window element B.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementB")]
        public static GameObject WindowElementB { get; set; }

        /// <summary>
        ///     The object window element B chunk A.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementBChunkA")]
        public static GameObject WindowElementBChunkA { get; set; }

        /// <summary>
        ///     The object window element B chunk D.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementBChunkD")]
        public static GameObject WindowElementBChunkD { get; set; }

        /// <summary>
        ///     The object window element C.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementC")]
        public static GameObject WindowElementC { get; set; }

        /// <summary>
        ///     The object window element C chunk A.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementCChunkA")]
        public static GameObject WindowElementCChunkA { get; set; }

        /// <summary>
        ///     The object window element C chunk D.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementCChunkD")]
        public static GameObject WindowElementCChunkD { get; set; }

        /// <summary>
        ///     The object window element A.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementD")]
        public static GameObject WindowElementD { get; set; }

        /// <summary>
        ///     The object window element D chunk A.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementDChunkA")]
        public static GameObject WindowElementDChunkA { get; set; }

        /// <summary>
        ///     The object window element D chunk D.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementDChunkD")]
        public static GameObject WindowElementDChunkD { get; set; }

        /// <summary>
        ///     The object window element E.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementE")]
        public static GameObject WindowElementE { get; set; }

        /// <summary>
        ///     The object window element E chunk A.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementEChunkA")]
        public static GameObject WindowElementEChunkA { get; set; }

        /// <summary>
        ///     The object window element E chunk D.
        /// </summary>
        [AssetReference("PolishedSlabWallWindowElementEChunkD")]
        public static GameObject WindowElementEChunkD { get; set; }

        #endregion

        /// <summary>
        ///     Creates game prefab instances for every prefab that's referenced here.
        /// </summary>
        public static void CreateGamePrefabs()
        {
            MaterialSmoothStone = ResourceManager.NewMaterial(
                baseTexture: TextureSmoothStone,
                normalMap: TextureSmoothStoneNormal,
                mask: TextureSmoothStoneMask);

            ResourceManager.CreateGamePrefabs(typeof(BobPolishedStoneAssets), MaterialSmoothStone);
        }
    }
}
