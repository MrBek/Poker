#ifndef ADVANCEDSS_CG_INCLUDED
#define ADVANCEDSS_CG_INCLUDED

float2 ParallaxOcclusionOffset (
	float3 pViewDir, float3 pViewDirWS, float parallaxHeight, float3 normal,
	float2 uv, sampler2D heightMap, float maxSamples )
{
	//viewDirNorm = normalize( pViewDir );
	//viewDirNorm.z += 0.42;
	
	float2 vParallaxDirection = normalize( pViewDir.xy );
    
    float fLength         = length( pViewDir );
    float fParallaxLength = sqrt( fLength * fLength - pViewDir.z * pViewDir.z ) / pViewDir.z;

    float2 vParallaxOffsetTS = vParallaxDirection * fParallaxLength;
    
    vParallaxOffsetTS *= parallaxHeight;
    
	//#if !defined(SHADER_API_OPENGL)
    float nMinSamples = 6;
    float nMaxSamples = maxSamples;
    int nNumSamples = (int)(lerp( nMinSamples, nMaxSamples, 1-dot( pViewDirWS, normal ) ));
    float fStepSize = 1.0 / (float)nNumSamples;   
    int    nStepIndex = 0;
    //#else
    //float fStepSize = 0.03125;
    //#endif
    float fCurrHeight = 0.0;
    float fPrevHeight = 1.0;
    float2 vTexOffsetPerStep = fStepSize * vParallaxOffsetTS;
    float2 vTexCurrentOffset = uv;
    float  fCurrentBound     = 1.0;
    float  fParallaxAmount   = 0.0;

    float2 pt1 = 0;
    float2 pt2 = 0;
    
    //#if !defined(SHADER_API_OPENGL)
    while ( nStepIndex < nNumSamples )
    {
        vTexCurrentOffset -= vTexOffsetPerStep;
        
        fCurrHeight = tex2Dlod( heightMap, float4(vTexCurrentOffset,0,0)).w;

        fCurrentBound -= fStepSize;

        if ( fCurrHeight > fCurrentBound ) 
        {   
           pt1 = float2( fCurrentBound, fCurrHeight );
           pt2 = float2( fCurrentBound + fStepSize, fPrevHeight );

           nStepIndex = nNumSamples + 1;   //Exit loop
           fPrevHeight = fCurrHeight;
        }
        else
        {
           nStepIndex++;
           fPrevHeight = fCurrHeight;
        }
    }
    //#else
    //
    //float done = 0;
    //float done2 = 0;
    //for(int i=0; i < 32; i++)
    //{
    //    vTexCurrentOffset -= vTexOffsetPerStep;
    //    
    //    //fCurrHeight = tex2Dlod( heightMap, float4(vTexCurrentOffset,0,0)).w;
    //    fCurrHeight = tex2D( heightMap, vTexCurrentOffset).w;
    //
    //    fCurrentBound -= fStepSize;
    //    
    //    done = step( fCurrentBound, fCurrHeight - done2 );
    //    done2 += done * 100;
    //
    //    pt1 += float2( fCurrentBound * done, fCurrHeight * done );
    //    pt2 += float2( (fCurrentBound + fStepSize) * done, fPrevHeight * done );
    //
    //    fPrevHeight = fCurrHeight;
    //}
    //#endif
    
    float fDelta2 = pt2.x - pt2.y;
    float fDelta1 = pt1.x - pt1.y;
      
    float fDenominator = fDelta2 - fDelta1;
    
    if ( fDenominator == 0.0f )
    {
        fParallaxAmount = 0.0f;
    }
    else
    {
        fParallaxAmount = (pt1.x * fDelta2 - pt2.x * fDelta1 ) / fDenominator;
    }
    
    return vParallaxOffsetTS * (1 - fParallaxAmount );
}

#define DEPTH_BIAS
#define BORDER_CLAMP

float2 RelaxedConeStep (
	float3 pViewDir, float parallaxHeight, float2 uv, sampler2D coneMap, float4 clipTiling )
{
	float3 p,v;	
	
	p = float3(uv,0);
	v = normalize(pViewDir*-1);
	
	v.z = abs(v.z);
	
#ifdef DEPTH_BIAS
	float db = 1.0-v.z; db*=db; db*=db; db=1.0-db*db;
	v.xy *= db;
#endif

	v.xy *= parallaxHeight;
	
	const int cone_steps=20;
	const int binary_steps=10;
	
	float3 p0 = p;

	v /= v.z;
	
	float dist = length(v.xy);
	
	for( int i = 0; i < cone_steps; i++ )
	{
		float4 tex = tex2D(coneMap, p.xy);
		
		float height = saturate(tex.w - p.z);
		
		float cone_ratio = tex.z;
		
		p += v * (cone_ratio * height / (dist + cone_ratio));
	}

	v *= p.z*0.5;
	p = p0 + v;

	for( int i = 0; i < binary_steps; i++ )
	{
		float4 tex = tex2D(coneMap, p.xy);
		v *= 0.5;
		if (p.z<tex.w)
			p+=v;
		else
			p-=v;
	}
	
#ifdef BORDER_CLAMP
    clip ( p.xy + clipTiling.zw + 0.01 );
    clip ( step( p.x - 0.01, clipTiling.x ) - 0.9 );
    clip ( step( p.y - 0.01, clipTiling.y ) - 0.9 );
#endif

    return p.xy;
}

float3 ConeStepNormal ( float3 normal )
{
	normal.xy = 2 * normal.xy - 1;
	normal.z = sqrt(1 - normal.x * normal.x - normal.y * normal.y);
	return normal;
}

#endif
