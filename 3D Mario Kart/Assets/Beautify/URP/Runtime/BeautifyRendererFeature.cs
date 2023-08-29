using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Beautify.Universal {
    public class BeautifyRendererFeature : ScriptableRendererFeature {

        public const string SKW_SHARPEN = "BEAUTIFY_SHARPEN";
        public const string SKW_TONEMAP_ACES = "BEAUTIFY_TONEMAP_ACES";
        public const string SKW_LUT = "BEAUTIFY_LUT";
        public const string SKW_BLOOM = "BEAUTIFY_BLOOM";
        public const string SKW_BLOOM_USE_DEPTH = "BEAUTIFY_BLOOM_USE_DEPTH";
        public const string SKW_BLOOM_USE_LAYER = "BEAUTIFY_BLOOM_USE_LAYER";
        public const string SKW_DIRT = "BEAUTIFY_DIRT";
        public const string SKW_ANAMORPHIC_FLARES_USE_DEPTH = "BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH";
        public const string SKW_DEPTH_OF_FIELD = "BEAUTIFY_DEPTH_OF_FIELD";
        public const string SKW_VIGNETTING = "BEAUTIFY_VIGNETTING";
        public const string SKW_VIGNETTING_MASK = "BEAUTIFY_VIGNETTING_MASK";
        public const string SKW_PURKINJE = "BEAUTIFY_PURKINJE";
        public const string SKW_EYE_ADAPTATION = "BEAUTIFY_EYE_ADAPTATION";
        public const string SKW_OUTLINE = "BEAUTIFY_OUTLINE";
        public const string SKW_TURBO = "BEAUTIFY_TURBO";
        public const string SKW_COLOR_TWEAKS = "BEAUTIFY_COLOR_TWEAKS";
        public const string SKW_NIGHT_VISION = "BEAUTIFY_NIGHT_VISION";
        public const string SKW_DITHER = "BEAUTIFY_DITHER";


        class BeautifyRenderPass : ScriptableRenderPass {

            public Beautify beautify;

            enum Pass {
                CopyExact = 0,
                Compare = 1,
                Beautify = 2,
                BloomLuminance = 3,
                BloomDebug = 4,
                BlurHoriz = 5,
                BlurVert = 6,
                BloomCompose = 7,
                BloomResample = 8,
                BloomResampleAndCombine = 9,
                BloomLuminanceAntiflicker = 10,
                AnamorphicFlaresResample = 11,
                AnamorphicFlaresResampleAndCombine = 12,
                ComputeScreenLum = 13,
                DownsampleScreenLum = 14,
                BlendScreenLum = 15,
                SimpleBlendLum = 16,
                AnamorphicFlaresLuminance = 17,
                AnamorphicFlaresLuminanceAntiflicker = 18,
                SunFlares = 19,
                SunFlaresAdditive = 20,
                DoFCoC = 21,
                DoFCoCDebug = 22,
                DoFBlur = 23,
                DoFBlurWithoutBokeh = 24,
                DoFBlurHorizontally = 25,
                DoFBlurVertically = 26,
                CopyBilinear = 27
            }

            static class ShaderParams {
                public static int inputTex;
                public static int sharpen;
                public static int colorParams;
                public static int colorBoost;
                public static int tintColor;
                public static int compareTex;
                public static int compareParams;
                public static int fxColor;
                public static int lutTex;
                public static int colorTemp;

                public static int blurScale;
                public static int tempBlurRT;
                public static int tempBloomCustomComposeRT;
                public static int tempBlurOneDirRT;
                public static int tempBlurOneDirRTOriginal;
                public static int tempBlurDownscaling;

                public static int bloom;
                public static int bloomWeights;
                public static int bloomWeights2;
                public static int bloomDepthThreshold;
                public static int bloomTex, bloomTex1, bloomTex2, bloomTex3, bloomTex4;
                public static int bloomTint;
                public static int bloomExclusionZBias;

                public static int dirt;
                public static int dirtTex;
                public static int screenLum;

                public static int afData;
                public static int afDepthThreshold;
                public static int afTintColor;
                public static int afCombineTex;

                public static int sfSunData;
                public static int sfSunPos;
                public static int sfSunPosRightEye;
                public static int sfSunTintColor;
                public static int sfCoronaRays1;
                public static int sfCoronaRays2;
                public static int sfGhosts1;
                public static int sfGhosts2;
                public static int sfGhosts3;
                public static int sfGhosts4;
                public static int sfHalo;
                public static int sfRT;
                public static int sfFlareTex;

                public static int dofRT;
                public static int dofTempBlurDoFAlphaRT;
                public static int dofTempBlurDoFTemp1RT;
                public static int dofTempBlurDoFTemp2RT;
                public static int dofBokehData;
                public static int dofBokehData2;
                public static int dofBokehData3;

                public static int vignette;
                public static int vignetteAspectRatio;
                public static int vignetteMask;

                public static int purkinje;

                public static int eaLumSrc;
                public static int eaHist;
                public static int eaParams;

                public static int outline;

                public static int blurRT;

                public static int nightVision;


                static ShaderParams() {
                    inputTex = Shader.PropertyToID("_BeautifyInputTex");

                    sharpen = Shader.PropertyToID("_Sharpen");
                    colorParams = Shader.PropertyToID("_Params");
                    colorBoost = Shader.PropertyToID("_ColorBoost");
                    tintColor = Shader.PropertyToID("_TintColor");
                    compareTex = Shader.PropertyToID("_CompareTex");
                    compareParams = Shader.PropertyToID("_CompareParams");
                    fxColor = Shader.PropertyToID("_FXColor");
                    lutTex = Shader.PropertyToID("_LUTTex");
                    colorTemp = Shader.PropertyToID("_ColorTemp");

                    tempBlurRT = Shader.PropertyToID("_BeautifyTempBlurRT");
                    tempBloomCustomComposeRT = Shader.PropertyToID("_BeautifyTempBloomCustomComposeRT");
                    tempBlurOneDirRT = tempBlurOneDirRTOriginal = Shader.PropertyToID("_BeautifyTempBlurOneDir0");
                    tempBlurDownscaling = Shader.PropertyToID("_BeautifyTempBlurDownscaling");

                    blurScale = Shader.PropertyToID("_BlurScale");

                    bloom = Shader.PropertyToID("_Bloom");
                    bloomWeights = Shader.PropertyToID("_BloomWeights");
                    bloomWeights2 = Shader.PropertyToID("_BloomWeights2");
                    bloomDepthThreshold = Shader.PropertyToID("_BloomDepthThreshold");
                    bloomTex = Shader.PropertyToID("_BloomTex");
                    bloomTex1 = Shader.PropertyToID("_BloomTex1");
                    bloomTex2 = Shader.PropertyToID("_BloomTex2");
                    bloomTex3 = Shader.PropertyToID("_BloomTex3");
                    bloomTex4 = Shader.PropertyToID("_BloomTex4");
                    bloomTint = Shader.PropertyToID("_BloomTint");
                    bloomExclusionZBias = Shader.PropertyToID("_BloomLayerZBias");

                    dirt = Shader.PropertyToID("_Dirt");
                    dirtTex = Shader.PropertyToID("_OverlayTex");
                    screenLum = Shader.PropertyToID("_ScreenLum");

                    afData = Shader.PropertyToID("_AFData");
                    afDepthThreshold = Shader.PropertyToID("_AFDepthThreshold");
                    afTintColor = Shader.PropertyToID("_AFTint");
                    afCombineTex = Shader.PropertyToID("_CombineTex");

                    sfSunData = Shader.PropertyToID("_SunData");
                    sfSunPos = Shader.PropertyToID("_SunPos");
                    sfSunPosRightEye = Shader.PropertyToID("_SunPosRightEye");
                    sfSunTintColor = Shader.PropertyToID("_SunTint");
                    sfCoronaRays1 = Shader.PropertyToID("_SunCoronaRays1");
                    sfCoronaRays2 = Shader.PropertyToID("_SunCoronaRays2");
                    sfGhosts1 = Shader.PropertyToID("_SunGhosts1");
                    sfGhosts2 = Shader.PropertyToID("_SunGhosts2");
                    sfGhosts3 = Shader.PropertyToID("_SunGhosts3");
                    sfGhosts4 = Shader.PropertyToID("_SunGhosts4");
                    sfHalo = Shader.PropertyToID("_SunHalo");
                    sfRT = Shader.PropertyToID("_BeautifyTempSF0");
                    sfFlareTex = Shader.PropertyToID("_FlareTex");

                    dofRT = Shader.PropertyToID("_DoFTex");
                    dofTempBlurDoFAlphaRT = Shader.PropertyToID("_BeautifyTempBlurAlphaDoF");
                    dofTempBlurDoFTemp1RT = Shader.PropertyToID("_BeautifyTempBlurPass1DoF");
                    dofTempBlurDoFTemp2RT = Shader.PropertyToID("_BeautifyTempBlurPass2DoF");
                    dofBokehData = Shader.PropertyToID("_BokehData");
                    dofBokehData2 = Shader.PropertyToID("_BokehData2");
                    dofBokehData3 = Shader.PropertyToID("_BokehData3");

                    vignette = Shader.PropertyToID("_Vignetting");
                    vignetteAspectRatio = Shader.PropertyToID("_VignettingAspectRatio");
                    vignetteMask = Shader.PropertyToID("_VignettingMask");

                    purkinje = Shader.PropertyToID("_Purkinje");

                    eaLumSrc = Shader.PropertyToID("_EALumSrc");
                    eaHist = Shader.PropertyToID("_EAHist");
                    eaParams = Shader.PropertyToID("_EyeAdaptation");

                    outline = Shader.PropertyToID("_Outline");

                    blurRT = Shader.PropertyToID("_BlurTex");

                    nightVision = Shader.PropertyToID("_NightVision");
                }
            }

            struct BloomMipData {
                public int rtDown, rtUp, width, height;
                public int rtDownOriginal, rtUpOriginal;
            }



            const int PYRAMID_COUNT_BLOOM = 5;
            const int PYRAMID_COUNT_BLOOM_TURBO = 3;
            const int PYRAMID_COUNT_EA = 9;

            Material bMat;
            RenderTargetIdentifier source;
            CameraData cameraData;
            RenderTextureDescriptor sourceDesc, sourceDescHP;
            bool supportsFPTextures;
            BloomMipData[] rt, rtAF;
            int[] rtEA;
            Texture2D dirtTexture, flareTex;
            float sunFlareCurrentIntensity;
            Vector4 sunLastScrPos;
            float sunLastRot;
            float sunFlareTime;
            float dofPrevDistance, dofLastAutofocusDistance;
            Vector4 dofLastBokehData;
            RenderTexture rtEAacum, rtEAHist;
            bool requiresLuminanceComputation;
            bool usesBloomAndFlares, usesDepthOfField, usesVignetting;

            List<string> keywords = new List<string>();
            string[] keywordsArray;
            bool setup;

            public void Setup(Shader shader, ScriptableRenderer renderer, RenderingData renderingData) {

                // Configures where the render pass should be injected.
                FindVolumeComponent();
                if (beautify != null && beautify.directWrite.value) {
                    renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
                } else {
                    renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
                }

                if (setup && cameraData.camera != null) return;
                setup = true;

                cameraData = renderingData.cameraData;
                source = renderer.cameraColorTarget;
                supportsFPTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
                if (bMat == null) bMat = CoreUtils.CreateEngineMaterial(shader);

                // Initialize bloom buffers descriptors
                if (rt == null || rt.Length != PYRAMID_COUNT_BLOOM + 1) {
                    rt = new BloomMipData[PYRAMID_COUNT_BLOOM + 1];
                }
                for (int k = 0; k < rt.Length; k++) {
                    rt[k].rtDown = rt[k].rtDownOriginal = Shader.PropertyToID("_BeautifyBloomDownMip" + k);
                    rt[k].rtUp = rt[k].rtUpOriginal = Shader.PropertyToID("_BeautifyBloomUpMip" + k);
                }

                // Initialize anamorphic flare buffers descriptors
                if (rtAF == null || rtAF.Length != PYRAMID_COUNT_BLOOM + 1) {
                    rtAF = new BloomMipData[PYRAMID_COUNT_BLOOM + 1];
                }
                for (int k = 0; k < rtAF.Length; k++) {
                    rtAF[k].rtDown = rtAF[k].rtDownOriginal = Shader.PropertyToID("_BeautifyAFDownMip" + k);
                    rtAF[k].rtUp = rtAF[k].rtUpOriginal = Shader.PropertyToID("_BeautifyAFUpMip" + k);
                }

                // Initialize eye adaptation buffers descriptors
                if (rtEA == null || rtEA.Length != PYRAMID_COUNT_EA) {
                    rtEA = new int[PYRAMID_COUNT_EA];
                }
                for (int k = 0; k < rtEA.Length; k++) {
                    rtEA[k] = Shader.PropertyToID("_BeautifyEAMip" + k);
                }

            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {

                if (bMat == null) return;

                FindVolumeComponent();

                if (beautify == null || !beautify.IsActive()) return;

                sourceDesc = cameraTextureDescriptor;
                sourceDesc.msaaSamples = 1;
                sourceDesc.depthBufferBits = 0;
                if (beautify.downsampling.value && beautify.downsamplingMultiplier.value > 1f) {
                    sourceDesc.width = (int)(sourceDesc.width / beautify.downsamplingMultiplier.value);
                    sourceDesc.height = (int)(sourceDesc.height / beautify.downsamplingMultiplier.value);
                }
                sourceDescHP = sourceDesc;
                if (supportsFPTextures) {
                    sourceDescHP.colorFormat = RenderTextureFormat.ARGBHalf;
                }
                UpdateMaterialProperties();

            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                if (bMat == null) {
                    Debug.LogError("Beautify material not initialized.");
                    return;
                }

                Camera cam = cameraData.camera;

                if (beautify == null || cam == null || !beautify.IsActive()) return;

                if (cam.cameraType == CameraType.SceneView && beautify.hideInSceneView.value) return;

                RestoreRTBufferIds();

                var cmd = CommandBufferPool.Get("Beautify");

                if (usesBloomAndFlares) {
                    DoBloomAndFlares(cmd);
                }

                if (requiresLuminanceComputation) {
                    DoEyeAdaptation(cmd);
                }

                if (usesDepthOfField) {
                    DoDoF(cmd);
                }

                if (usesVignetting) {
                    DoVignette();
                }

                if (beautify.directWrite.value) {
                    // direct output to camera
                    if (beautify.debugOutput.value == Beautify.DebugOutput.DepthOfFieldCoC) {
                        if (beautify.depthOfField.value) {
                            // we ignore input contents
                            Blit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.DoFCoCDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.BloomAndFlares) {
                        if (beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || beautify.sunFlaresIntensity.value > 0 || beautify.lensDirtIntensity.value > 0) {
                            // we ignore input contents
                            Blit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.BloomDebug);
                        }
                    } else if (beautify.blurIntensity.value > 0) {
                        ApplyFinalBlur(cmd);

                        if (beautify.compareMode.value) {
                            cmd.GetTemporaryRT(ShaderParams.compareTex, sourceDesc, FilterMode.Point);
                            Blit(cmd, ShaderParams.blurRT, ShaderParams.compareTex, bMat, (int)Pass.Beautify);
                            Blit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.Compare);
                            cmd.ReleaseTemporaryRT(ShaderParams.compareTex);
                        } else {
                            Blit(cmd, ShaderParams.blurRT, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.Beautify);
                        }
                    } else if (beautify.compareMode.value) {
                        cmd.GetTemporaryRT(ShaderParams.compareTex, sourceDesc, FilterMode.Point);
                        Blit(cmd, source, ShaderParams.compareTex, bMat, (int)Pass.Beautify);
                        Blit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.Compare);
                        cmd.ReleaseTemporaryRT(ShaderParams.compareTex);
                    } else {
                        Blit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.Beautify);
                    }

                } else {
                    bool useBilinearFiltering = beautify.downsampling.value && beautify.downsamplingMultiplier.value > 1f && beautify.downsamplingBilinear.value;
                    int copyPass = useBilinearFiltering ? (int)Pass.CopyBilinear : (int)Pass.CopyExact;

                    cmd.GetTemporaryRT(ShaderParams.inputTex, sourceDesc, (!beautify.downsampling.value || (beautify.downsamplingMultiplier.value > 1f && !beautify.downsamplingBilinear.value)) ? FilterMode.Point : FilterMode.Bilinear);

                    if (beautify.debugOutput.value == Beautify.DebugOutput.DepthOfFieldCoC) {
                        if (beautify.depthOfField.value) {
                            // we ignore input contents
                            Blit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.DoFCoCDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.BloomAndFlares) {
                        if (beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || beautify.sunFlaresIntensity.value > 0 || beautify.lensDirtIntensity.value > 0) {
                            // we ignore input contents
                            Blit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.BloomDebug);
                        }
                    } else if (beautify.blurIntensity.value > 0) {
                        ApplyFinalBlur(cmd);

                        if (beautify.compareMode.value) {
                            cmd.GetTemporaryRT(ShaderParams.compareTex, sourceDesc, FilterMode.Point);
                            Blit(cmd, ShaderParams.blurRT, ShaderParams.compareTex, bMat, (int)Pass.Beautify);
                            Blit(cmd, source, ShaderParams.inputTex, bMat, copyPass);
                            Blit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.Compare);
                            cmd.ReleaseTemporaryRT(ShaderParams.compareTex);
                        } else {
                            Blit(cmd, ShaderParams.blurRT, source, bMat, (int)Pass.Beautify);
                        }
                    } else if (beautify.compareMode.value) {
                        cmd.GetTemporaryRT(ShaderParams.compareTex, sourceDesc, FilterMode.Point);
                        Blit(cmd, source, ShaderParams.compareTex, bMat, (int)Pass.Beautify);
                        Blit(cmd, source, ShaderParams.inputTex, bMat, copyPass);
                        Blit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.Compare);
                        cmd.ReleaseTemporaryRT(ShaderParams.compareTex);
                    } else {
                        Blit(cmd, source, ShaderParams.inputTex, bMat, copyPass);
                        Blit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.Beautify);
                    }

                    cmd.ReleaseTemporaryRT(ShaderParams.inputTex);
                }

                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);

            }



            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd) {
            }

            public void Cleanup() {
                CoreUtils.Destroy(bMat);
                CoreUtils.Destroy(bMatDoFPass2);
                CoreUtils.Destroy(bMatDoFPass3);
                CoreUtils.Destroy(finalBlurMat);
                CoreUtils.Destroy(finalBlurMatDownscalingPass1);
            }

            void FindVolumeComponent() {
                if (beautify == null) {
                    beautify = VolumeManager.instance.stack.GetComponent<Beautify>();
                }
            }


            void RestoreRTBufferIds() {
                // Restore temorary rt ids
                for (int k = 0; k < rt.Length; k++) {
                    rt[k].rtDown = rt[k].rtDownOriginal;
                    rt[k].rtUp = rt[k].rtUpOriginal;
                }
                for (int k = 0; k < rtAF.Length; k++) {
                    rtAF[k].rtDown = rtAF[k].rtDownOriginal;
                    rtAF[k].rtUp = rtAF[k].rtUpOriginal;
                }
                ShaderParams.tempBlurOneDirRT = ShaderParams.tempBlurOneDirRTOriginal;
            }

            Material finalBlurMatDownscalingPass1, finalBlurMat;
            void ApplyFinalBlur(CommandBuffer cmd) {

                int size;
                RenderTextureDescriptor rtBlurDesc = sourceDescHP;

                float blurIntensity = beautify.blurIntensity.value;
                if (blurIntensity < 1f) {
                    size = (int)Mathf.Lerp(rtBlurDesc.width, 512, blurIntensity);
                } else {
                    size = (int)(512 / blurIntensity);
                }
                float aspectRatio = (float)sourceDesc.height / sourceDesc.width;
                rtBlurDesc.width = size;
                rtBlurDesc.height = Mathf.Max(1, (int)(size * aspectRatio));
                cmd.GetTemporaryRT(ShaderParams.blurRT, rtBlurDesc, FilterMode.Bilinear);

                float ratio = (float)sourceDesc.width / size;
                if (finalBlurMatDownscalingPass1 == null) {
                    finalBlurMatDownscalingPass1 = Instantiate(bMat);
                }
                float blurScale = blurIntensity > 1f ? 1f : blurIntensity;
                finalBlurMatDownscalingPass1.SetFloat(ShaderParams.blurScale, blurScale * ratio);

                BlurThisDownscaling(cmd, rtBlurDesc, source, ShaderParams.blurRT, ratio, finalBlurMatDownscalingPass1, bMat);

                if (finalBlurMat == null) {
                    finalBlurMat = Instantiate(bMat);
                }
                finalBlurMat.SetFloat(ShaderParams.blurScale, blurScale);

                BlurThis(cmd, rtBlurDesc, ShaderParams.blurRT, rtBlurDesc.width, rtBlurDesc.height, finalBlurMat, blurScale);
                if (!beautify.turboMode.value) {
                    BlurThis(cmd, rtBlurDesc, ShaderParams.blurRT, rtBlurDesc.width, rtBlurDesc.height, finalBlurMat, blurScale);
                    BlurThis(cmd, rtBlurDesc, ShaderParams.blurRT, rtBlurDesc.width, rtBlurDesc.height, finalBlurMat, blurScale);
                }
            }



            void DoBloomAndFlares(CommandBuffer cmd) {

                Camera cam = cameraData.camera;
                bool sunFlareEnabled = false;
                if (beautify.sunFlaresIntensity.value > 0) {
                    CheckSun();
                    sunFlareEnabled = sceneSettings != null && sceneSettings.sun != null;
                }

                if (beautify.lensDirtIntensity.value > 0 || beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || sunFlareEnabled) {

                    int mipCount = beautify.turboMode.value ? PYRAMID_COUNT_BLOOM_TURBO : PYRAMID_COUNT_BLOOM;
                    float aspectRatio = (float)sourceDesc.height / sourceDesc.width;
                    int rtBloom = -1;
                    int downsamping = beautify.turboMode.value ? 4 : 2;
                    int lensDirtSpread = beautify.turboMode.value ? 2 : beautify.lensDirtSpread.value;

                    if (beautify.bloomIntensity.value > 0 || (beautify.lensDirtIntensity.value > 0 && beautify.anamorphicFlaresIntensity.value <= 0)) {

                        BeautifySettings.bloomExcludeMask = beautify.bloomExcludeLayers.value ? (int)beautify.bloomExclusionLayerMask.value : 0;

                        int size = (int)(Mathf.Lerp(512, sourceDesc.width, beautify.bloomResolution.value / 10f) / 4f) * 4;
                        RenderTextureDescriptor bloomDesc = sourceDescHP;
                        for (int k = 0; k <= mipCount; k++) {
                            rt[k].width = size;
                            rt[k].height = Mathf.Max(1, (int)(size * aspectRatio));
                            bloomDesc.width = rt[k].width;
                            bloomDesc.height = rt[k].height;
                            cmd.GetTemporaryRT(rt[k].rtDown, bloomDesc, FilterMode.Bilinear);
                            cmd.GetTemporaryRT(rt[k].rtUp, bloomDesc, FilterMode.Bilinear);
                            size /= downsamping;
                        }

                        rtBloom = rt[0].rtDown;
                        if (beautify.bloomAntiflicker.value) {
                            cmd.Blit(source, rtBloom, bMat, (int)Pass.BloomLuminanceAntiflicker);
                        } else {
                            cmd.Blit(source, rtBloom, bMat, (int)Pass.BloomLuminance);
                        }

                        // Blitting down...
                        for (int k = 0; k < mipCount; k++) {
                            cmd.Blit(rt[k].rtDown, rt[k + 1].rtDown, bMat, (int)Pass.BloomResample);
                            BlurThis(cmd, bloomDesc, rt[k + 1].rtDown, rt[k + 1].width, rt[k + 1].height, bMat);
                        }
                        if (beautify.bloomIntensity.value > 0 || beautify.lensDirtIntensity.value > 0) {
                            // Blitting up...
                            rtBloom = rt[mipCount].rtDown;
                            for (int k = mipCount; k > 0; k--) {
                                cmd.SetGlobalTexture(ShaderParams.bloomTex, rt[k - 1].rtDown);
                                cmd.Blit(rtBloom, rt[k - 1].rtUp, bMat, (int)Pass.BloomResampleAndCombine);
                                rtBloom = rt[k - 1].rtUp;
                            }
                            if (beautify.bloomCustomize.value) {
                                cmd.SetGlobalTexture(ShaderParams.bloomTex4, rt[4].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex3, rt[3].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex2, rt[2].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex1, rt[1].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex, rt[0].rtUp);
                                bloomDesc.width = rt[0].width;
                                bloomDesc.height = rt[0].height;
                                cmd.GetTemporaryRT(ShaderParams.tempBloomCustomComposeRT, bloomDesc, FilterMode.Bilinear);
                                rtBloom = ShaderParams.tempBloomCustomComposeRT;
                                cmd.Blit(rt[mipCount].rtUp, rtBloom, bMat, (int)Pass.BloomCompose);
                            }
                        }
                    }

                    // anamorphic flares
                    if (beautify.anamorphicFlaresIntensity.value > 0) {

                        int sizeAF = (int)(Mathf.Lerp(512, sourceDescHP.width, beautify.anamorphicFlaresResolution.value / 10f) / 4f) * 4;

                        RenderTextureDescriptor afDesc = sourceDescHP;
                        float spread = beautify.anamorphicFlaresSpread.value * sizeAF / 512f;
                        for (int origSize = sizeAF, k = 0; k <= mipCount; k++) {
                            int w = Mathf.Max(1, (int)(sizeAF * aspectRatio / spread));
                            if (beautify.anamorphicFlaresVertical.value) {
                                rtAF[k].width = origSize;
                                rtAF[k].height = w;
                            } else {
                                rtAF[k].width = w;
                                rtAF[k].height = origSize;
                            }
                            afDesc.width = rtAF[k].width;
                            afDesc.height = rtAF[k].height;
                            cmd.GetTemporaryRT(rtAF[k].rtDown, afDesc, FilterMode.Bilinear);
                            cmd.GetTemporaryRT(rtAF[k].rtUp, afDesc, FilterMode.Bilinear);
                            sizeAF /= downsamping;
                        }

                        if (beautify.anamorphicFlaresAntiflicker.value) {
                            cmd.Blit(source, rtAF[0].rtDown, bMat, (int)Pass.AnamorphicFlaresLuminanceAntiflicker);
                        } else {
                            cmd.Blit(source, rtAF[0].rtDown, bMat, (int)Pass.AnamorphicFlaresLuminance);
                        }

                        BlurThisOneDirection(cmd, afDesc, ref rtAF[0].rtDown, rtAF[0].width, rtAF[0].height, beautify.anamorphicFlaresVertical.value);

                        for (int k = 0; k < mipCount; k++) {
                            cmd.Blit(rtAF[k].rtDown, rtAF[k + 1].rtDown, bMat, (int)Pass.BloomResample);
                            BlurThisOneDirection(cmd, afDesc, ref rtAF[k + 1].rtDown, rtAF[k + 1].width, rtAF[k + 1].height, beautify.anamorphicFlaresVertical.value);
                        }

                        int last = rtAF[mipCount].rtDown;
                        for (int k = mipCount; k > 0; k--) {
                            cmd.SetGlobalTexture(ShaderParams.bloomTex, rtAF[k].rtDown);
                            if (k == 1) {
                                cmd.Blit(last, rtAF[k - 1].rtUp, bMat, (int)Pass.AnamorphicFlaresResample); // applies intensity in last stage
                            } else {
                                cmd.Blit(last, rtAF[k - 1].rtUp, bMat, (int)Pass.BloomResampleAndCombine);
                            }
                            last = rtAF[k - 1].rtUp;
                        }
                        if (beautify.bloomIntensity.value > 0) {
                            if (beautify.lensDirtIntensity.value > 0) {
                                BlendOneOne(cmd, rtAF[lensDirtSpread].rtUp, ref rt[lensDirtSpread].rtUp, ref rt[lensDirtSpread].rtDown);
                            }
                            BlendOneOne(cmd, rtAF[0].rtUp, ref rtBloom, ref rt[0].rtDown);
                        } else {
                            rtBloom = rtAF[0].rtUp;
                        }
                    }

                    if (sunFlareEnabled) {
                        // check if Sun is visible
                        Vector3 sunWorldPosition = cam.transform.position - sceneSettings.sun.transform.forward * 1000f;
                        float flareIntensity = 0;
                        Vector3 sunScrPos = cam.WorldToViewportPoint(sunWorldPosition, sourceDesc.vrUsage == VRTextureUsage.TwoEyes ? Camera.MonoOrStereoscopicEye.Left : Camera.MonoOrStereoscopicEye.Mono);
                        bool sunVisible = sunScrPos.z > 0 && sunScrPos.x >= -0.1f && sunScrPos.x < 1.1f && sunScrPos.y >= -0.1f && sunScrPos.y < 1.1f;
                        if (sunVisible) {
                            if (sourceDesc.vrUsage == VRTextureUsage.TwoEyes) {
                                Vector3 sunScrPosRightEye = cam.WorldToViewportPoint(sunWorldPosition, Camera.MonoOrStereoscopicEye.Right);
                                bMat.SetVector(ShaderParams.sfSunPosRightEye, sunScrPosRightEye);
                                sunVisible = sunScrPosRightEye.z > 0 && sunScrPosRightEye.x >= -0.1f && sunScrPosRightEye.x < 1.1f && sunScrPosRightEye.y >= -0.1f && sunScrPosRightEye.y < 1.1f;
                            }
                            if (sunVisible) {
                                Vector2 dd = sunScrPos - Vector3.one * 0.5f;
                                flareIntensity = beautify.sunFlaresIntensity.value * Mathf.Clamp01((0.6f - Mathf.Max(Mathf.Abs(dd.x), Mathf.Abs(dd.y))) / 0.6f);
                                if (beautify.bloomIntensity.value <= 0 && beautify.anamorphicFlaresIntensity.value <= 0) { // ensure _Bloom.x is 1 into the shader for sun flares to be visible if no bloom nor anamorphic flares are enabled
                                    bMat.SetVector(ShaderParams.bloom, Vector4.one);
                                } else {
                                    flareIntensity /= (beautify.bloomIntensity.value + 0.0001f);
                                }
                            }
                        }
                        sunFlareCurrentIntensity = Mathf.Lerp(sunFlareCurrentIntensity, flareIntensity, Application.isPlaying ? 0.5f : 1f);
                        if (sunFlareCurrentIntensity > 0) {
                            if (flareIntensity > 0) {
                                sunLastScrPos = sunScrPos;
                            }
                            bMat.SetColor(ShaderParams.sfSunTintColor, beautify.sunFlaresTint.value * sunFlareCurrentIntensity);
                            sunLastScrPos.z = 0.5f + sunFlareTime * beautify.sunFlaresSolarWindSpeed.value;
                            Vector2 sfDist = new Vector2(0.5f - sunLastScrPos.y, sunLastScrPos.x - 0.5f);
                            if (!beautify.sunFlaresRotationDeadZone.value || sfDist.sqrMagnitude > 0.00025f) {
                                sunLastRot = Mathf.Atan2(sfDist.x, sfDist.y);
                            }
                            sunLastScrPos.w = sunLastRot;
                            sunFlareTime += Time.deltaTime;
                            bMat.SetVector(ShaderParams.sfSunPos, sunLastScrPos);

                            RenderTextureDescriptor sfDesc = sourceDesc;
                            sfDesc.width /= beautify.sunFlaresDownsampling.value;
                            sfDesc.height /= beautify.sunFlaresDownsampling.value;
                            cmd.GetTemporaryRT(ShaderParams.sfRT, sfDesc, FilterMode.Bilinear);
                            if (rtBloom >= 0) {
                                cmd.Blit(rtBloom, ShaderParams.sfRT, bMat, (int)Pass.SunFlaresAdditive);
                            } else {
                                cmd.Blit(source, ShaderParams.sfRT, bMat, (int)Pass.SunFlares);
                            }
                            if (beautify.lensDirtIntensity.value > 0 && beautify.bloomIntensity.value > 0) {
                                BlendOneOne(cmd, ShaderParams.sfRT, ref rt[lensDirtSpread].rtUp, ref rt[lensDirtSpread].rtDown);
                            }
                            rtBloom = ShaderParams.sfRT;
                        }
                    }
                    if (rtBloom >= 0) {
                        cmd.SetGlobalTexture(ShaderParams.bloomTex, rtBloom);
                    } else {
                        bMat.DisableKeyword(SKW_BLOOM);
                    }

                    if (beautify.lensDirtIntensity.value > 0) {
                        int rtID = (beautify.anamorphicFlaresIntensity.value > 0 && beautify.bloomIntensity.value <= 0) ? rtAF[lensDirtSpread].rtUp : rt[lensDirtSpread].rtUp;
                        cmd.SetGlobalTexture(ShaderParams.screenLum, rtID);
                    }

                }
            }

            void BlendOneOne(CommandBuffer cmd, int source, ref int destination, ref int tempBuffer) {
                cmd.SetGlobalTexture(ShaderParams.afCombineTex, destination); // _BloomTex used as temporary rt for combining
                cmd.Blit(source, tempBuffer, bMat, (int)Pass.AnamorphicFlaresResampleAndCombine);
                // swap buffers
                int tmp = destination;
                destination = tempBuffer;
                tempBuffer = tmp;
            }

            void BlurThis(CommandBuffer cmd, RenderTextureDescriptor desc, int rt, int width, int height, Material blurMat, float blurScale = 1f) {
                desc.width = width;
                desc.height = height;
                blurMat.SetFloat(ShaderParams.blurScale, blurScale);
                cmd.GetTemporaryRT(ShaderParams.tempBlurRT, desc, FilterMode.Bilinear);
                cmd.Blit(rt, ShaderParams.tempBlurRT, blurMat, (int)Pass.BlurHoriz);
                cmd.Blit(ShaderParams.tempBlurRT, rt, blurMat, (int)Pass.BlurVert);
                cmd.ReleaseTemporaryRT(ShaderParams.tempBlurRT);
            }

            void BlurThisOneDirection(CommandBuffer cmd, RenderTextureDescriptor desc, ref int rt, int width, int height, bool vertical) {
                desc.width = width;
                desc.height = height;
                cmd.GetTemporaryRT(ShaderParams.tempBlurOneDirRT, desc, FilterMode.Bilinear);
                cmd.Blit(rt, ShaderParams.tempBlurOneDirRT, bMat, vertical ? (int)Pass.BlurVert : (int)Pass.BlurHoriz);
                int aux = rt;
                rt = ShaderParams.tempBlurOneDirRT;
                ShaderParams.tempBlurOneDirRT = aux;
            }

            void BlurThisDownscaling(CommandBuffer cmd, RenderTextureDescriptor desc, RenderTargetIdentifier source, RenderTargetIdentifier destination, float ratio, Material blurMatPass1, Material blurMatPass2) {
                cmd.GetTemporaryRT(ShaderParams.tempBlurDownscaling, desc, FilterMode.Bilinear);
                cmd.Blit(source, ShaderParams.tempBlurDownscaling, blurMatPass1, (int)Pass.BlurHoriz);
                cmd.Blit(ShaderParams.tempBlurDownscaling, destination, blurMatPass2, (int)Pass.BlurVert);
                cmd.ReleaseTemporaryRT(ShaderParams.tempBlurDownscaling);
            }

            void DoDoF(CommandBuffer cmd) {

                Camera cam = cameraData.camera;
                if (cam.cameraType != CameraType.Game) {
                    bMat.DisableKeyword(SKW_DEPTH_OF_FIELD);
                    return;
                }

                UpdateDepthOfFieldData();

                int width = cam.pixelWidth / beautify.depthOfFieldDownsampling.value;
                int height = cam.pixelHeight / beautify.depthOfFieldDownsampling.value;
                RenderTextureDescriptor dofDesc = sourceDescHP;
                dofDesc.width = width;
                dofDesc.height = height;
                dofDesc.colorFormat = RenderTextureFormat.ARGBHalf;
                cmd.GetTemporaryRT(ShaderParams.dofRT, dofDesc, FilterMode.Bilinear);
                cmd.Blit(source, ShaderParams.dofRT, bMat, (int)Pass.DoFCoC);

                if (beautify.depthOfFieldForegroundBlur.value && beautify.depthOfFieldForegroundBlurHQ.value) {
                    BlurThisAlpha(cmd, dofDesc, ShaderParams.dofRT);
                }

                Pass pass = beautify.depthOfFieldBokeh.value ? Pass.DoFBlur : Pass.DoFBlurWithoutBokeh;
                BlurThisDoF(cmd, dofDesc, ShaderParams.dofRT, (int)pass);
                cmd.SetGlobalTexture(ShaderParams.dofRT, ShaderParams.dofRT);
            }

            Material bMatDoFPass2, bMatDoFPass3;
            void BlurThisDoF(CommandBuffer cmd, RenderTextureDescriptor dofDesc, int rt, int renderPass) {
                cmd.GetTemporaryRT(ShaderParams.dofTempBlurDoFTemp1RT, dofDesc, beautify.depthOfFieldFilterMode.value);
                cmd.GetTemporaryRT(ShaderParams.dofTempBlurDoFTemp2RT, dofDesc, beautify.depthOfFieldFilterMode.value);

                UpdateDepthOfFieldBlurData(bMat, new Vector2(0.44721f, -0.89443f));
                cmd.Blit(rt, ShaderParams.dofTempBlurDoFTemp1RT, bMat, renderPass);

                if (bMatDoFPass2 == null) {
                    bMatDoFPass2 = Instantiate(bMat);
                    bMatDoFPass3 = Instantiate(bMat);
                }
                UpdateDepthOfFieldBlurData(bMatDoFPass2, new Vector2(-1f, 0f));
                cmd.Blit(ShaderParams.dofTempBlurDoFTemp1RT, ShaderParams.dofTempBlurDoFTemp2RT, bMatDoFPass2, renderPass);

                UpdateDepthOfFieldBlurData(bMatDoFPass3, new Vector2(0.44721f, 0.89443f));
                cmd.Blit(ShaderParams.dofTempBlurDoFTemp2RT, rt, bMatDoFPass3, renderPass);

                cmd.ReleaseTemporaryRT(ShaderParams.dofTempBlurDoFTemp2RT);
                cmd.ReleaseTemporaryRT(ShaderParams.dofTempBlurDoFTemp1RT);
            }


            void BlurThisAlpha(CommandBuffer cmd, RenderTextureDescriptor dofDesc, int rt) {
                cmd.GetTemporaryRT(ShaderParams.dofTempBlurDoFAlphaRT, dofDesc, FilterMode.Bilinear);
                cmd.Blit(rt, ShaderParams.dofTempBlurDoFAlphaRT, bMat, (int)Pass.DoFBlurHorizontally);
                cmd.Blit(ShaderParams.dofTempBlurDoFAlphaRT, rt, bMat, (int)Pass.DoFBlurVertically);
                cmd.ReleaseTemporaryRT(ShaderParams.dofTempBlurDoFAlphaRT);
            }

            void UpdateDepthOfFieldBlurData(Material bMat, Vector2 blurDir) {
                float downsamplingRatio = 1f / (float)beautify.depthOfFieldDownsampling.value;
                blurDir *= downsamplingRatio;
                dofLastBokehData.z = blurDir.x;
                dofLastBokehData.w = blurDir.y;
                bMat.SetVector(ShaderParams.dofBokehData, dofLastBokehData);
            }

            void DoVignette() {
                float outerRing = 1f - beautify.vignettingOuterRing.value;
                float innerRing = 1f - beautify.vignettingInnerRing.value;
                bool vignettingEnabled = outerRing < 1 || innerRing < 1f || beautify.vignettingFade.value > 0 || beautify.vignettingBlink.value > 0;
                if (vignettingEnabled) {
                    Color vignettingColorAdjusted = beautify.vignettingColor.value;
                    float vb = 1f - beautify.vignettingBlink.value * 2f;
                    if (vb < 0) vb = 0;
                    vignettingColorAdjusted.r *= vb;
                    vignettingColorAdjusted.g *= vb;
                    vignettingColorAdjusted.b *= vb;
                    vignettingColorAdjusted.a = outerRing;
                    bMat.SetColor(ShaderParams.vignette, vignettingColorAdjusted);
                    Camera cam = cameraData.camera;
                    bMat.SetFloat(ShaderParams.vignetteAspectRatio, (beautify.vignettingCircularShape.value && beautify.vignettingBlink.value <= 0) ? 1.0f / cam.aspect : beautify.vignettingAspectRatio.value + 1.001f / (1.001f - beautify.vignettingBlink.value) - 1f);
                }
            }

            void DoEyeAdaptation(CommandBuffer cmd) {

                int sizeEA = (int)Mathf.Pow(2, rtEA.Length);

                RenderTextureDescriptor eaDesc = sourceDescHP;
                for (int k = 0; k < rtEA.Length; k++) {
                    eaDesc.width = eaDesc.height = sizeEA;
                    cmd.GetTemporaryRT(rtEA[k], eaDesc, FilterMode.Bilinear);
                    sizeEA /= 2;
                }

                cmd.Blit(source, rtEA[0], bMat, (int)Pass.CopyBilinear);

                int lumRT = rtEA.Length - 1;
                for (int k = 0; k < lumRT; k++) {
                    cmd.Blit(rtEA[k], rtEA[k + 1], bMat, k == 0 ? (int)Pass.ComputeScreenLum : (int)Pass.DownsampleScreenLum);
                }
                cmd.SetGlobalTexture(ShaderParams.eaLumSrc, rtEA[lumRT]);
                if (rtEAacum == null) {
                    RenderTextureDescriptor rtEASmallDesc = sourceDescHP;
                    rtEASmallDesc.width = rtEASmallDesc.height = 2;
                    rtEAacum = new RenderTexture(rtEASmallDesc);
                    rtEAacum.Create();
                    cmd.Blit(rtEA[lumRT], rtEAacum, bMat, (int)Pass.CopyExact);
                    rtEAHist = new RenderTexture(rtEASmallDesc);
                    rtEAHist.Create();
                    cmd.Blit(rtEAacum, rtEAHist, bMat, (int)Pass.CopyExact);
                } else {
                    //rtEAacum.MarkRestoreExpected();
                    cmd.Blit(rtEA[lumRT], rtEAacum, bMat, (int)Pass.BlendScreenLum);
                    cmd.Blit(rtEAacum, rtEAHist, bMat, (int)Pass.SimpleBlendLum);
                }
                cmd.SetGlobalTexture(ShaderParams.eaHist, rtEAHist);
            }

            Vector3 camPrevForward, camPrevPos;
            float currSens;

            void UpdateMaterialProperties() {

                Camera cam = cameraData.camera;
                if (cam == null) return;

                CheckCameraDepthTextureMode(cam);

                keywords.Clear();

                // Compute motion sensibility
                float sharpenIntensity = beautify.sharpenIntensity.value;
                bool usesSharpen = sharpenIntensity > 0 && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifySharpen.value);
                if (usesSharpen) {
                    keywords.Add(SKW_SHARPEN);
                    float tempSharpen = sharpenIntensity;
                    float sensibility = beautify.sharpenMotionSensibility.value;
                    if (sensibility > 0) {
                        Vector3 camCurForward = cam.transform.forward;
                        Vector3 camCurPos = cam.transform.position;
                        float angleDiff = Vector3.Angle(camPrevForward, camCurForward) * sensibility;
                        float posDiff = (camCurPos - camPrevPos).sqrMagnitude * 10f * sensibility;
                        float diff = angleDiff + posDiff;
                        if (diff > 0.1f) {
                            camPrevForward = camCurForward;
                            camPrevPos = camCurPos;
                            if (diff > sensibility) {
                                diff = sensibility;
                            }
                            currSens += diff;
                            float min = sharpenIntensity * sensibility * 0.75f;
                            float max = sharpenIntensity * (1f + sensibility) * 0.5f;
                            currSens = Mathf.Clamp(currSens, min, max);
                        } else {
                            currSens *= 0.75f;
                        }
                        tempSharpen = Mathf.Clamp(sharpenIntensity - currSens, 0, sharpenIntensity);
                    }

                    bMat.SetVector(ShaderParams.sharpen, new Vector4(tempSharpen, beautify.sharpenDepthThreshold.value + 0.000001f, beautify.sharpenClamp.value, beautify.sharpenRelaxation.value));
                }

                bool isOrtho = cam.orthographic;
                bool linearColorSpace = QualitySettings.activeColorSpace == ColorSpace.Linear;

                bMat.SetVector(ShaderParams.colorParams, new Vector4(beautify.sepia.value, beautify.daltonize.value, (beautify.sharpenMinMaxDepth.value.x + beautify.sharpenMinMaxDepth.value.y) * 0.5f, Mathf.Abs(beautify.sharpenMinMaxDepth.value.y - beautify.sharpenMinMaxDepth.value.x) * 0.5f + (isOrtho ? 1000.0f : 0f)));

                float contrast = linearColorSpace ? 1.0f + (beautify.contrast.value - 1.0f) / 2.2f : beautify.contrast.value;
                bMat.SetVector(ShaderParams.colorBoost, new Vector4(beautify.brightness.value, contrast, beautify.saturate.value, beautify.ditherIntensity.value));
                bMat.SetColor(ShaderParams.tintColor, beautify.tintColor.value);

                bMat.SetVector(ShaderParams.colorTemp, new Vector3(beautify.colorTemp.value, beautify.colorTempBlend.value));

                bMat.SetVector(ShaderParams.compareParams, new Vector4(Mathf.Cos(beautify.compareLineAngle.value), Mathf.Sin(beautify.compareLineAngle.value), -Mathf.Cos(beautify.compareLineAngle.value), beautify.compareLineWidth.value));
                bMat.SetColor(ShaderParams.fxColor, new Color(beautify.tonemapExposurePre.value, beautify.tonemapBrightnessPost.value, 0, beautify.lutIntensity.value));

                bMat.SetFloat(ShaderParams.blurScale, 1f);

                // bloom related
                usesBloomAndFlares = false;
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyBloom.value) {
                    bool sunFlareEnabled = false;
                    if (beautify.sunFlaresIntensity.value > 0) {
                        CheckSun();
                        sunFlareEnabled = sceneSettings != null && sceneSettings.sun != null;
                    }

                    if (beautify.lensDirtIntensity.value > 0 || beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || sunFlareEnabled) {

                        float bloomWeightsSum = 0.00001f + beautify.bloomWeight0.value + beautify.bloomWeight1.value + beautify.bloomWeight2.value + beautify.bloomWeight3.value + beautify.bloomWeight4.value + beautify.bloomWeight5.value;
                        bMat.SetVector(ShaderParams.bloomWeights2, new Vector4(beautify.bloomWeight4.value / bloomWeightsSum + beautify.bloomBoost4.value, beautify.bloomWeight5.value / bloomWeightsSum + beautify.bloomBoost5.value, beautify.bloomMaxBrightness.value, bloomWeightsSum));
                        bMat.SetColor(ShaderParams.bloomTint, beautify.bloomTint.value);

                        UpdateMaterialBloomIntensityAndThreshold();
                        if (beautify.bloomIntensity.value > 0 || (beautify.lensDirtIntensity.value > 0 && beautify.anamorphicFlaresIntensity.value <= 0)) {
                            bMat.SetVector(ShaderParams.bloomWeights, new Vector4(beautify.bloomWeight0.value / bloomWeightsSum + beautify.bloomBoost0.value, beautify.bloomWeight1.value / bloomWeightsSum + beautify.bloomBoost1.value, beautify.bloomWeight2.value / bloomWeightsSum + beautify.bloomBoost2.value, beautify.bloomWeight3.value / bloomWeightsSum + beautify.bloomBoost3.value));
                            if (beautify.bloomDepthAtten.value > 0) {
                                keywords.Add(SKW_BLOOM_USE_DEPTH);
                                bMat.SetFloat(ShaderParams.bloomDepthThreshold, beautify.bloomDepthAtten.value);
                            }
                            if (beautify.bloomExcludeLayers.value && beautify.bloomExclusionLayerMask.value > 0) {
                                keywords.Add(SKW_BLOOM_USE_LAYER);
                                bMat.SetFloat(ShaderParams.bloomExclusionZBias, beautify.bloomExclusionZBias.value);
                            }
                        }
                        keywords.Add(SKW_BLOOM);
                        usesBloomAndFlares = true;

                        if (beautify.lensDirtIntensity.value > 0 && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyLensDirt.value)) {
                            Vector4 dirtData = new Vector4(1.0f, beautify.lensDirtIntensity.value * beautify.lensDirtIntensity.value, beautify.lensDirtThreshold.value, Mathf.Max(beautify.bloomIntensity.value, 1f));
                            bMat.SetVector(ShaderParams.dirt, dirtData);
                            Texture tex = beautify.lensDirtTexture.value;
                            if (tex == null) {
                                if (dirtTexture == null) {
                                    dirtTexture = Resources.Load<Texture2D>("Textures/lensDirt") as Texture2D;
                                }
                                tex = dirtTexture;
                            }
                            if (tex != null) {
                                bMat.SetTexture(ShaderParams.dirtTex, tex);
                                keywords.Add(SKW_DIRT);
                            }
                        }
                    }

                    // anamorphic flares related
                    if (beautify.anamorphicFlaresIntensity.value > 0) {
                        usesBloomAndFlares = true;
                        if (beautify.anamorphicFlaresDepthAtten.value > 0) {
                            keywords.Add(SKW_ANAMORPHIC_FLARES_USE_DEPTH);
                            bMat.SetFloat(ShaderParams.afDepthThreshold, beautify.anamorphicFlaresDepthAtten.value);
                        }

                        bMat.SetColor(ShaderParams.afTintColor, beautify.anamorphicFlaresTint.value);
                    }

                    // sun flares related
                    if (sunFlareEnabled) {
                        usesBloomAndFlares = true;
                        bMat.SetVector(ShaderParams.sfSunData, new Vector4(beautify.sunFlaresSunIntensity.value, beautify.sunFlaresSunDiskSize.value, beautify.sunFlaresSunRayDiffractionIntensity.value, beautify.sunFlaresSunRayDiffractionThreshold.value));
                        bMat.SetVector(ShaderParams.sfCoronaRays1, new Vector4(beautify.sunFlaresCoronaRays1Length.value, Mathf.Max(beautify.sunFlaresCoronaRays1Streaks.value / 2f, 1), Mathf.Max(beautify.sunFlaresCoronaRays1Spread.value, 0.0001f), beautify.sunFlaresCoronaRays1AngleOffset.value));
                        bMat.SetVector(ShaderParams.sfCoronaRays2, new Vector4(beautify.sunFlaresCoronaRays2Length.value, Mathf.Max(beautify.sunFlaresCoronaRays2Streaks.value / 2f, 1), Mathf.Max(beautify.sunFlaresCoronaRays2Spread.value, 0.0001f), beautify.sunFlaresCoronaRays2AngleOffset.value));
                        bMat.SetVector(ShaderParams.sfGhosts1, new Vector4(0, beautify.sunFlaresGhosts1Size.value, beautify.sunFlaresGhosts1Offset.value, beautify.sunFlaresGhosts1Brightness.value));
                        bMat.SetVector(ShaderParams.sfGhosts2, new Vector4(0, beautify.sunFlaresGhosts2Size.value, beautify.sunFlaresGhosts2Offset.value, beautify.sunFlaresGhosts2Brightness.value));
                        bMat.SetVector(ShaderParams.sfGhosts3, new Vector4(0, beautify.sunFlaresGhosts3Size.value, beautify.sunFlaresGhosts3Offset.value, beautify.sunFlaresGhosts3Brightness.value));
                        bMat.SetVector(ShaderParams.sfGhosts4, new Vector4(0, beautify.sunFlaresGhosts4Size.value, beautify.sunFlaresGhosts4Offset.value, beautify.sunFlaresGhosts4Brightness.value));
                        bMat.SetVector(ShaderParams.sfHalo, new Vector3(beautify.sunFlaresHaloOffset.value, beautify.sunFlaresHaloAmplitude.value, beautify.sunFlaresHaloIntensity.value * 100f));
                        if (flareTex == null) {
                            flareTex = Resources.Load<Texture2D>("Textures/flareNoise") as Texture2D;
                        }
                        bMat.SetTexture(ShaderParams.sfFlareTex, flareTex);
                    }
                }

                // DoF
                usesDepthOfField = false;
                if (beautify.depthOfField.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyDoF.value)) {
                    keywords.Add(SKW_DEPTH_OF_FIELD);
                    usesDepthOfField = true;
                    //if (settings.depthOfFieldTransparencySupport.value || settings.depthOfFieldExclusionLayerMask.value != 0) {
                    //    sheet.EnableKeyword(SKW_DEPTH_OF_FIELD_TRANSPARENT);
                    //}
                }

                // Vignette
                usesVignetting = false;
                float outerRing = 1f - beautify.vignettingOuterRing.value;
                float innerRing = 1f - beautify.vignettingInnerRing.value;
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyVignetting.value) {
                    usesVignetting = outerRing < 1 || innerRing < 1f || beautify.vignettingFade.value > 0 || beautify.vignettingBlink.value > 0;
                    if (innerRing >= outerRing) {
                        innerRing = outerRing - 0.0001f;
                    }
                    if (usesVignetting) {
                        if (beautify.vignettingMask.value != null) {
                            bMat.SetTexture(ShaderParams.vignetteMask, beautify.vignettingMask.value);
                            keywords.Add(SKW_VIGNETTING_MASK);
                        } else {
                            keywords.Add(SKW_VIGNETTING);
                        }
                    }
                }

                // Purkinje and vignetting data
                bool usesPurkinje = beautify.purkinje.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyPurkinje.value);
                if (usesPurkinje || usesVignetting) {
                    float vd = beautify.vignettingFade.value + beautify.vignettingBlink.value * 0.5f;
                    if (beautify.vignettingBlink.value > 0.99f) vd = 1f;
                    Vector4 purkinjeData = new Vector4(beautify.purkinjeAmount.value, beautify.purkinjeLuminanceThreshold.value, vd, innerRing);
                    bMat.SetVector(ShaderParams.purkinje, purkinjeData);
                    if (beautify.purkinje.value) {
                        keywords.Add(SKW_PURKINJE);
                    }
                }

                // Eye adaptation
                bool usesEyeAdaptation = beautify.eyeAdaptation.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyEyeAdaptation.value);
                requiresLuminanceComputation = Application.isPlaying && (usesEyeAdaptation || usesPurkinje);
                if (requiresLuminanceComputation) {
                    Vector4 eaData = new Vector4(beautify.eyeAdaptationMinExposure.value, beautify.eyeAdaptationMaxExposure.value, beautify.eyeAdaptationSpeedToDark.value, beautify.eyeAdaptationSpeedToLight.value);
                    bMat.SetVector(ShaderParams.eaParams, eaData);
                    if (usesEyeAdaptation) {
                        keywords.Add(SKW_EYE_ADAPTATION);
                    }
                }

                // Outline
                if (beautify.outline.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyOutline.value)) {
                    keywords.Add(SKW_OUTLINE);
                    Color color = beautify.outlineColor.value;
                    color.a = 1f - beautify.outlineThreshold.value;
                    bMat.SetColor(ShaderParams.outline, color);
                }

                // Color tweaks
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyColorTweaks.value) {
                    if (beautify.sepia.value > 0 || beautify.daltonize.value > 0 || beautify.colorTempBlend.value > 0) {
                        keywords.Add(SKW_COLOR_TWEAKS);
                    }
                }

                // ACES Tonemapping
                if (beautify.tonemap.value == Beautify.TonemapOperator.ACES && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyTonemapping.value)) {
                    keywords.Add(SKW_TONEMAP_ACES);
                }

                // LUT or Nightvision
                if (beautify.lut.value && beautify.lutTexture.value != null && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyLUT.value)) {
                    bMat.SetTexture(ShaderParams.lutTex, beautify.lutTexture.value);
                    keywords.Add(SKW_LUT);
                } else if (beautify.nightVision.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyNightVision.value)) {
                    keywords.Add(SKW_NIGHT_VISION);
                    Color nightVisionAdjusted = beautify.nightVisionColor.value;
                    if (linearColorSpace) {
                        nightVisionAdjusted.a *= 5.0f * nightVisionAdjusted.a;
                    } else {
                        nightVisionAdjusted.a *= 3.0f * nightVisionAdjusted.a;
                    }
                    nightVisionAdjusted.r *= nightVisionAdjusted.a;
                    nightVisionAdjusted.g *= nightVisionAdjusted.a;
                    nightVisionAdjusted.b *= nightVisionAdjusted.a;
                    bMat.SetColor(ShaderParams.nightVision, nightVisionAdjusted);
                }

                // Dither
                if (beautify.ditherIntensity.value > 0f && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyDithering.value)) {
                    keywords.Add(SKW_DITHER);
                }

                // Best performance mode
                if (beautify.turboMode.value) {
                    keywords.Add(SKW_TURBO);
                }

                int keywordsCount = keywords.Count;
                if (keywordsArray == null || keywordsArray.Length < keywordsCount) {
                    keywordsArray = new string[keywordsCount];
                }
                for (int k = 0; k < keywordsArray.Length; k++) {
                    if (k < keywordsCount) {
                        keywordsArray[k] = keywords[k];
                    } else {
                        keywordsArray[k] = "";
                    }
                }
                bMat.shaderKeywords = keywordsArray;
            }



            void UpdateMaterialBloomIntensityAndThreshold() {
                float bloomThreshold = beautify.bloomThreshold.value;
                float anamorphicThreshold = beautify.anamorphicFlaresThreshold.value;
                if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
                    bloomThreshold *= bloomThreshold;
                    anamorphicThreshold *= anamorphicThreshold;
                }
                float anamorphicFlaresIntensity = beautify.turboMode.value ? beautify.anamorphicFlaresIntensity.value * 2f : beautify.anamorphicFlaresIntensity.value;
                float bloomIntensity = beautify.turboMode.value ? beautify.bloomIntensity.value * 2f : beautify.bloomIntensity.value;
                if (anamorphicFlaresIntensity > 0) {
                    float intensity = anamorphicFlaresIntensity / (bloomIntensity + 0.0001f);
                    bMat.SetVector(ShaderParams.afData, new Vector3(intensity, anamorphicThreshold, 0));
                }
                Vector4 b4 = new Vector4(bloomIntensity + (anamorphicFlaresIntensity > 0 ? 0.0001f : 0f), 0, 0, bloomThreshold);
                bMat.SetVector(ShaderParams.bloom, b4);
            }

            void UpdateDepthOfFieldData() {
                // TODO: get focal length from camera FOV: FOV = 2 arctan (x/2f) x = diagonal of film (0.024mm)
                if (!CheckSceneSettings()) return;
                Camera cam = cameraData.camera;
                float d = beautify.depthOfFieldDistance.value;
                switch ((int)beautify.depthOfFieldFocusMode.value) {
                    case (int)Beautify.DoFFocusMode.AutoFocus:
                        UpdateDoFAutofocusDistance(cam);
                        d = dofLastAutofocusDistance > 0 ? dofLastAutofocusDistance : cam.farClipPlane;
                        BeautifySettings.depthOfFieldCurrentFocalPointDistance = dofLastAutofocusDistance;
                        break;
                    case (int)Beautify.DoFFocusMode.FollowTarget:
                        if (sceneSettings.depthOfFieldTarget != null) {
                            Vector3 spos = cam.WorldToScreenPoint(sceneSettings.depthOfFieldTarget.position);
                            if (spos.z < 0) {
                                d = cam.farClipPlane;
                            } else {
                                d = Vector3.Distance(cam.transform.position, sceneSettings.depthOfFieldTarget.position);
                            }
                        }
                        break;
                }

                if (sceneSettings.OnBeforeFocus != null) {
                    d = sceneSettings.OnBeforeFocus(d);
                }

                dofPrevDistance = Mathf.Lerp(dofPrevDistance, d, Application.isPlaying ? beautify.depthOfFieldFocusSpeed.value * Time.deltaTime * 30f : 1f);
                float dofCoc = beautify.depthOfFieldAperture.value * (beautify.depthOfFieldFocalLength.value / Mathf.Max(dofPrevDistance - beautify.depthOfFieldFocalLength.value, 0.001f)) * (1f / 0.024f);
                dofLastBokehData = new Vector4(dofPrevDistance, dofCoc, 0, 0);
                bMat.SetVector(ShaderParams.dofBokehData, dofLastBokehData);
                bMat.SetVector(ShaderParams.dofBokehData2, new Vector4(beautify.depthOfFieldForegroundBlur.value ? beautify.depthOfFieldForegroundDistance.value : cam.farClipPlane, beautify.depthOfFieldMaxSamples.value, beautify.depthOfFieldBokehThreshold.value, beautify.depthOfFieldBokehIntensity.value * beautify.depthOfFieldBokehIntensity.value));
                bMat.SetVector(ShaderParams.dofBokehData3, new Vector3(beautify.depthOfFieldMaxBrightness.value, beautify.depthOfFieldMaxDistance.value * (cam.farClipPlane + 1f), 0));
            }


            void UpdateDoFAutofocusDistance(Camera cam) {
                Vector3 p = beautify.depthOfFieldAutofocusViewportPoint.value;
                p.z = 10f;
                Ray r = cam.ViewportPointToRay(p);
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, cam.farClipPlane, beautify.depthOfFieldAutofocusLayerMask.value)) {
                    // we don't use hit.distance as ray origin has a small shift from camera
                    float distance = Vector3.Distance(cam.transform.position, hit.point);
                    distance += beautify.depthOfFieldAutofocusDistanceShift.value;
                    dofLastAutofocusDistance = Mathf.Clamp(distance, beautify.depthOfFieldAutofocusMinDistance.value, beautify.depthOfFieldAutofocusMaxDistance.value);
                } else {
                    dofLastAutofocusDistance = cam.farClipPlane;
                }
            }



            // Scene dependant settings
            BeautifySettings sceneSettings;

            void CheckSun() {

                if (!CheckSceneSettings()) return;

                // Fetch a valid Sun reference
                if (sceneSettings.sun == null) {
                    Light[] lights = FindObjectsOfType<Light>();
                    for (int k = 0; k < lights.Length; k++) {
                        Light light = lights[k];
                        if (light.type == LightType.Directional && light.isActiveAndEnabled) {
                            sceneSettings.sun = light.transform;
                            break;
                        }
                    }
                }
            }

            bool CheckSceneSettings() {
                sceneSettings = BeautifySettings.instance;
                return sceneSettings != null;
            }

            void CheckCameraDepthTextureMode(Camera cam) {
                if (beautify.RequiresDepthTexture()) {
                    cam.depthTextureMode |= DepthTextureMode.Depth;
                }
            }

        }


        class BeautifyBloomMaskPass : ScriptableRenderPass {

            readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

            RenderTargetHandle m_Depth;
            Material m_DepthOnlyMaterial = null;

            public BeautifyBloomMaskPass() {
                m_Depth.Init("_BloomSourceDepth");
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
                Shader depthOnly = Shader.Find("Universal Render Pipeline/Unlit");
                if (depthOnly != null) {
                    m_DepthOnlyMaterial = new Material(depthOnly);
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                RenderTextureDescriptor depthDesc = cameraTextureDescriptor;
                depthDesc.colorFormat = RenderTextureFormat.Depth;
                depthDesc.depthBufferBits = 24;
                depthDesc.msaaSamples = 1;
                cmd.GetTemporaryRT(m_Depth.id, depthDesc, FilterMode.Point);
                cmd.SetGlobalTexture("_BloomSourceDepth", m_Depth.Identifier());
                ConfigureTarget(m_Depth.Identifier());
                ConfigureClear(ClearFlag.All, Color.black);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
                drawingSettings.perObjectData = PerObjectData.None;
                drawingSettings.overrideMaterial = m_DepthOnlyMaterial;

                var filter = new FilteringSettings(RenderQueueRange.opaque) { layerMask = BeautifySettings.bloomExcludeMask };
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filter);
            }

            public override void FrameCleanup(CommandBuffer cmd) {
                if (cmd == null) return;
                cmd.ReleaseTemporaryRT(m_Depth.id);
            }
        }


        [SerializeField, HideInInspector]
        Shader shader;
        BeautifyRenderPass m_BeautifyRenderPass;
        BeautifyBloomMaskPass m_BeautifyBloomMaskPass;

        public static bool installed;

        void OnDisable() {
            if (m_BeautifyRenderPass != null) {
                m_BeautifyRenderPass.Cleanup();
            }
            installed = false;
        }


        public override void Create() {
            name = "Beautify";
            m_BeautifyRenderPass = new BeautifyRenderPass();
            m_BeautifyBloomMaskPass = new BeautifyBloomMaskPass();
            installed = true;
        }

        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {

            shader = Shader.Find("Hidden/Kronnect/Beautify");
            if (shader == null) {
                Debug.LogWarning("Could not load Beautify shader. Please make sure BeautifyCore.shader is present.");
                return;
            }

            if (renderingData.cameraData.postProcessEnabled) {
                if (BeautifySettings.bloomExcludeMask > 0) {
                    renderer.EnqueuePass(m_BeautifyBloomMaskPass);
                }
                m_BeautifyRenderPass.Setup(shader, renderer, renderingData);
                renderer.EnqueuePass(m_BeautifyRenderPass);
            }
        }
    }
}
