using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace DCCS.REST.Data
{
    public class Result<T> : Params
    {
        public IEnumerable<T> Data { get; protected set; }
        public int Total { get; set; }

        public Result(Params ps) : base(ps)
        { }

        public Result(Params ps, IQueryable<T> data) : base(ps)
        {
            SetData(data);
        }

        public void SetData(IQueryable<T> data)
        {
            var result = data;

            // Sortieren...
            if (!string.IsNullOrWhiteSpace(OrderBy))
            {
                //System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy()
                result = result.OrderBy($"{OrderBy} {(Desc ? "desc" : "")}");
            }
            else if (data.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                // Vor einem Skip (siehe unten) muß ein OrderBy aufgerufen werden.
                // Ist keine Sortierung angegeben, müssen wir dennoch sortieren und behalten
                // dabei die Reihenfolge bei.
                result = result.OrderBy(x => true);
            }

            Total = result.Count();
            if (Count.HasValue)
            {
                Count = Math.Min(Count.Value, Total);
            }


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
                    tempresult = result.Skip(skip).Take(take);
                }
                //Mit OrderBy, da es keine Sortierung gibt
                else
                {
                    tempresult = result.Skip(skip).OrderBy(x => true).Take(take).OrderBy(x => true);
                }
                if (!tempresult.Any())
                {
                    Page = 1;
                    tempresult = result.Take(Count.Value);
                }
            }

            Data = (tempresult ?? new List<T>().AsQueryable()).ToArray();
        }
    }
}
