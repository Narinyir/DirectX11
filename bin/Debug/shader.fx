float time : TIME;
Texture2D tex : TEX;
SamplerState mySampler
{
};
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
	ps.position = input.position;
	return ps;
}

float calcCol(float t, PS_INPUT input)
{
	return abs(sin(input.pos.x + t) - input.pos.y)*exp(input.pos.x);
}

float4 PS(PS_INPUT input) :SV_Target
{
	//return float4(calcCol(time + 1.0, input), calcCol(time + 2.0, input), calcCol(time + 3.0, input), 1);
	//return tex.Sample(mySampler, float2(input.pos.x / 2.0 + 1 / 2.0, -input.pos.y / 2.0 + 1 / 2.0));
	float r = sqrt(dot(input.pos.xy,input.pos.xy));
	float theta = atan(input.pos.y / input.pos.x);
	float4 col = tex.Sample(mySampler, float2(input.pos.x*cos(r*time/1000)-input.pos.y*sin(r*time/1000),input.pos.x*sin(r*time/1000)+input.pos.y*(r*time/1000)));
	/*col.rgb = (-input.pos.y / 2.0 ) <= sin(input.pos.x * 10) ?
		length(col.rgb / 1.732).rrr*float3(0.3, 0.59, 0.11)*float3(1,0.5,0) :
			length(col.rgb / 1.732).rrr*float3(0.3, 0.59, 0.11)*(1,0,0.5);*/
	/*float theta = time/1000.0;
	float r = time*length(input.pos.x, input.pos.y) / 1414.0;*/
	//col.rgb = float3(r, theta, 1.0).rrr;
	
	return col;
}

technique10 DefaultTechnique
{
	pass DefaultPass
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}