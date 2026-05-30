using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("UberUnity/Compatibility/Fallback Image Effect")]
public class UberUnityFallbackImageEffect : MonoBehaviour
{
    [Header("Bloom And Flares")]
    public int tweakMode;
    public int screenBlendMode;
    public bool hdr;
    public float sepBlurSpread = 2.5f;
    public int quality;
    public float bloomIntensity = 0.5f;
    public float bloomThreshhold = 0.4f;
    public Color bloomThreshholdColor = Color.white;
    public int bloomBlurIterations = 2;
    public int hollywoodFlareBlurIterations = 2;
    public float flareRotation;
    public int lensflareMode = 1;
    public float hollyStretchWidth = 2.5f;
    public float lensflareIntensity;
    public float lensflareThreshhold = 0.3f;
    public float lensFlareSaturation = 0.75f;
    public Color flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);
    public Color flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);
    public Color flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);
    public Color flareColorD = new Color(0.8f, 0.4f, 0f, 0.75f);
    public float blurWidth = 1f;
    public Texture2D lensFlareVignetteMask;
    public Shader lensFlareShader;
    public Shader screenBlendShader;
    public Shader blurAndFlaresShader;
    public Shader brightPassFilterShader;

    [Header("Color Correction")]
    public AnimationCurve redChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve greenChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve blueChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public bool useDepthCorrection;
    public AnimationCurve zCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve depthRedChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve depthGreenChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve depthBlueChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public float saturation = 1f;
    public bool selectiveCc;
    public Color selectiveFromColor = Color.gray;
    public Color selectiveToColor = Color.gray;
    public int mode;
    public bool updateTextures = true;
    public Shader colorCorrectionCurvesShader;
    public Shader simpleColorCorrectionCurvesShader;
    public Shader colorCorrectionSelectiveShader;

    [Header("Sun Shafts")]
    public int resolution = 1;
    public Transform sunTransform;
    public int radialBlurIterations = 1;
    public Color sunColor = Color.white;
    public float sunShaftBlurRadius = 2.5f;
    public float sunShaftIntensity = 1f;
    public float useSkyBoxAlpha = 0.75f;
    public float maxRadius = 0.75f;
    public bool useDepthTexture = true;
    public Shader sunShaftsShader;
    public Shader simpleClearShader;

    [Header("Fog")]
    public int fogMode;
    public float startDistance = 10f;
    public float globalDensity = 0.01f;
    public float heightScale = 1f;
    public float height;
    public Color globalFogColor = Color.gray;
    public Shader fogShader;

    [Header("Contrast")]
    public float intensity = 0.5f;
    public float threshhold;
    public float blurSpread = 1f;
    public Shader separableBlurShader;
    public Shader contrastCompositeShader;

    // These scene components came from missing post-processing scripts.
    // The fallback keeps scenes loadable and simply passes the image through.
    // NOTE: destination can be null — that means "render to screen" (final effect).
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
    }
}
