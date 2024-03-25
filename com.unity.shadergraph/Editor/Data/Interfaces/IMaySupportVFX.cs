namespace UnityEditor.ShaderGraph
{
    interface IMaySupportVFX
    {
        bool SupportsVFX();
        bool CanSupportVFX();
        bool HasBlock(BlockFieldDescriptor descriptor, out int slotID);
    }

    static class MaySupportVFXExtensions
    {
        public static bool SupportsVFX(this Target target)
        {
            var vfxTarget = target as IMaySupportVFX;
            return vfxTarget != null && vfxTarget.SupportsVFX();
        }

        public static bool CanSupportVFX(this Target target)
        {
            var vfxTarget = target as IMaySupportVFX;
            return vfxTarget != null && vfxTarget.CanSupportVFX();
        }
    }
}
