using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAuth.Models
{
    public class TaskManager
    {
        public enum Priority
        {
            VeryLow,
            Low,
            Average, 
            High,
            VeryHigh
        }
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public Priority Importance { get; set; }
        public bool IsFinished { get; set; }
    }
}
