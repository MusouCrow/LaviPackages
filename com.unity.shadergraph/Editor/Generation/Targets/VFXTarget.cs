#if VFX_GRAPH_10_0_0_OR_NEWER
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Legacy;

namespace UnityEditor.ShaderGraph
{
    // KOIYUN
    sealed class VFXTarget : Target, IMaySupportVFX
    {
        [SerializeField]
        bool m_AlphaTest = false;

        public VFXTarget()
        {
            displayName = "Visual Effect";
        }

        public bool alphaTest
        {
            get => m_AlphaTest;
            set => m_AlphaTest = value;
        }

        public override bool IsActive() => true;

        public override void Setup(ref TargetSetupContext context)
        {
        }

        public override void GetFields(ref TargetFieldContext context)
        {
        }

        public override bool IsNodeAllowedByTarget(Type nodeType)
        {
            return base.IsNodeAllowedByTarget(nodeType);
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context)
        {
            context.AddBlock(BlockFields.SurfaceDescription.BaseColor);
            context.AddBlock(BlockFields.SurfaceDescription.Alpha);
            context.AddBlock(BlockFields.SurfaceDescription.AlphaClipThreshold, alphaTest);
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo)
        {
            context.AddProperty("Alpha Clipping", new Toggle() { value = m_AlphaTest }, (evt) =>
            {
                if (Equals(m_AlphaTest, evt.newValue))
                    return;

                registerUndo("Change Alpha Test");
                m_AlphaTest = evt.newValue;
                onChange();
            });
        }

        public static Dictionary<BlockFieldDescriptor, int> s_BlockMap = new Dictionary<BlockFieldDescriptor, int>()
        {
            { BlockFields.SurfaceDescription.BaseColor, ShaderGraphVfxAsset.ColorSlotId },
            { BlockFields.SurfaceDescription.Alpha, ShaderGraphVfxAsset.AlphaSlotId },
            { BlockFields.SurfaceDescription.AlphaClipThreshold, ShaderGraphVfxAsset.AlphaThresholdSlotId },
        };

        public override bool WorksWithSRP(RenderPipelineAsset scriptableRenderPipeline)
        {
            return GraphicsSettings.currentRenderPipeline != null && scriptableRenderPipeline?.GetType() == GraphicsSettings.currentRenderPipeline.GetType();
        }

        public bool SupportsVFX() => true;
        public bool CanSupportVFX() => true;
    }
}
#endif
