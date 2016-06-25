using System.Collections.Generic;
using PropertyChanged;
using System;

namespace BD2.Model
{
    [ImplementPropertyChanged]
    public class Person
    {
        public string Name { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string Gender { get; set; }
        public string Father { get; set; }
        public string Mother { get; set; }

        public List<Child> Children { get; set; }


        public Person()
        {
            Children = new List<Child>();
        }

       
    }
}
