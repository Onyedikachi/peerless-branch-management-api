namespace Retail.Branch.Services.Common
{
    public class PagedResponse<T>
    {
        public PagedResponse(string msg, object? T, int count, int currentPage, int pageSize)
        {
            
            var pagecount = PageCount(count, pageSize); 
            this.Results = T;
            this.Count = count;
            this.Next = ((currentPage + 1) > pagecount) ? null : (currentPage + 1);
            this.Previous = ((currentPage - 1) < 1) ? null : (currentPage - 1);
        }
        public int? Next { get; set; } = 1;
        public int? Previous { get; set; } = 1;
        public int Count { get; set; }
        public object? Results { get; set; }

        private int PageCount(int total,int pageSize)
        {
            int numberOfPages = (int)Math.Ceiling((double)total / pageSize);
            return numberOfPages;
        }

    }


    public static class IQueryableExtensions
    {
        public static IList<T> GetPaginatedReponseAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {           
            return
            query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();
        }
    }

}
