#include "Teapot.hlsli"

struct PSOutput
{
	float4 color : SV_TARGET;
};

// https://en.wikipedia.org/wiki/Blinn-Phong_reflection_model#High-Level_Shading_Language_code_sample
struct Lighting
{
	float3 Diffuse;
	float3 Specular;
};

struct PointLight
{
	float3 position;
	float3 diffuseColor;
	float  diffusePower;
	float3 specularColor;
	float  specularPower;
};

float powToShineFactor( float x )
{
	x = x * x;	// 2
	x = x * x;	// 4
	x = x * x;	// 8
	x = x * x;	// 16
	x = x * x;	// 32
	x = x * x;	// 64
	return x;
}

Lighting computeBlinnPhong( PointLight light, float3 pos3D, float3 viewDir, float3 normal )
{
	Lighting OUT;

	float3 lightDir = light.position - pos3D; // 3D position in space of the surface
	float distance = dot( lightDir, lightDir );
	lightDir /= sqrt( distance );

	// Intensity of the diffuse light. Saturate to keep within the 0-1 range.
	float NdotL = dot( normal, lightDir );
	float intensity = saturate( NdotL );

	// Calculate the diffuse light factoring in light color, power and the attenuation
	OUT.Diffuse = intensity * light.diffuseColor * light.diffusePower / distance;

	// Calculate the half vector between the light vector and the view vector.
	// This is typically slower than calculating the actual reflection vector
	// due to the normalize function's reciprocal square root
	float3 H = normalize( lightDir + viewDir );

	// Intensity of the specular light
	float NdotH = dot( normal, H );
	intensity = powToShineFactor( saturate( NdotH ) );

	// Sum up the specular light factoring
	OUT.Specular = intensity * light.specularColor * light.specularPower / distance;

	return OUT;
}

static const float3 cameraPos = float3( 0.0, 0.0, 0.0 );

void main( in sTeapotVertexOutput src, out PSOutput PSOut )
{
	float3 viewDir = normalize( cameraPos - src.viewPos );
	// When GPU interpolates vectors across a triangle, it uses linear interpolation as opposed to spherical.
	// Consequentially, the interpolated normals are no longer unit length.
	// computeBlinnPhong function from Wikipedia appears to be quite sensitive to that, without re-normalizing here in the PS lightning artifacts are noticeable in the centers of triangles.
	float3 normal = normalize( src.normal );

	PointLight pl;
	pl.position = float3( 3.0, 1.0, 0.0 );
	pl.diffuseColor = float3( 1.0, 1.0, 0.88 );
	pl.diffusePower = 3.0;
	pl.specularColor = float3( 1.0, 0.0, 0.0 );
	pl.specularPower = 11.0;

	Lighting res = computeBlinnPhong( pl, src.viewPos, viewDir, normal );
	float3 color = res.Diffuse + res.Specular;

	pl.position = float3( -3.0, 1.0, 0.0 );
	pl.specularColor = float3( 0.0, 1.0, 0.0 );
	res = computeBlinnPhong( pl, src.viewPos, viewDir, normal );
	color += (res.Diffuse + res.Specular);

	pl.position = float3( 0.0, -2.0, 0.0 );
	pl.specularColor = float3( 0.0, 0.0, 1.0 );
	res = computeBlinnPhong( pl, src.viewPos, viewDir, normal );
	color += ( res.Diffuse + res.Specular );

	PSOut.color = float4( saturate( color ), 1.0 );
}