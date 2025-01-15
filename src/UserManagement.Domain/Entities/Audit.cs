using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class Audit
    {
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
