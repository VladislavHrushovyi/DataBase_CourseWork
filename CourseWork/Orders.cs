using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    class Orders: SomeAbstact
    {
        public string Id { get; set; }
        public string Product_id { get; set; }
        public string Amount { get; set; }
        public string Cost_Order { get; set; }
        public string Client_id { get; set; }
    }
}
