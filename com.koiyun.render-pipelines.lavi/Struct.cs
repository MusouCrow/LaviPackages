using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public struct RenderData {
        public Camera camera;
        public CullingResults cullingResults;
        public RenderTextureDescriptor cameraRTD;
        public List<int> mainLightIndexes;
    }

    public enum ShadowResolution {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }

    public struct RenderTexutreRegister {
        public int tid;
        public Func<RenderTextureDescriptor, RenderTextureDescriptor> RTDHandler;

        public RenderTargetIdentifier RTI => new RenderTargetIdentifier(this.tid);

        public RenderTextureDescriptor HandleRTD(RenderTextureDescriptor rtd) {
            if (RTDHandler != null) {
                return RTDHandler(rtd);
            }

            return rtd;
        }
    }
}