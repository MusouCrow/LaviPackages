namespace Koiyun.Render.ShaderGraph {
    public enum SurfaceType {Opaque, Transparent}
    public enum BlendMode {Alpha, Additive}
    public enum AlphaClipMode {None, Switch, Enabled}
    public enum StencilType {
        None = 0,
        Scene = 1,
        Unit = 2
    }
}