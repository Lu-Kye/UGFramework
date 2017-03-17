namespace UGFramework.Editor.Inspector
{
    public static class InspectorUtility
    {
        public static void DrawObject(object obj)
        {
            ObjectDrawer.Draw(obj);
        }

        public static bool DrawObjectWritable(object obj)
        {
            return ObjectDrawer.DrawWritable(obj);
        }

        public static void DrawInt()
        {
        }

        public static bool DrawIntWritable()
        {
            return false;
        }
    }
}