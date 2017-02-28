using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OvrAvatarRenderComponent : MonoBehaviour {

    static readonly string[] LayerKeywords = new[] { "LAYERS_0", "LAYERS_1", "LAYERS_2", "LAYERS_3", "LAYERS_4", "LAYERS_5", "LAYERS_6", "LAYERS_7", "LAYERS_8", };
    static readonly string[] LayerSampleModeParameters = new[] { "_LayerSampleMode0", "_LayerSampleMode1", "_LayerSampleMode2", "_LayerSampleMode3", "_LayerSampleMode4", "_LayerSampleMode5", "_LayerSampleMode6", "_LayerSampleMode7", };
    static readonly string[] LayerBlendModeParameters = new[] { "_LayerBlendMode0", "_LayerBlendMode1", "_LayerBlendMode2", "_LayerBlendMode3", "_LayerBlendMode4", "_LayerBlendMode5", "_LayerBlendMode6", "_LayerBlendMode7", };
    static readonly string[] LayerMaskTypeParameters = new[] { "_LayerMaskType0", "_LayerMaskType1", "_LayerMaskType2", "_LayerMaskType3", "_LayerMaskType4", "_LayerMaskType5", "_LayerMaskType6", "_LayerMaskType7", };
    static readonly string[] LayerColorParameters = new[] { "_LayerColor0", "_LayerColor1", "_LayerColor2", "_LayerColor3", "_LayerColor4", "_LayerColor5", "_LayerColor6", "_LayerColor7", };
    static readonly string[] LayerSurfaceParameters = new[] { "_LayerSurface0", "_LayerSurface1", "_LayerSurface2", "_LayerSurface3", "_LayerSurface4", "_LayerSurface5", "_LayerSurface6", "_LayerSurface7", };
    static readonly string[] LayerSampleParametersParameters = new[] { "_LayerSampleParameters0", "_LayerSampleParameters1", "_LayerSampleParameters2", "_LayerSampleParameters3", "_LayerSampleParameters4", "_LayerSampleParameters5", "_LayerSampleParameters6", "_LayerSampleParameters7", };
    static readonly string[] LayerMaskParametersParameters = new[] { "_LayerMaskParameters0", "_LayerMaskParameters1", "_LayerMaskParameters2", "_LayerMaskParameters3", "_LayerMaskParameters4", "_LayerMaskParameters5", "_LayerMaskParameters6", "_LayerMaskParameters7", };
    static readonly string[] LayerMaskAxisParameters = new[] { "_LayerMaskAxis0", "_LayerMaskAxis1", "_LayerMaskAxis2", "_LayerMaskAxis3", "_LayerMaskAxis4", "_LayerMaskAxis5", "_LayerMaskAxis6", "_LayerMaskAxis7", };

    Dictionary<Material, ovrAvatarMaterialState> materialStates = new Dictionary<Material, ovrAvatarMaterialState>();

    protected void UpdateActive(OvrAvatar avatar, ovrAvatarVisibilityFlags mask)
    {
        bool active = avatar.ShowFirstPerson && (mask & ovrAvatarVisibilityFlags.FirstPerson) != 0;
        active |= avatar.ShowThirdPerson && (mask & ovrAvatarVisibilityFlags.ThirdPerson) != 0;
        this.gameObject.SetActive(active);
    }

    protected SkinnedMeshRenderer CreateSkinnedMesh(ulong assetID, ovrAvatarVisibilityFlags visibilityMask, bool physicallyBasedShader, int thirdPersonLayer, int firstPersonLayer, int sortingOrder)
    {
        OvrAvatarAssetMesh meshAsset = (OvrAvatarAssetMesh)OvrAvatarSDKManager.Instance.GetAsset(assetID);
        if (meshAsset == null)
        {
            throw new Exception("Couldn't find mesh for asset " + assetID);
        }
        if ((visibilityMask & ovrAvatarVisibilityFlags.ThirdPerson) != 0)
        {
            this.gameObject.layer = thirdPersonLayer;
        }
        else
        {
            this.gameObject.layer = firstPersonLayer;
        }
        SkinnedMeshRenderer renderer = meshAsset.CreateSkinnedMeshRendererOnObject(gameObject);
        renderer.sharedMaterial = CreateAvatarMaterial(gameObject.name + "_material", physicallyBasedShader: physicallyBasedShader, selfOccluding: (visibilityMask & ovrAvatarVisibilityFlags.SelfOccluding) != 0);
        renderer.quality = SkinQuality.Bone4;
        renderer.sortingOrder = sortingOrder;
        if ((visibilityMask & ovrAvatarVisibilityFlags.SelfOccluding) == 0)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        return renderer;
    }

    protected void UpdateSkinnedMesh(OvrAvatar avatar, SkinnedMeshRenderer mesh, Transform[] bones, ovrAvatarTransform localTransform, ovrAvatarVisibilityFlags visibilityMask, ovrAvatarSkinnedMeshPose skinnedPose)
    {
        UpdateActive(avatar, visibilityMask);
        OvrAvatar.ConvertTransform(localTransform, this.transform);
        for (int i = 0; i < skinnedPose.jointCount; i++)
        {
            Transform targetBone = bones[i];
            OvrAvatar.ConvertTransform(skinnedPose.jointTransform[i], targetBone);
        }
    }

    protected Material CreateAvatarMaterial(string name, bool physicallyBasedShader, bool selfOccluding)
    {
        string shaderPath;
        if (physicallyBasedShader)
        {
            shaderPath = "OvrAvatar/AvatarSurfaceShaderPBS";
        }
        else
        {
            shaderPath = selfOccluding ? "OvrAvatar/AvatarSurfaceShaderSelfOccluding" : "OvrAvatar/AvatarSurfaceShader";
        }

        Shader s = Shader.Find(shaderPath);
        Material mat = new Material(s);
        mat.name = name;
        materialStates.Add(mat, new ovrAvatarMaterialState());
        return mat;
    }

    protected void UpdateAvatarMaterial(Material mat, ovrAvatarMaterialState matState)
    {
        // Only update the material if it's a material we actively created
        ovrAvatarMaterialState cachedState;
        if (!materialStates.TryGetValue(mat, out cachedState))
        {
            return;
        }
        if (cachedState.Equals(matState))
        {
            return;
        }

        mat.SetColor("_BaseColor", matState.baseColor);
        mat.SetInt("_BaseMaskType", (int)matState.baseMaskType);
        mat.SetVector("_BaseMaskParameters", matState.baseMaskParameters);
        mat.SetVector("_BaseMaskAxis", matState.baseMaskAxis);
        if (matState.alphaMaskTextureID != 0)
        {
            mat.SetTexture("_AlphaMask", GetLoadedTexture(matState.alphaMaskTextureID));
            mat.SetTextureScale("_AlphaMask", new Vector2(matState.alphaMaskScaleOffset.x, matState.alphaMaskScaleOffset.y));
            mat.SetTextureOffset("_AlphaMask", new Vector2(matState.alphaMaskScaleOffset.z, matState.alphaMaskScaleOffset.w));
        }
        if (matState.normalMapTextureID != 0)
        {
            mat.EnableKeyword("NORMAL_MAP_ON");
            mat.SetTexture("_NormalMap", GetLoadedTexture(matState.normalMapTextureID));
            mat.SetTextureScale("_NormalMap", new Vector2(matState.normalMapScaleOffset.x, matState.normalMapScaleOffset.y));
            mat.SetTextureOffset("_NormalMap", new Vector2(matState.normalMapScaleOffset.z, matState.normalMapScaleOffset.w));
        }
        if (matState.parallaxMapTextureID != 0)
        {
            mat.EnableKeyword("PARALLAX_ON");
            mat.SetTexture("_ParallaxMap", GetLoadedTexture(matState.parallaxMapTextureID));
            mat.SetTextureScale("_ParallaxMap", new Vector2(matState.parallaxMapScaleOffset.x, matState.parallaxMapScaleOffset.y));
            mat.SetTextureOffset("_ParallaxMap", new Vector2(matState.parallaxMapScaleOffset.z, matState.parallaxMapScaleOffset.w));
        }
        if (matState.parallaxMapTextureID != 0)
        {
            mat.EnableKeyword("ROUGHNESS_ON");
            mat.SetTexture("_RoughnessMap", GetLoadedTexture(matState.roughnessMapTextureID));
            mat.SetTextureScale("_RoughnessMap", new Vector2(matState.roughnessMapScaleOffset.x, matState.roughnessMapScaleOffset.y));
            mat.SetTextureOffset("_RoughnessMap", new Vector2(matState.roughnessMapScaleOffset.z, matState.roughnessMapScaleOffset.w));
        }
        mat.EnableKeyword(LayerKeywords[matState.layerCount]);
        for (ulong layerIndex = 0; layerIndex < matState.layerCount; layerIndex++)
        {
            ovrAvatarMaterialLayerState layer = matState.layers[layerIndex];

            mat.SetInt(LayerSampleModeParameters[layerIndex], (int)layer.sampleMode);
            mat.SetInt(LayerBlendModeParameters[layerIndex], (int)layer.blendMode);
            mat.SetInt(LayerMaskTypeParameters[layerIndex], (int)layer.maskType);
            mat.SetColor(LayerColorParameters[layerIndex], layer.layerColor);
            if (layer.sampleMode != ovrAvatarMaterialLayerSampleMode.Color)
            {
                string surfaceProperty = LayerSurfaceParameters[layerIndex];
                mat.SetTexture(surfaceProperty, GetLoadedTexture(layer.sampleTexture));
                mat.SetTextureScale(surfaceProperty, new Vector2(layer.sampleScaleOffset.x, layer.sampleScaleOffset.y));
                mat.SetTextureOffset(surfaceProperty, new Vector2(layer.sampleScaleOffset.z, layer.sampleScaleOffset.w));
            }
            mat.SetColor(LayerSampleParametersParameters[layerIndex], layer.sampleParameters);
            mat.SetColor(LayerMaskParametersParameters[layerIndex], layer.maskParameters);
            mat.SetColor(LayerMaskAxisParameters[layerIndex], layer.maskAxis);
        }
        materialStates[mat] = matState;
    }

    protected static Texture2D GetLoadedTexture(UInt64 assetId)
    {
        if (assetId == 0)
        {
            return null;
        }
        OvrAvatarAssetTexture tex = (OvrAvatarAssetTexture)OvrAvatarSDKManager.Instance.GetAsset(assetId);
        if (tex == null)
        {
            throw new Exception("Could not find texture for asset " + assetId);
        }
        return tex.texture;
    }
}
