using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CrowdKnowledge2.Models
{
    public class Domeniu
    {
        [Key]
        public int IDDomeniu { get; set; }

        [Required(ErrorMessage = "Numele domeniului este camp obligatoriu")]
        public string NumeDomeniu { get; set; }

        public virtual ICollection<Articol> Articole { get; set; }
    }
}