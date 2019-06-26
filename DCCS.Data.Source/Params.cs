using System;
using System.Collections.Generic;
using System.Text;

namespace DCCS.Data.Source
{
    public class Params
    {
        public Params()
        {
        }

        public Params(Params ps)
        {
            if (ps != null)
            {
                this.Page = ps.Page;
                this.Count = ps.Count;
                this.OrderBy = ps.OrderBy;
                this.Desc = ps.Desc;
            }
        }

        public int? Page { get; set; }
        public int? Count { get; set; }
        public string OrderBy { get; set; }
        public bool Desc { get; set; }
    }
}
