namespace Koiyun.Render.ShaderGraph {
    public enum SurfaceType {Opaque, Transparent}
    public enum BlendMode {Alpha, Additive}
    public enum AlphaClipMode {None, Swtich, Enabled}
    public enum StencilType {
        None = 0,
        Scene = 1
    }
}