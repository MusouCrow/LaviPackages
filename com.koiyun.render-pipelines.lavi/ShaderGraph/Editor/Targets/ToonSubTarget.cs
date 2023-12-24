using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    class ToonSubTarget : SubTarget<LaviTarget> {
        private static readonly GUID SOURCE_GUID = new GUID("1cee6872c31e642e7ba7a3142abca5eb"); // ToonSubTarget.cs

        public AlphaClipMode alphaClipMode;
        public bool shadowCasterPass;
        
        public ToonSubTarget() {
            this.displayName = "Toon";
        }

        public override bool IsActive() {
            return true;
        }

        public override void Setup(ref TargetSetupContext context) {
            this.target.surfaceType = SurfaceType.Opaque;

            context.AddAssetDependency(SOURCE_GUID, AssetCollection.Flags.SourceDependency);
            context.AddSubShader(ToonPass.SubShader(this, this.target.RenderType, this.target.RenderQueue));
        }

        public override void GetFields(ref TargetFieldContext context) {

        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context) {
            context.AddBlock(BlockFields.VertexDescription.Position);
            context.AddBlock(BlockFields.VertexDescription.Normal);
            context.AddBlock(BlockFields.VertexDescription.Tangent);
            
            context.AddBlock(ShaderPropertyUtil.SurfaceDescription.Color);
            context.AddBlock(ShaderPropertyUtil.SurfaceDescription.Glow);
            context.AddBlock(ShaderPropertyUtil.SurfaceDescription.LutUV);

            if (this.alphaClipMode > AlphaClipMode.None) {
                context.AddBlock(BlockFields.SurfaceDescription.AlphaClipThreshold);
            }
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            this.target.DrawCullModeProperty(ref context, onChange, registerUndo);
            this.target.DrawZWriteProperty(ref context, onChange, registerUndo);
            this.target.DrawZTestProperty(ref context, onChange, registerUndo);

            this.DrawAlphaClipProperty(ref context, onChange, registerUndo);
            this.DrawShadowCasterPassProperty(ref context, onChange, registerUndo);

            this.target.DrawCullModeProperty(ref context, onChange, registerUndo, true);
            this.target.DrawZWriteProperty(ref context, onChange, registerUndo, true);
            this.target.DrawZTestProperty(ref context, onChange, registerUndo, true);
        }

        private void DrawAlphaClipProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            if (this.target.surfaceType == SurfaceType.Transparent) {
                return;
            }
            
            var field = new EnumField(AlphaClipMode.None) {value = this.alphaClipMode};

            context.AddProperty("Alpha Clip", field, (evt) => {
                var value = (AlphaClipMode)evt.newValue;

                if (this.alphaClipMode == value) {
                    return;
                }

                registerUndo("Change Alpha Clip");
                this.alphaClipMode = value;
                onChange();
            });
        }

        private void DrawShadowCasterPassProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var toggle = new Toggle() {value = this.shadowCasterPass};

            context.AddProperty("Shadow Caster", toggle, (evt) => {
                if (this.shadowCasterPass == evt.newValue) {
                    return;
                }

                registerUndo("Change Shadow Caster");
                this.shadowCasterPass = evt.newValue;
                onChange();
            });
        }
    }
}