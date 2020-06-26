using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DCCS.Data.Source
{
    public class Result<T> : ResultWithoutTotal<T>
    {
        public Result(Params ps, IQueryable<T> data = null, bool sort = true, bool page = true) : base(ps, data, sort, page)
        {
        }

        public Result(Params ps, IEnumerable<T> data, int total) : base(ps, data)
        {
            Total = total;
        }

        public override void SetData(IQueryable<T> data, bool sort, bool page)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Total = data.Count();
            if (Count.HasValue)
            {
                Count = Math.Min(Count.Value, Total);
            }
            base.SetData(data, sort: sort, page: page);
        }

        public int Total { get; set; }
    }
}
