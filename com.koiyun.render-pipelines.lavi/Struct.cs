using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public struct RenderData {
        public Camera camera;
        public CullingResults cullingResults;
        public RenderTextureDescriptor cameraRTD;
        public int mainLightIndex;
    }

    public enum ShadowResolution {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }
}