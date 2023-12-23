using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public struct RenderData {
        public Camera camera;
        public CullingResults cullingResults;
        public List<int> mainLightIndexes;
    }

    public enum ShadowResolution {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }

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
        public bool scaling;
        public TextureFormat format;
        public bool global;

        public RenderTargetIdentifier RTI => new RenderTargetIdentifier(this.tid);

        public bool SRGB {
            get {
                return this.format == TextureFormat.LDR || this.format == TextureFormat.HDR;
            }
        }

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

        public RenderTextureDescriptor GetRTD(Camera camera, float scale) {
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

            if (this.scaling) {
                width = Mathf.FloorToInt(width * scale);
                height = Mathf.FloorToInt(height * scale);
            }

            var rtd = new RenderTextureDescriptor(width, height) {
                graphicsFormat = this.GraphicsFormat,
                sRGB = this.SRGB
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