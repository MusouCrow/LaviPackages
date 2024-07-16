using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Koiyun.Render {
    public struct RenderData {
        public Camera camera;
        public CullingResults cullingResults;
        public List<int> mainLightIndexes;
        public Color backgroundColor;
    }

#if ODIN_INSPECTOR
    public enum ShadowResolution {
        [LabelText("256")]
        _256 = 256,
        [LabelText("512")]
        _512 = 512,
        [LabelText("1024")]
        _1024 = 1024,
        [LabelText("2048")]
        _2048 = 2048,
        [LabelText("4096")]
        _4096 = 4096
    }
#else
    public enum ShadowResolution {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }
#endif

    public enum TextureFormat {
        LDR,
        HDR,
        Normal,
        Shadow,
        Depth
    }

    public struct RenderTexutreRegister {
        public int tid;
        public int size;
        public float scale;
        public TextureFormat format;
        public bool global;
        public bool srgb;

        public RenderTargetIdentifier RTI => new RenderTargetIdentifier(this.tid);

        public GraphicsFormat GraphicsFormat {
            get {
                switch (this.format) {
                    case TextureFormat.LDR:
                        return SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
                    case TextureFormat.HDR:
                        return SystemInfo.GetGraphicsFormat(DefaultFormat.HDR);
                    case TextureFormat.Normal:
                        return GraphicsFormat.R8G8B8A8_SNorm;
                    case TextureFormat.Shadow:
                        return SystemInfo.GetGraphicsFormat(DefaultFormat.Shadow);
                    case TextureFormat.Depth:
                        return SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil);
                    default:
                        return SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
                }
            }
        }

        public RenderTextureDescriptor GetRTD(Camera camera) {
            int width;
            int height;

            if (this.size > 0) {
                width = this.size;
                height = this.size;
            }
            else {
                width = camera.pixelWidth;
                height = camera.pixelHeight;
            }

            width = Mathf.FloorToInt(width * this.scale);
            height = Mathf.FloorToInt(height * this.scale);

            var rtd = new RenderTextureDescriptor(width, height) {
                graphicsFormat = this.GraphicsFormat,
                sRGB = this.srgb
            };

            if (this.format == TextureFormat.Depth) {
                rtd.depthBufferBits = 32;
                rtd.colorFormat = RenderTextureFormat.Depth;
            }
            else if (this.format == TextureFormat.Shadow) {
                rtd.depthBufferBits = 16;
                rtd.colorFormat = RenderTextureFormat.Shadowmap;
            }

            return rtd;
        }
    }
}