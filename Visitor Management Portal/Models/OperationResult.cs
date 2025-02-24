using System;

namespace Visitor_Management_Portal.Models
{
    public class OperationResult
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string RedirectURL { get; set; }
    }
    public class OperationResult<T>
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string RedirectURL { get; set; }
        public T Data { get; set; }
    }
}
