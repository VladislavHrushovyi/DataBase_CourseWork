using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    class DirectorInfo:SomeAbstact
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string DateBorn { get; set; }
        public string Phone { get; set; }
        public string E_mail { get; set; }
    }
}
