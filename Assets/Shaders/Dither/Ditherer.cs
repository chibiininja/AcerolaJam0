using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ditherer : BaseShader {
    [Range(0.0f, 1.0f)]
    public float spread = 0.5f;

    [Range(2, 16)]
    public int redColorCount = 2;
    [Range(2, 16)]
    public int greenColorCount = 2;
    [Range(2, 16)]
    public int blueColorCount = 2;

    [Range(0, 2)]
    public int bayerLevel = 0;

    [Range(0, 8)]
    public int downSamples = 0;

    public bool pointFilterDown = false;
    
    public override void OnRenderImage(RenderTexture source, RenderTexture destination) {
        shaderMaterial.SetFloat("_Spread", spread);
        shaderMaterial.SetInt("_RedColorCount", redColorCount);
        shaderMaterial.SetInt("_GreenColorCount", greenColorCount);
        shaderMaterial.SetInt("_BlueColorCount", blueColorCount);
        shaderMaterial.SetInt("_BayerLevel", bayerLevel);

        int width = source.width;
        int height = source.height;

        RenderTexture[] textures = new RenderTexture[8];

        RenderTexture currentSource = source;

        for (int i = 0; i < downSamples; ++i) {
            width /= 2;
            height /= 2;

            if (height < 2)
                break;

            RenderTexture currentDestination = textures[i] = RenderTexture.GetTemporary(width, height, 0, source.format);

            if (pointFilterDown)
                Graphics.Blit(currentSource, currentDestination, shaderMaterial, 1);
            else
                Graphics.Blit(currentSource, currentDestination);

            currentSource = currentDestination;
        }

        RenderTexture dither = RenderTexture.GetTemporary(width, height, 0, source.format);
        Graphics.Blit(currentSource, dither, shaderMaterial, 0);

        Graphics.Blit(dither, destination, shaderMaterial, 1);
        RenderTexture.ReleaseTemporary(dither);
        for (int i = 0; i < downSamples; ++i) {
            RenderTexture.ReleaseTemporary(textures[i]);
        }
    }
}
