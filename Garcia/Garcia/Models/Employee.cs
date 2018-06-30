using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Garcia.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [StringLength(50,MinimumLength =3,ErrorMessage ="Name must be between 3-50 characters long")]
        public string Name { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3-50 characters long")]
        public string Surname { get; set; }

    }
}