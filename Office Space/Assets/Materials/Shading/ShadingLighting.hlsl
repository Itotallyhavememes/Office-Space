#ifndef ADDITIONAL_LIGHT_INCLUDED
#define ADDITIONAL_LIGHT_INCLUDED

void MainLight_float(float3 worldPos, out float3 direction, out float3 color, out float attenuation)
{
#ifdef SHADERGRAPH_PREVIEW
    direction = normalize(float3(1.0f, 1.0f, 0.0f));
    color = 1.0f;
    attenuation = 1.0f;
#else
    Light mainLight = GetMainLight();
    direction = mainLight.direction;
    color = mainLight.color;
    attenuation = mainLight.distanceAttenuation;
#endif
}

void AdditionalLight_float(float3 worldPos, int lightID, out float3 direction, out float3 color, out float attenuation)
{
    direction = normalize(float3(1.0f, 1.0f, 0.0f));
    color = 0.0f;
    attenuation = 0.0f;
    
    #ifndef SHADERGRAPH_PREVIEW
    
    int lightCount = GetAdditionalLightsCount();
    if (lightID < lightCount)
    {
        Light light = GetAdditionalLight(lightID, worldPos);
        direction = light.direction;
        color = light.color;
        attenuation = light.distanceAttenuation;
    }
    
    #endif
}

void AllLights_float(float3 worldPos, float3 worldNormal, float2 cutoffThresh, out float3 lightColor)
{
    lightColor = 0.0f;

#ifndef SHADERGRAPH_PREVIEW
    int lightCount = GetAdditionalLightsCount();

    for (int i = 0; i < lightCount; i++)
    {
        Light light = GetAdditionalLight(i, worldPos);

        float3 color = dot(light.direction, worldNormal);
        color = smoothstep(cutoffThresh.x, cutoffThresh.y, color);
        color *= light.color;
        color *= light.distanceAttenuation;

        lightColor += color;
    }
#endif
}



#endif