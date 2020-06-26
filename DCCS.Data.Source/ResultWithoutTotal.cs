using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace DCCS.Data.Source
{
    public class ResultWithoutTotal<T> : Params
    {
        public IEnumerable<T> Data { get; protected set; }

        public ResultWithoutTotal(Params ps) : base(ps)
        { }

        public ResultWithoutTotal(Params ps, IQueryable<T> data, bool sort = true, bool page = true) : base(ps)
        {
            if (data != null)
            {
                SetData(data, sort: sort, page: page);
            }
        }

        public ResultWithoutTotal(Params ps, IEnumerable<T> data) : base(ps)
        {
            Data = data;
            Count = Data.Count();
        }

        public virtual void SetData(IQueryable<T> data, bool sort, bool page)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (Count.HasValue && Count.Value == 0)
            {
                Data = new List<T>();
            }
            else
            {
                var result = data;
                if (sort) { result = Sort(result); }
                if (page) { result = Paging(result); }
                Data = result.ToArray();
            }
        }

        public Result<DTO> Select<DTO>(Expression<Func<T, DTO>> predicate)
        {
            return new Result<DTO>(new Params
            {
                Count = Count,
                OrderBy = OrderBy,
                Desc = Desc,
                Page = Page
            })
            {
                Data = Data.AsQueryable().Select(predicate)
            };
        }

        protected IQueryable<T> Sort(IQueryable<T> data)
        {
            // Sortieren...
            if (!string.IsNullOrWhiteSpace(OrderBy))
            {
                var members = typeof(T).GetMember(OrderBy, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (!members.Any(m => m is FieldInfo || m is PropertyInfo))
                {
                    throw new ArgumentOutOfRangeException(nameof(OrderBy), OrderBy, $"Order by with '{OrderBy}' is not allowed");
                }
                return data.OrderBy($"{OrderBy} {(Desc ? "desc" : "")}");
            }
            else if (data.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                // Vor einem Skip (siehe unten) muß ein OrderBy aufgerufen werden.
                // Ist keine Sortierung angegeben, müssen wir dennoch sortieren und behalten
                // dabei die Reihenfolge bei.
                return data.OrderBy(x => true);
            }

            return data;
        }

        protected IQueryable<T> Paging(IQueryable<T> data)
        {
            // Pagen...
            IQueryable<T> tempresult = null; // Wird für "Kann diese Seite überhaupt angezeigt werden" benötigt
            if (Page.HasValue)
            {
                if (!Count.HasValue)
                    throw new ArgumentNullException($"With specified {nameof(Page)} is the {nameof(Count)} required");

                //Manuel 24.10.2016
                //INFO: es wurde falsche ergebnisse geliefert weil "OrderBy>true" gefehlt hat. -> das sollte sich stephan nochmal anschauen
                //Folgende anpassungen sind als Quickfix anzusehen und sollten noch optimiert werden

                var skip = (Page.Value - 1) * Count.Value;
                var take = Count.Value;

                //Ohne OrderBy, da es eine Sortierung gibt
                if (!string.IsNullOrWhiteSpace(OrderBy))
                {
                    tempresult = data.Skip(skip).Take(take);
                }
                //Mit OrderBy, da es keine Sortierung gibt
                else
                {
                    tempresult = data.Skip(skip).OrderBy(x => true).Take(take).OrderBy(x => true);
                }
                if (!tempresult.Any())
                {
                    Page = 1;
                    tempresult = data.Take(Count.Value);
                }
            }
            else
            {
                Page = 1;
                if (Count.HasValue)
                {
                    tempresult = data.Take(Count.Value);
                }
                else
                {
                    tempresult = data;
                }
            }
            return (tempresult ?? new List<T>().AsQueryable());

        }
    }
}
