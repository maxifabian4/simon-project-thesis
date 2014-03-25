// Microbes 1.0
// Copyright 2006 Michael Anderson
// December 20, 2006

uniform extern float4x4 worldViewProj : WORLDVIEWPROJECTION;
float4 color;
float blurThreshold = 0.9;

struct VS_OUTPUT
{
    float4 position : POSITION;
    float4 col : TEXCOORD0;
    float2 polar : TEXCOORD1;
};


VS_OUTPUT MyVS(
    float4 Pos  : POSITION, 
    float2 Tex : TEXCOORD0 )
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

	Out.position = mul(Pos,worldViewProj);
    Out.col = Pos;
    Out.polar = Tex;

    return Out;
}


float4 MyPS( float4 col : TEXCOORD0, float2 polar : TEXCOORD1 ) : COLOR0
{
	float theta = polar.x;
	float rho = polar.y;
	
	float sat = 0.8; // saturation
	float invsat = 1 - sat;
	
	float3 colorCur = float3(invsat, invsat, invsat);

	float r = colorCur.r + color.r * rho * sat;
	float g = colorCur.g + color.g * rho * sat;
	float b = colorCur.b + color.b * rho * sat;
	float a;

	// Uncomment the following line to remove edge blurring
	// blurThreshold = 1;
	
	if( rho < blurThreshold )
	{
		a = 1.0f;
	}
	else
	{
		float normrho = (rho - blurThreshold) * 1 / (1 - blurThreshold);
		a = cos(normrho  * 3.14159/2);
	}

	float4 finalColor = float4(r,g,b,a);
	return finalColor;
}


technique MyTechnique
{
    pass MyPass
    {
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
        vertexShader = compile vs_2_0 MyVS();
        pixelShader = compile ps_2_0 MyPS();
    }
}
