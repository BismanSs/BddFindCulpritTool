using System.Data.Common;

namespace BddFindCulpritTool.Models
{
    public class BinarySearchPoint
    {
        public int Id { get; set; }
        public string lastBadCommit { get; set; }
        public string lastGoodCommit { get; set; }
        public string bisectedCommit { get; set; }

    }
}
