namespace Koiyun.Render.ShaderGraph {
    public enum SurfaceType {Opaque, Transparent}
    public enum BlendMode {Alpha, Additive}
    public enum AlphaClipMode {None, Switch, Enabled}
    public enum RenderQueues {
        Scene = 1000,
        Unit = 2000,
        UnitClip = 2500,
        Effect = 3000,
        UI = 3500
    }

    public enum StencilType {
        None = 0,
        Scene = 1,
        Unit = 2
    }
}