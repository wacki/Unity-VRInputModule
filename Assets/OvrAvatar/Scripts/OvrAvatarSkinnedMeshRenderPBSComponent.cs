using UnityEngine;
using System.Collections;
using System;

public class OvrAvatarSkinnedMeshRenderPBSComponent : OvrAvatarRenderComponent {
    SkinnedMeshRenderer mesh;
    Transform[] bones;

    internal void Initialize(ovrAvatarRenderPart_SkinnedMeshRenderPBS skinnedMeshRenderPBS, int thirdPersonLayer, int firstPersonLayer, int sortOrder)
    {
        mesh = CreateSkinnedMesh(skinnedMeshRenderPBS.meshAssetID, skinnedMeshRenderPBS.visibilityMask, true, thirdPersonLayer, firstPersonLayer, sortOrder);
        bones = mesh.bones;
    }

    internal void UpdateSkinnedMeshRenderPBS(OvrAvatar avatar, ovrAvatarRenderPart_SkinnedMeshRenderPBS meshRender)
    {
        UpdateSkinnedMesh(avatar, mesh, bones, meshRender.localTransform, meshRender.visibilityMask, meshRender.skinnedPose);

        Material mat = mesh.sharedMaterial;
        mat.SetTexture("_Albedo", GetLoadedTexture(meshRender.albedoTextureAssetID));
        mat.SetTexture("_Surface", GetLoadedTexture(meshRender.surfaceTextureAssetID));
    }
}
