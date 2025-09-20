//namespace ERPSys.SharedKernel
//{
//    public class PaginatedList<T>
//    {
//        public List<T> Items { get; }
//        public int PageIndex { get; }
//        public int TotalPages { get; }
//        public int TotalCount { get; } // <<< add this

//        public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
//        {
//            PageIndex = pageIndex;
//            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
//            Items = new List<T>(items);
//            TotalCount = count; // <<< set this
//        }
//    }

//}