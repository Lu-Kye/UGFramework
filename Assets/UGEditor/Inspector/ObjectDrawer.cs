namespace UGFramework.Editor.Inspector
{
    public struct ObjectDrawer
    {
        public static void Draw(object obj)
        {
            var drawer = new ObjectDrawer();
            drawer.Object = obj;
            drawer.Draw();
        }

        public static bool DrawWritable(object obj)
        {
            var drawer = new ObjectDrawer();
            drawer.Object = obj;
            return drawer.DrawWritable();
        }

        public object Object { get; set; }

        public void Draw()
        {
        }

        public bool DrawWritable()
        {
            return false;
        }
    }
}