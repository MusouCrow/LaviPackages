using System;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    class SpriteSubTarget : SubTarget<LaviTarget> {
        private static readonly GUID SOURCE_GUID = new GUID("1cee6872c31e642e7ba7a3142abca5eb"); // SpriteSubTarget.cs
        
        public SpriteSubTarget() {
            this.displayName = "Sprite";
        }

        public override bool IsActive() {
            return true;
        }

        public override void Setup(ref TargetSetupContext context) {
            this.target.surfaceType = SurfaceType.Transparent;

            context.AddAssetDependency(SOURCE_GUID, AssetCollection.Flags.SourceDependency);
            context.AddSubShader(SpritePass.SubShader(this, this.target.RenderType, this.target.RenderQueue));
        }

        public override void GetFields(ref TargetFieldContext context) {

        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context) {
            context.AddBlock(BlockFields.VertexDescription.Position);
            context.AddBlock(BlockFields.VertexDescription.Normal);
            context.AddBlock(BlockFields.VertexDescription.Tangent);

            context.AddBlock(BlockFields.SurfaceDescription.BaseColor);
            context.AddBlock(BlockFields.SurfaceDescription.Alpha);
            context.AddBlock(ShaderPropertyUtil.SurfaceDescription.Glow);
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            this.target.DrawRenderQueueProperty(ref context, onChange, registerUndo);
            this.target.DrawBlendModeProperty(ref context, onChange, registerUndo);
            this.target.DrawCullModeProperty(ref context, onChange, registerUndo);
            this.target.DrawZWriteProperty(ref context, onChange, registerUndo);
            this.target.DrawZTestProperty(ref context, onChange, registerUndo);

            this.target.DrawBlendModeProperty(ref context, onChange, registerUndo, true);
            this.target.DrawCullModeProperty(ref context, onChange, registerUndo, true);
            this.target.DrawZWriteProperty(ref context, onChange, registerUndo, true);
            this.target.DrawZTestProperty(ref context, onChange, registerUndo, true);
        }
    }
}