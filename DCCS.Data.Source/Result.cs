using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DCCS.Data.Source
{
    public class Result<T> : ResultWithoutTotal<T>
    {
        public Result(Params ps, IQueryable<T> data = null) : base(ps, data)
        {
        }

        public Result(Params ps, IEnumerable<T> data) : base(ps, data) { }

        public override void SetData(IQueryable<T> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Total = data.Count();
            if (Count.HasValue)
            {
                Count = Math.Min(Count.Value, Total);
            }
            base.SetData(data);
        }

        public int Total { get; set; }
    }
}
