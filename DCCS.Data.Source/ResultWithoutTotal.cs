using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DCCS.Data.Source
{
    public class ResultWithoutTotal<T> : Params
    {
        public IEnumerable<T> Data { get; protected set; }

        public ResultWithoutTotal(Params ps) : base(ps)
        { }

        public ResultWithoutTotal(Params ps, IQueryable<T> data) : base(ps)
        {
            if (data != null)
                SetData(data);
        }

        public virtual void SetData(IQueryable<T> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var result = Sort(data);

            result = Paging(result);
            Data = result.ToArray();
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
                if (!Count.HasValue) throw new ArgumentNullException("Bei angegebener Seite (page) muss auch die Anzahl der Einträge (count) angegeben werden!");

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

            return (tempresult ?? new List<T>().AsQueryable());

        }
    }
}
