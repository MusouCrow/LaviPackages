SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    $SurfaceDescriptionInputs.WorldSpaceNormal: float3 unnormalizedNormalWS = input.normalWS;
    $SurfaceDescriptionInputs.WorldSpaceNormal: const float renormFactor = 1.0 / length(unnormalizedNormalWS);

    $SurfaceDescriptionInputs.WorldSpaceBiTangent: float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
    $SurfaceDescriptionInputs.WorldSpaceBiTangent: float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);

    $SurfaceDescriptionInputs.WorldSpaceNormal: output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;
    $SurfaceDescriptionInputs.ObjectSpaceNormal: output.ObjectSpaceNormal = normalize(mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M));
    $SurfaceDescriptionInputs.ViewSpaceNormal: output.ViewSpaceNormal = mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);
    $SurfaceDescriptionInputs.TangentSpaceNormal: output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);

    $SurfaceDescriptionInputs.WorldSpaceTangent: output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
    $SurfaceDescriptionInputs.WorldSpaceBiTangent: output.WorldSpaceBiTangent = renormFactor * bitang;

    $SurfaceDescriptionInputs.ObjectSpaceTangent: output.ObjectSpaceTangent = TransformWorldToObjectDir(output.WorldSpaceTangent);
    $SurfaceDescriptionInputs.ViewSpaceTangent: output.ViewSpaceTangent = TransformWorldToViewDir(output.WorldSpaceTangent);
    $SurfaceDescriptionInputs.TangentSpaceTangent: output.TangentSpaceTangent = float3(1.0f, 0.0f, 0.0f);
    $SurfaceDescriptionInputs.ObjectSpaceBiTangent: output.ObjectSpaceBiTangent = TransformWorldToObjectDir(output.WorldSpaceBiTangent);
    $SurfaceDescriptionInputs.ViewSpaceBiTangent: output.ViewSpaceBiTangent = TransformWorldToViewDir(output.WorldSpaceBiTangent);
    $SurfaceDescriptionInputs.TangentSpaceBiTangent: output.TangentSpaceBiTangent = float3(0.0f, 1.0f, 0.0f);
    $SurfaceDescriptionInputs.WorldSpaceViewDirection: output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
    $SurfaceDescriptionInputs.ObjectSpaceViewDirection: output.ObjectSpaceViewDirection = TransformWorldToObjectDir(output.WorldSpaceViewDirection);
    $SurfaceDescriptionInputs.ViewSpaceViewDirection: output.ViewSpaceViewDirection = TransformWorldToViewDir(output.WorldSpaceViewDirection);
    $SurfaceDescriptionInputs.TangentSpaceViewDirection: float3x3 tangentSpaceTransform = float3x3(output.WorldSpaceTangent, output.WorldSpaceBiTangent, output.WorldSpaceNormal);
    $SurfaceDescriptionInputs.TangentSpaceViewDirection: output.TangentSpaceViewDirection = mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
    $SurfaceDescriptionInputs.WorldSpacePosition: output.WorldSpacePosition = input.positionWS;
    $SurfaceDescriptionInputs.ObjectSpacePosition: output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
    $SurfaceDescriptionInputs.ViewSpacePosition: output.ViewSpacePosition = TransformWorldToView(input.positionWS);
    $SurfaceDescriptionInputs.TangentSpacePosition: output.TangentSpacePosition = float3(0.0f, 0.0f, 0.0f);
    $SurfaceDescriptionInputs.AbsoluteWorldSpacePosition: output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
    $SurfaceDescriptionInputs.WorldSpacePositionPredisplacement: output.WorldSpacePositionPredisplacement = input.positionWS;
    $SurfaceDescriptionInputs.ObjectSpacePositionPredisplacement: output.ObjectSpacePositionPredisplacement = TransformWorldToObject(input.positionWS);
    $SurfaceDescriptionInputs.ViewSpacePositionPredisplacement: output.ViewSpacePositionPredisplacement = TransformWorldToView(input.positionWS);
    $SurfaceDescriptionInputs.TangentSpacePositionPredisplacement: output.TangentSpacePositionPredisplacement = float3(0.0f, 0.0f, 0.0f);
    $SurfaceDescriptionInputs.AbsoluteWorldSpacePositionPredisplacement: output.AbsoluteWorldSpacePositionPredisplacement = GetAbsolutePositionWS(input.positionWS);
    $SurfaceDescriptionInputs.ScreenPosition: output.ScreenPosition = input.screenPosition;
    $SurfaceDescriptionInputs.uv0: output.uv0 = input.texCoord0;
    $SurfaceDescriptionInputs.uv1: output.uv1 = input.texCoord1;
    $SurfaceDescriptionInputs.uv2: output.uv2 = input.texCoord2;
    $SurfaceDescriptionInputs.uv3: output.uv3 = input.texCoord3;
    $SurfaceDescriptionInputs.VertexColor: output.VertexColor = input.color;

    return output;
}