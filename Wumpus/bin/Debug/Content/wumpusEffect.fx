float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 FlashlightPosition;
float3 FlashlightVector;
float3 FlashlightColor = float3(1, 1, 1);

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;
 
float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;
 
float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);
float SpecularIntensity = 1;
float3 ViewVector = float3(1, 0, 0);
float3 ViewPosition;

float Brightness = 0.8f;

float Alpha = 1.0f;

float Cycle = 0;

float Animation = 0;
float Wave = 0;
float WaveHeight = 0.1f;
float WaveLength = 2.0f;
 
texture ModelTexture;
 
//--------------------------- TOON SHADER PROPERTIES ------------------------------
// The color to draw the lines in.  Black is a good default.
float4 LineColor = float4(0, 0, 0, 1);
 
// The thickness of the lines.  This may need to change, depending on the scale of
// the objects you are drawing.
float LineThickness = .02;


sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Anisotropic;
    MagFilter = Linear;
	MipFilter = LINEAR;
	MaxAnisotropy = 8;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler2D mapSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Anisotropic;
    MagFilter = Linear;
	MipFilter = LINEAR;
	MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
};

BlendState AlphaBlendingOn
{
    BlendEnable[0] = TRUE;
    DestBlend = INV_SRC_ALPHA;
    SrcBlend = SRC_ALPHA;
};
 
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};

float FlashlightShading(VertexShaderInput input) {
	float4 norm = float4(input.Normal.x, input.Normal.y, input.Normal.z, 0);
	float4 n = mul(norm, World);
	//normalize(n);

	float light = 1;
	float4 lightPos = float4(FlashlightPosition, 1);
	float4 vertPos = mul(input.Position, World);//PlanetCurve(input.Position);
	float4 lightPosDif = vertPos - lightPos;
	lightPosDif = normalize(lightPosDif);
	float posDif = (0.74f - pow(distance(lightPosDif, FlashlightVector),0.6f));
	float normDif = dot(-lightPosDif, n);
	float depth = 1.0f-(distance(lightPos, vertPos)/32.0f);//saturate((0.7f / (distance(lightPos, vertPos)*0.2f)));
	float shade = saturate(posDif * normDif * depth);//((posDif * normDif) + posDif) / 2.0f);
	return shade;
}
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));

    output.Normal = normal;
 
    output.TextureCoordinate = input.TextureCoordinate;

	float shade = FlashlightShading(input);
	float4 lightColor = float4(shade, shade, shade, 1.0f) * float4(FlashlightColor, 1);
	output.Color = lightColor;

    return output;
}

VertexShaderOutput WaterVertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
	//float4 mapColor = tex2D(mapSampler, float2(input.Position.x + Animation, input.Position.z + Animation));

    float4 worldPosition = mul(input.Position, World);
	float bump = cos(sqrt(pow(worldPosition.x - Wave, 2) + pow(worldPosition.z - Wave, 2)) * WaveLength) * WaveHeight;
	worldPosition += float4(0,bump,0,0);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));

    output.Normal = normal;
 
    output.TextureCoordinate = input.TextureCoordinate;

	float shade = FlashlightShading(input);
	float4 lightColor = float4(shade, shade, shade, 1.0f) * float4(FlashlightColor, 1);
	output.Color = lightColor;

    return output;
}

VertexShaderOutput ShadelessVertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	
	float4 normal = normalize(mul(input.Normal, World));
		
	float4 position = (float4(0,cos(Cycle*2)*0.2f,0,0)-((input.Normal*float4(1,1,1,0))*Animation));
	float4 worldPosition = mul(input.Position+position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Normal = normal;
 
    output.TextureCoordinate = input.TextureCoordinate;

	/*float shade = FlashlightShading(input);
	float4 lightColor = float4(shade, shade, shade, 1.0f) * float4(FlashlightColor, 1);
	output.Color = lightColor;*/
	output.Color = float4(1,1,1,1);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    //textureColor.a = 1;
 
	float4 pos = input.Position;
	
    float4 output = saturate(textureColor * input.Color);//lerp(textureColor, input.Color, input.Color.a) + AmbientColor * AmbientIntensity + specular);
	//output.a = 1;
	return output * float4(Brightness, Brightness, Brightness, 1.0f) * Alpha;
}

float4 WaterPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    //textureColor.a = 1;
 
	//float4 pos = input.Position;
	
    float4 output = saturate(textureColor * input.Color * float4(Brightness, Brightness, Brightness, 1.0f) * Alpha);//lerp(textureColor, input.Color, input.Color.a) + AmbientColor * AmbientIntensity + specular);
	//output.a = 0.5f;
	//output.a = 1;
	return output;//float4(0.0f,0.0f,1.0f,0.5f);
}

float4 ShadelessPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    //textureColor.a = 1;
 
	//float4 pos = input.Position;
	
    float4 output = saturate(textureColor * input.Color);//lerp(textureColor, input.Color, input.Color.a) + AmbientColor * AmbientIntensity + specular);
	//output.a = Alpha;
	return output*Alpha;
}
 
 
technique Generic
{
    pass Pass1
    {
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
		ZWriteEnable = true;

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique Ghost
{
    pass Pass1
    {
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
		ZWriteEnable = true;

        VertexShader = compile vs_2_0 ShadelessVertexShaderFunction();
        PixelShader = compile ps_2_0 ShadelessPixelShaderFunction();
    }
}

technique Water
{
    pass Pass1
    {
		AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;		
		ZWriteEnable = true;

        VertexShader = compile vs_2_0 WaterVertexShaderFunction();
        PixelShader = compile ps_2_0 WaterPixelShaderFunction();
    }
}
