using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEditor.UIElements;

namespace Koiyun.Render.ShaderGraph.Editor {
    class ToonSubTarget : SubTarget<LaviTarget> {
        private static readonly GUID SOURCE_GUID = new GUID("1cee6872c31e642e7ba7a3142abca5eb"); // LaviSubTarget.cs

        // public bool alphaClip;
        public AlphaClipMode alphaClipMode;
        public bool outlinePass;
        public bool shadowCasterPass;

        public ToonSubTarget() {
            this.displayName = "Toon";
        }

        public override bool IsActive() {
            return true;
        }

        public override void Setup(ref TargetSetupContext context) {
            context.AddAssetDependency(SOURCE_GUID, AssetCollection.Flags.SourceDependency);
            context.AddSubShader(ToonPasses.SubShader(this, this.target.RenderType, this.target.RenderQueue));
        }

        public override void GetFields(ref TargetFieldContext context) {

        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context) {
            context.AddBlock(BlockFields.VertexDescription.Position);
            context.AddBlock(BlockFields.VertexDescription.Normal);
            context.AddBlock(BlockFields.VertexDescription.Tangent);
            
            context.AddBlock(BlockFields.SurfaceDescription.BaseColor);
            context.AddBlock(ShaderPropertyUtil.SurfaceDescription.Glow);

            var surfaceType = this.target.surfaceType;

            if (surfaceType == SurfaceType.Transparent) {
                context.AddBlock(BlockFields.SurfaceDescription.Alpha);
            }
            else if (surfaceType == SurfaceType.Opaque && this.alphaClipMode > AlphaClipMode.None) {
                context.AddBlock(BlockFields.SurfaceDescription.AlphaClipThreshold);
            }

            if (this.outlinePass) {
                context.AddBlock(ShaderPropertyUtil.SurfaceDescription.OutlineColor);
            }
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            this.DrawAlphaClipProperty(ref context, onChange, registerUndo);
            this.DrawOutlinePassProperty(ref context, onChange, registerUndo);
            this.DrawShadowCasterPassProperty(ref context, onChange, registerUndo);
        }

        private void DrawOutlinePassProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var toggle = new Toggle() {value = this.outlinePass};

            context.AddProperty("Outline", toggle, (evt) => {
                if (this.outlinePass == evt.newValue) {
                    return;
                }

                registerUndo("Change Outline");
                this.outlinePass = evt.newValue;
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
    }
}