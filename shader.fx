float time : TIME;
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
	if (input.position.x == 1 && input.position.y == 1){
		input.position.xy *= float2(0.0, 0.0);
	}
	ps.position = input.position*float4(0.8, 0.8, 1, 1);
	
	return ps;
}

float calcCol(float t, PS_INPUT input)
{
	return abs(sin(input.pos.x + t) - input.pos.y)*exp(input.pos.x);
}

float4 PS(PS_INPUT input) :SV_Target
{
	return float4(calcCol(time + 1.0, input), calcCol(time + 2.0, input), calcCol(time + 3.0, input), 1);
}

technique10 DefaultTechnique
{
	pass DefaultPass
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}