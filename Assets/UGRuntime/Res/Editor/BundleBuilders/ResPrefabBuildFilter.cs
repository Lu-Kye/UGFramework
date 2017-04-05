namespace UGFramework.Res
{
    public class ResPrefabBuildFilter : ResAbstractBuildFilter
    {
        public override string FileExtension { get { return ".prefab"; } }
        public ResPrefabBuildFilter() : base() {}
    }
}
