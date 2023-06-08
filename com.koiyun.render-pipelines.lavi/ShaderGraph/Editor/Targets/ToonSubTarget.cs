using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    class ToonSubTarget : SubTarget<LaviTarget> {
        private static readonly GUID SOURCE_GUID = new GUID("1cee6872c31e642e7ba7a3142abca5eb"); // LaviSubTarget.cs

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
            context.AddBlock(BlockFields.SurfaceDescription.BaseColor);

            if (this.target.surfaceType == SurfaceType.Transparent) {
                context.AddBlock(BlockFields.SurfaceDescription.Alpha);
            }
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            this.DrawShadowCasterPassProperty(ref context, onChange, registerUndo);
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