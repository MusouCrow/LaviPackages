using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Serialization;

namespace Koiyun.Render.ShaderGraph.Editor {
    class LaviTarget : Target {
        private static readonly GUID SOURCE_GUID = new GUID("796e5b41a09e467439eef237611d7d2d"); // LaviTarget.cs

        private List<SubTarget> subTargets;
        private List<string> subTargetsNames;

        [SerializeField]
        private JsonData<SubTarget> activeSubTarget;

        public SurfaceType surfaceType = SurfaceType.Opaque;
        public BlendMode blendMode = BlendMode.Alpha;
        public CullMode cullMode = CullMode.Back;
        public bool zWrite = true;
        public ZTest zTest = ZTest.LEqual;
        public StencilType stencil = StencilType.None;
        public StencilType occlusion = StencilType.None;
        public CompareFunction stencilComp = CompareFunction.Always;
        public bool ui = false;
        public bool overrideBlendMode;
        public bool overrideCullMode;
        public bool overrideZWrite;
        public bool overrideZTest;
        public string customEditorGUI;

        public string RenderType {
            get {
                if (this.surfaceType == SurfaceType.Opaque) {
                    return $"{UnityEditor.ShaderGraph.RenderType.Opaque}";
                }
                
                return $"{UnityEditor.ShaderGraph.RenderType.Transparent}";
            }
        }

        public string RenderQueue {
            get {
                if (this.surfaceType == SurfaceType.Opaque) {
                    return $"{UnityEditor.ShaderGraph.RenderQueue.Geometry}";
                }

                return $"{UnityEditor.ShaderGraph.RenderQueue.Transparent}";
            }
        }

        public override object saveContext => this.activeSubTarget.value?.saveContext;

        public LaviTarget() {
            this.displayName = "Lavivagnar";
            this.subTargets = TargetUtils.GetSubTargets(this);
            this.subTargetsNames = new List<string>();

            foreach (var t in this.subTargets) {
                this.subTargetsNames.Add(t.displayName);
            }

            TargetUtils.ProcessSubTargetList(ref this.activeSubTarget, ref this.subTargets);
        }

        public override bool IsActive() {
            bool ok = GraphicsSettings.currentRenderPipeline is LaviRenderPipelineAsset;

            return ok && this.activeSubTarget.value.IsActive();
        }

        public override void Setup(ref TargetSetupContext context) {
            context.AddAssetDependency(SOURCE_GUID, AssetCollection.Flags.SourceDependency);

            var gui = this.customEditorGUI != "" ? this.customEditorGUI : typeof(LaviShaderGUI).FullName;
            context.AddCustomEditorForRenderPipeline(gui, typeof(LaviRenderPipelineAsset));

            TargetUtils.ProcessSubTargetList(ref this.activeSubTarget, ref this.subTargets);

            this.activeSubTarget.value.target = this;
            this.activeSubTarget.value.Setup(ref context);
        }

        public override void OnAfterMultiDeserialize(string json) {
            TargetUtils.ProcessSubTargetList(ref this.activeSubTarget, ref this.subTargets);
            this.activeSubTarget.value.target = this;
        }

        public override void GetFields(ref TargetFieldContext context) {
            context.AddField(Fields.GraphVertex);
            context.AddField(Fields.GraphPixel);
            
            this.activeSubTarget.value.GetFields(ref context);
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context) {
            this.activeSubTarget.value.GetActiveBlocks(ref context);
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode) {
            base.CollectShaderProperties(collector, generationMode);

            if (this.overrideBlendMode) {
                this.GetBlend(out var srcBlend, out var dstBlend);
                ShaderPropertyUtil.AddFloatProperty(collector, ShaderGraphConst.SRC_BLEND_PROPERTY, (float)srcBlend);
                ShaderPropertyUtil.AddFloatProperty(collector, ShaderGraphConst.DST_BLEND_PROPERTY, (float)dstBlend);
            }

            if (this.overrideCullMode) {
                ShaderPropertyUtil.AddFloatProperty(collector, ShaderGraphConst.CULL_PROPERTY, (float)this.cullMode);
            }

            if (this.overrideZWrite) {
                ShaderPropertyUtil.AddFloatProperty(collector, ShaderGraphConst.ZWRITE_PROPERTY, this.zWrite ? 1 : 0);
            }

            if (this.overrideZTest) {
                ShaderPropertyUtil.AddFloatProperty(collector, ShaderGraphConst.ZTEST_PROPERTY, (float)this.zTest);
            }

            this.activeSubTarget.value.CollectShaderProperties(collector, generationMode);
        }

        public override void ProcessPreviewMaterial(Material material) {
            if (this.overrideBlendMode) {
                this.GetBlend(out var srcBlend, out var dstBlend);
                material.SetFloat(ShaderGraphConst.SRC_BLEND_PROPERTY, (float)srcBlend);
                material.SetFloat(ShaderGraphConst.DST_BLEND_PROPERTY, (float)dstBlend);

                if (srcBlend == Blend.One && dstBlend == Blend.One) {
                    material.EnableKeyword(ShaderGraphConst.ADDITIVE_KEYWORD);
                }
                else {
                    material.DisableKeyword(ShaderGraphConst.ADDITIVE_KEYWORD);
                }
            }

            if (this.overrideCullMode) {
                material.SetFloat(ShaderGraphConst.CULL_PROPERTY, (float)this.cullMode);
            }

            if (this.overrideZWrite) {
                material.SetFloat(ShaderGraphConst.ZWRITE_PROPERTY, this.zWrite ? 1 : 0);
            }
            
            if (this.overrideZTest) {
                material.SetFloat(ShaderGraphConst.ZTEST_PROPERTY, (float)this.zTest);
            }
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            this.DrawSelectMaterialProperty(ref context, onChange, registerUndo);
            this.activeSubTarget.value.GetPropertiesGUI(ref context, onChange, registerUndo);
            
            this.DrawStencilProperty(ref context, onChange, registerUndo);
            this.DrawOcclusionProperty(ref context, onChange, registerUndo);
            this.DrawStencilCompProperty(ref context, onChange, registerUndo);
            this.DrawUIProperty(ref context, onChange, registerUndo);
            this.DrawCustomEditorProperty(ref context, onChange, registerUndo);
        }

        public override bool WorksWithSRP(RenderPipelineAsset scriptableRenderPipeline) {
            return scriptableRenderPipeline is LaviRenderPipelineAsset;
        }

        public bool TrySetActiveSubTarget(Type subTargetType) {
            if (!subTargetType.IsSubclassOf(typeof(SubTarget))) {
                return false;
            }
            
            foreach (var subTarget in this.subTargets) {
                if (subTarget.GetType().Equals(subTargetType)) {
                    this.activeSubTarget = subTarget;
                    return true;
                }
            }
            
            return false;
        }

        public void GetBlend(out Blend src, out Blend dst) {
            src = Blend.One;
            dst = Blend.Zero;

            if (this.surfaceType == SurfaceType.Transparent) {
                if (this.blendMode == BlendMode.Alpha) {
                    src = Blend.SrcAlpha;
                    dst = Blend.OneMinusSrcAlpha;
                }
                else {
                    src = Blend.One;
                    dst = Blend.One;
                }
            }
        }

        protected void DrawSelectMaterialProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            int activeSubTargetIndex = this.subTargets.IndexOf(this.activeSubTarget);
            var subTargetField = new PopupField<string>(this.subTargetsNames, activeSubTargetIndex);

            context.AddProperty("Material", subTargetField, (evt) =>
            {
                if (activeSubTargetIndex == subTargetField.index) {
                    return;
                }

                registerUndo("Change Material");
                this.activeSubTarget = this.subTargets[subTargetField.index];
                onChange();
            });
        }

        public void DrawSurfaceTypeProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var field = new EnumField(SurfaceType.Opaque) {value = this.surfaceType};
            
            context.AddProperty("Surface Type", field, (evt) => {
                var value = (SurfaceType)evt.newValue;

                if (this.surfaceType == value) {
                    return;
                }

                registerUndo("Change Surface");
                this.surfaceType = value;
                onChange();
            });
        }

        public void DrawBlendModeProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo, bool overrided=false) {
            if (overrided) {
                var toggle = new Toggle() {value = this.overrideBlendMode};

                context.AddProperty("> Blend Mode", toggle, (evt) => {
                    if (this.overrideBlendMode == evt.newValue) {
                        return;
                    }

                    registerUndo("Change Override Blend Mode");
                    this.overrideBlendMode = evt.newValue;
                    onChange();
                });
            }
            else {
                var field = new EnumField(BlendMode.Alpha) {value = this.blendMode};
                
                context.AddProperty("Blend Mode", field, this.surfaceType == SurfaceType.Transparent, (evt) => {
                    var value = (BlendMode)evt.newValue;

                    if (this.blendMode == value) {
                        return;
                    }

                    registerUndo("Change Blend");
                    this.blendMode = value;
                    onChange();
                });
            }
        }

        public void DrawCullModeProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo, bool overrided=false) {
            if (overrided) {
                var toggle = new Toggle() {value = this.overrideCullMode};

                context.AddProperty("> Cull Mode", toggle, (evt) => {
                    if (this.overrideCullMode == evt.newValue) {
                        return;
                    }

                    registerUndo("Change Override Cull Mode");
                    this.overrideCullMode = evt.newValue;
                    onChange();
                });
            }
            else {
                var field = new EnumField(CullMode.Back) {value = this.cullMode};
                
                context.AddProperty("Cull Mode", field, (evt) => {
                    var value = (CullMode)evt.newValue;

                    if (this.cullMode == value) {
                        return;
                    }

                    registerUndo("Change Cull");
                    this.cullMode = value;
                    onChange();
                });
            }
        }

        public void DrawZWriteProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo, bool overrided=false) {
            if (overrided) {
                var toggle = new Toggle() {value = this.overrideZWrite};

                context.AddProperty("> Z Write", toggle, (evt) => {
                    if (this.overrideZWrite == evt.newValue) {
                        return;
                    }

                    registerUndo("Change Override Z Write");
                    this.overrideZWrite = evt.newValue;
                    onChange();
                });
            }
            else {
                var toggle = new Toggle() {value = this.zWrite};

                context.AddProperty("Z Write", toggle, (evt) => {
                    if (this.zWrite == evt.newValue) {
                        return;
                    }

                    registerUndo("Change Z Write");
                    this.zWrite = evt.newValue;
                    onChange();
                });
            }
        }

        public void DrawZTestProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo, bool overrided=false) {
            if (overrided) {
                var toggle = new Toggle() {value = this.overrideZTest};

                context.AddProperty("> Z Test", toggle, (evt) => {
                    if (this.overrideZTest == evt.newValue) {
                        return;
                    }

                    registerUndo("Change Override Z Test");
                    this.overrideZTest = evt.newValue;
                    onChange();
                });
            }
            else {
                var field = new EnumField(ZTest.LEqual) {value = this.zTest};
                
                context.AddProperty("Z Test", field, (evt) => {
                    var value = (ZTest)evt.newValue;

                    if (this.zTest == value) {
                        return;
                    }

                    registerUndo("Change Z Test");
                    this.zTest = value;
                    onChange();
                });
            }
        }

        protected void DrawStencilProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var field = new EnumField(StencilType.None) {value = this.stencil};
            
            context.AddProperty("Stencil", field, (evt) => {
                var value = (StencilType)evt.newValue;

                if (this.stencil == value) {
                    return;
                }

                registerUndo("Change Stencil");
                this.stencil = value;
                onChange();
            });
        }

        protected void DrawOcclusionProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var field = new EnumField(StencilType.None) {value = this.occlusion};
            
            context.AddProperty("Occlusion", field, (evt) => {
                var value = (StencilType)evt.newValue;

                if (this.stencil == value) {
                    return;
                }

                registerUndo("Change Occlusion");
                this.occlusion = value;
                onChange();
            });
        }

        public void DrawStencilCompProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var field = new EnumField(CompareFunction.Always) {value = this.stencilComp};
            
            context.AddProperty("Stencil Comp", field, (evt) => {
                var value = (CompareFunction)evt.newValue;

                if (this.stencilComp == value) {
                    return;
                }

                registerUndo("Change Stencil Comp");
                this.stencilComp = value;
                onChange();
            });
        }

        public void DrawUIProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var toggle = new Toggle() {value = this.ui};

            context.AddProperty("UI", toggle, (evt) => {
                if (this.ui == evt.newValue) {
                    return;
                }

                registerUndo("Change UI");
                this.ui = evt.newValue;
                onChange();
            });
        }

        protected void DrawCustomEditorProperty(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo) {
            var field = new TextField() {value = this.customEditorGUI};
            field.RegisterCallback<FocusOutEvent>((s) => {
                if (this.customEditorGUI == field.value) {
                    return;
                }

                registerUndo("Change Custom Editor GUI");
                this.customEditorGUI = field.value;
                onChange();
            });

            context.AddProperty("Shader GUI", field, (evt) => {});
        }
    }
}