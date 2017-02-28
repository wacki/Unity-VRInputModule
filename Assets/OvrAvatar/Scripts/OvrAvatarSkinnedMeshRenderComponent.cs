using UnityEngine;
using System.Collections;
using System;

public class OvrAvatarSkinnedMeshRenderComponent : OvrAvatarRenderComponent
{
    SkinnedMeshRenderer mesh;
    Transform[] bones;

    internal void Initialize(ovrAvatarRenderPart_SkinnedMeshRender skinnedMeshRender, int thirdPersonLayer, int firstPersonLayer, int sortOrder)
    {
        mesh = CreateSkinnedMesh(skinnedMeshRender.meshAssetID, skinnedMeshRender.visibilityMask, false, thirdPersonLayer, firstPersonLayer, sortOrder);
        bones = mesh.bones;
    }

    internal void UpdateSkinnedMeshRender(OvrAvatar avatar, ovrAvatarRenderPart_SkinnedMeshRender meshRender)
    {
        UpdateSkinnedMesh(avatar, mesh, bones, meshRender.localTransform, meshRender.visibilityMask, meshRender.skinnedPose);
        UpdateAvatarMaterial(mesh.sharedMaterial, meshRender.materialState);
    }
}
