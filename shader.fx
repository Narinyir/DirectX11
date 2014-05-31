﻿float time : TIME;
float4 col:COLOR;
float4x4 proj:projection;
float4 li:LIGHT;
struct VS_INPUT
{
	float4 position:POSITION;
};

struct PS_INPUT
{
	float4 pos:POSITION;
	float4 position:SV_Position;
};

PS_INPUT VS(VS_INPUT input)
{
	PS_INPUT ps;
	ps.pos = input.position;
	ps.position = mul(input.position,proj);
	return ps;
}

float calcCol(float t, PS_INPUT input)
{
	return abs(sin(input.pos.x + t) - input.pos.y)*exp(input.pos.x);
}

float4 PS(PS_INPUT input) :SV_Target
{
	return col*li;
}

technique10 DefaultTechnique
{
	pass DefaultPass
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}