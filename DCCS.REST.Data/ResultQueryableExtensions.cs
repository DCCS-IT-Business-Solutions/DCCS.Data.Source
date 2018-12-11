using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DCCS.REST.Data
{
    public static class ResultQueryableExtensions
    {
        public static Result<T> ToResult<T>(this IQueryable<T> source, Params ps)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new Result<T>(ps, source);
        }
    }
}
