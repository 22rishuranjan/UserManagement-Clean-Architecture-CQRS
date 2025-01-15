using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.Entities
{
    public class Country :Audit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }


        // Initialize navigation property to avoid null references
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
