namespace BddFindCulpritTool.Models
{
    public class BinarySearchPrRun
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GitHash { get; set; }
        public string Link { get; set; }
        public int RunStatus { get; set; }

        public BinarySearchPrRun()
        {

        }
    }
}
