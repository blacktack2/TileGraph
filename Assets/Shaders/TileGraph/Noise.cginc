static const int p[512] = {
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140,
    36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234,
    75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237,
    149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
    27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105,
    92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73,
    209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
    164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
    147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
    28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101,
    155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232,
    178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
    191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
    181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
    138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215,
    61, 156, 180,
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140,
    36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234,
    75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237,
    149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
    27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105,
    92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73,
    209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
    164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
    147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
    28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101,
    155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232,
    178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
    191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
    181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
    138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215,
    61, 156, 180
};
#define F2 0.366025403;
#define G2 0.211324865;
#define F3 0.333333333;
#define G3 0.166666667;

// Fractal Noise parameters
uint _Octaves;
RWStructuredBuffer<float> _Lacunarity, _Persistence;

float _CentroidThreshold;

#define Fade(t) t * t * t * (t * (t * 6.0 - 15.0) + 10.0)

float Grad(int seed, float pos)
{
    int h = seed & 15;
    float grad = 1.0 + (h & 7);
    if ((h & 8) != 0)
        grad = -grad;
    return grad * pos;
}

float Grad(int seed, float2 pos)
{
    int h = seed & 7;
    float u = h < 4 ? pos.x : pos.y;
    float v = h < 4 ? pos.y : pos.x;
    return ((h & 1) ? u : -u) + ((h & 2) ? 2.0 * v : -2.0 * v);
}

float Grad(int seed, float3 pos)
{
    int h = seed & 15;
    float u = h < 8 ? pos.x : pos.y;
    float v = h < 4 ? pos.y : (h == 12 || h == 14 ? pos.x : pos.z);
    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

float WhiteNoise(float pos)
{
    return frac(Random1D(pos) / (float) MAXINT);
}

float WhiteNoise(float2 pos)
{
    return frac(Random1D(pos) / (float) MAXINT);
}

float WhiteNoise(float3 pos)
{
    return frac(Random1D(pos) / (float) MAXINT);
}

float ValueNoise(float pos)
{
    int posi = floor(pos);
    float posr = frac(pos);

    float r0 = frac((float) Random1D(posi    ) / (float) MAXINT);
    float r1 = frac((float) Random1D(posi + 1) / (float) MAXINT);

    return lerp(r0, r1, Fade(posr)) * 2.0 - 1.0;
}

float ValueNoise(float2 pos)
{
    int2 posi = floor(pos);
    float2 posr = Fade(frac(pos));

    float r00 = frac((float) Random1D(posi               ) / (float) MAXINT);
    float r10 = frac((float) Random1D(posi + float2(1, 0)) / (float) MAXINT);
    float r01 = frac((float) Random1D(posi + float2(0, 1)) / (float) MAXINT);
    float r11 = frac((float) Random1D(posi + float2(1, 1)) / (float) MAXINT);

    float rx0 = lerp(r00, r10, posr.x);
    float rx1 = lerp(r01, r11, posr.x);

    return lerp(rx0, rx1, posr.y) * 2.0 - 1.0;
}

float ValueNoise(float3 pos)
{
    return 0.5;
}

float PerlinNoise(float pos)
{
    return 0.5;
}

float PerlinNoise(float2 pos)
{
    int2 icoords = ((int2) floor(pos)) & 255;
    float2 coords = pos - floor(pos);
    float2 curves = Fade(coords);

    int A = p[icoords.x    ] + icoords.y;
    int B = p[icoords.x + 1] + icoords.y;

    return lerp(lerp(Grad(p[A    ], coords),
                     Grad(p[B    ], coords + float2(-1,  0)), curves.x),
                lerp(Grad(p[A + 1], coords + float2( 0, -1)),
                     Grad(p[B + 1], coords + float2(-1, -1)), curves.x), curves.y);
}

// https://mrl.cs.nyu.edu/~perlin/noise/
float PerlinNoise(float3 pos)
{
    int3 icoords = (int3) floor(pos) & 255;
    float3 coords = pos - floor(pos);
    float3 curves = Fade(coords);

    int A  = p[icoords.x    ] + icoords.y;
    int AA = p[A            ] + icoords.z;
    int AB = p[A + 1        ] + icoords.z;

    int B  = p[icoords.x + 1] + icoords.y;
    int BA = p[B            ] + icoords.z;
    int BB = p[B + 1        ] + icoords.z;

    return lerp(lerp(lerp(Grad(p[AA    ], coords),
                          Grad(p[BA    ], coords + float3(-1,  0,  0)), curves.x),
                     lerp(Grad(p[AB    ], coords + float3( 0, -1,  0)),
                          Grad(p[BB    ], coords + float3(-1, -1,  0)), curves.x), curves.y),
                lerp(lerp(Grad(p[AA + 1], coords + float3( 0,  0, -1)),
                          Grad(p[BA + 1], coords + float3(-1,  0, -1)), curves.x),
                     lerp(Grad(p[AB + 1], coords + float3( 0, -1, -1)),
                          Grad(p[BB + 1], coords + float3(-1, -1, -1)), curves.x), curves.y), curves.z);
}

// https://github.com/WardBenjamin/SimplexNoise/blob/master/SimplexNoise/Noise.cs
float SimplexNoise(float pos)
{
    int i0 = floor(pos);
    int i1 = i0 + 1;
    float x0 = pos - i0;
    float x1 = x0 - 1.0;

    float t0 = 1.0 - x0 * x0;
    t0 *= t0;
    float n0 = t0 * t0 * Grad(p[i0 & 255], x0);

    float t1 = 1.0 - x1 * x1;
    t1 *= t1;
    float n1 = t1 * t1 * Grad(p[i1 & 255], x1);

    return 0.395 * (n0 + n1);
}

float SimplexNoise(float2 pos)
{
    float n0, n1, n2;

    float s = (pos.x + pos.y) * F2;
    float xs = pos.x + s;
    float ys = pos.y + s;
    int i = floor(xs);
    int j = floor(ys);

    float t = (i + j) * G2;
    float X0 = i - t;
    float Y0 = j - t;
    float x0 = pos.x - X0;
    float y0 = pos.y - Y0;

    int i1, j1;
    if (x0 > y0)
    {
        i1 = 1;
        j1 = 0;
    }
    else
    {
        i1 = 0;
        j1 = 1;
    }

    float x1 = x0 - i1 + G2;
    float y1 = y0 - j1 + G2;
    float x2 = x0 - 1.0 + 2.0 * G2;
    float y2 = y0 - 1.0 + 2.0 * G2;

    int ii = i & 255;
    int jj = j & 255;

    float t0 = 0.5 - x0 * x0 - y0 * y0;
    if (t0 < 0.0)
    {
        n0 = 0.0;
    }
    else
    {
        t0 *= t0;
        n0 = t0 * t0 * Grad(p[ii + p[jj]], float2(x0, y0));
    }

    float t1 = 0.5 - x1 * x1 - y1 * y1;
    if (t1 < 0.0)
    {
        n1 = 0.0;
    }
    else
    {
        t1 *= t1;
        n1 = t1 * t1 * Grad(p[ii + i1 + p[jj + j1]], float2(x1, y1));
    }

    float t2 = 0.5 - x2 * x2 - y2 * y2;
    if (t2 < 0.0)
    {
        n2 = 0.0;
    }
    else
    {
        t2 *= t2;
        n2 = t2 * t2 * Grad(p[ii + 1 + p[jj + 1]], float2(x2, y2));
    }

    return 40.0 * (n0 + n1 + n2);
}

float SimplexNoise(float3 pos)
{
    return 0.5f;
}

#define IntNoiseKernel(name)\
[numthreads(8, 1, 1)]\
void name##1D(uint3 id: SV_DispatchThreadID)\
{\
    SetContTileAt(id.xy, frac(name(id.x + _IntOffset.x)));\
}\
[numthreads(8, 8, 1)]\
void name##2D(uint3 id: SV_DispatchThreadID)\
{\
    SetContTileAt(id.xy, frac(name(id.xy + _IntOffset.xy)));\
}\
[numthreads(8, 8, 8)]\
void name##3D(uint3 id: SV_DispatchThreadID)\
{\
    SetContTileAt(id.xy, frac(name(id + _IntOffset)));\
}

#define NoiseKernel(name)\
[numthreads(8, 1, 1)]\
void name##1D(uint3 id: SV_DispatchThreadID)\
{\
    SetContTileAt(id.xy, 0.5 + name(id.x * _Frequency.x + _Offset.x) * 0.5);\
}\
[numthreads(8, 8, 1)]\
void name##2D(uint3 id: SV_DispatchThreadID)\
{\
    SetContTileAt(id.xy, 0.5 + name(id.xy * _Frequency.xy + _Offset.xy) * 0.5);\
}\
[numthreads(8, 8, 8)]\
void name##3D(uint3 id: SV_DispatchThreadID)\
{\
    SetContTileAt(id.xy, 0.5 + name(id * _Frequency + _Offset) * 0.5);\
}

#define FractalNoiseKernelPart(name, dim, subset)\
void Fractal##name##dim##D(uint3 id: SV_DispatchThreadID)\
{\
    float##dim pos = id.subset * _Frequency.subset + _Offset.subset;\
    float value = name(pos);\
    float totalMax = 1.0;\
    for (int octave = 0; octave < (int) _Octaves - 1; octave++)\
    {\
        value += name(pos * _Lacunarity[octave]) * _Persistence[octave];\
        totalMax += _Persistence[octave];\
    }\
    SetContTileAt(id.xy, 0.5 + (value / totalMax) * 0.5);\
}

#define FractalNoiseKernel(name)\
NoiseKernel(name)\
[numthreads(8, 1, 1)]\
FractalNoiseKernelPart(name, 1, x)\
[numthreads(8, 8, 1)]\
FractalNoiseKernelPart(name, 2, xy)\
[numthreads(8, 8, 8)]\
FractalNoiseKernelPart(name, 3, xyz)\

IntNoiseKernel(WhiteNoise)
FractalNoiseKernel(ValueNoise)
FractalNoiseKernel(PerlinNoise)
FractalNoiseKernel(SimplexNoise)

[numthreads(8, 8, 1)]
void VoronoiNoise2D(uint3 id: SV_DispatchThreadID)
{
    float2 pos = id.xy * _Frequency.xy + _Offset.xy;
    int2 ipos = floor(pos);
    float2 fpos = frac(pos);

    float minDist = _CentroidThreshold;
    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            int2 neighbour = int2(x, y);
            
            float2 p = frac((float2) Random2D(ipos + neighbour) / (float) MAXINT);
            float2 diff = neighbour + p - fpos;
            float dist = length(diff);

            minDist = min(minDist, dist);
        }
    }

    SetContTileAt(id.xy, minDist / _CentroidThreshold);
}
