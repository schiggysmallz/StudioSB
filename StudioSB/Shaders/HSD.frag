﻿#version 330
in vec3 vertPosition;
in vec3 normal;
in vec3 bitangent;
in vec3 tangent;
in vec4 color;
in vec2 tex0;
in vec3 specularPass;

noperspective in vec3 edgeDistance;

uniform int hasDiffuse;
uniform int diffuseCoordType;
uniform vec2 diffuseScale;
uniform sampler2D diffuseTex;

uniform int hasExt;
uniform int extCoordType;
uniform vec2 extScale;
uniform sampler2D extTex;

uniform int hasBumpMap;
uniform int bumpMapWidth;
uniform int bumpMapHeight;
uniform sampler2D bumpMapTex;
uniform vec2 bumpMapTexScale;

uniform vec4 diffuseColor;
uniform vec4 ambientColor;

uniform int enableDiffuseLighting;

uniform float glossiness;
uniform float transparency;

uniform mat4 mvp;
uniform mat4 sphereMatrix;

uniform int renderDiffuse;
uniform int renderSpecular;

uniform int renderAlpha;

uniform int renderNormalMap;

uniform vec3 cameraPos;

uniform int renderWireframe;

out vec4 fragColor;

// Defined in Wireframe.frag.
float WireframeIntensity(vec3 distanceToEdges);

vec3 CalculateBumpMapNormal(vec3 normal, vec3 tangent, vec3 bitangent,
    int hasBump, sampler2D bumpMap, int width, int height, vec2 texCoords)
{
    if (hasBump != 1)
        return normal;

    // Compute a normal based on difference in height.
    float offsetX = 1.0 / width;
    float offsetY = 1.0 / height;
    float a = texture2D(bumpMap, texCoords).r;
    float b = texture2D(bumpMap, texCoords + vec2(offsetX, 0)).r;
    float c = texture2D(bumpMap, texCoords + vec2(0, offsetY)).r;
    vec3 bumpNormal = normalize(vec3(b-a, c-a, 0.1));

    mat3 tbnMatrix = mat3(tangent, bitangent, normal);
    vec3 newNormal = tbnMatrix * bumpNormal;
    return normalize(newNormal);
}

vec2 GetSphereCoords(vec3 N)
{
    vec3 viewNormal = mat3(sphereMatrix) * normal.xyz;
    return viewNormal.xy * 0.5 + 0.5;
}

vec2 GetCoordType(int coordType, vec2 tex0)
{
	//COORD_REFLECTION
	if(coordType == 1)
		return GetSphereCoords(normal);
	//COORD_UV
	return tex0;
}

// color map pass for the diffuse texture
vec3 ColorMapDiffusePass(vec3 N, vec3 V)
{
    vec4 diffuseMap = vec4(1);

    vec2 diffuseCoords = GetCoordType(diffuseCoordType, tex0);

    if (hasDiffuse == 1)
        diffuseMap = texture(diffuseTex, diffuseCoords * diffuseScale);

    return diffuseMap.rgb;
}

// basic lambert diffuse
vec3 DiffusePass(vec3 N, vec3 V)
{
    float lambert = clamp(dot(N, V), 0, 1);
	
    vec3 diffuseTerm = diffuseColor.rgb * lambert;

	diffuseTerm *= enableDiffuseLighting;

	return diffuseTerm;
}

// This is usally a reflection map
//
vec3 ColorMapExtPass(vec3 N, vec3 V)
{
    vec4 Map = vec4(0);

    vec2 Coords = GetCoordType(extCoordType, tex0);

    if (hasExt== 1)
        Map = texture(extTex, Coords * extScale);

    return Map.rgb;
}

void main()
{
	fragColor = vec4(0, 0, 0, 1);
	
	vec3 V = normalize(vertPosition - cameraPos);
    vec3 N = normal;
    /*if (renderNormalMap == 1)
    {
        // This seems to only affect diffuse.
        N = CalculateBumpMapNormal(normal, tangent, bitangent, hasBumpMap,
            bumpMapTex, bumpMapWidth, bumpMapHeight, tex0  * bumpMapTexScale);
    }*/

	// Render passes
	fragColor.rgb += ambientColor.rgb * ColorMapDiffusePass(N, V) * 0.5;
	fragColor.rgb += DiffusePass(N, V) * ColorMapDiffusePass(N, V) * renderDiffuse;
	fragColor.rgb += specularPass * renderSpecular;//SpecularPass(N, V) * renderSpecular;
	fragColor.rgb += ColorMapExtPass(N, V);

	//fragColor.rgb *= color.rgb;
	
    if (renderWireframe == 1)
    {
        vec3 edgeColor = vec3(1);
        float intensity = WireframeIntensity(edgeDistance);
        fragColor.rgb = mix(fragColor.rgb, edgeColor, intensity);
    }

	// Set alpha
    if (renderAlpha == 1)
        fragColor.a = texture(diffuseTex, tex0 * diffuseScale).a * transparency;
}