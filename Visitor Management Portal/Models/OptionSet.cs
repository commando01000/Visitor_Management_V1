
using System;

namespace Visitor_Management_Portal.Models
{
    public class OptionSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OptionSet<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
    }
}


