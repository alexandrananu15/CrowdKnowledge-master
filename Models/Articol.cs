using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdKnowledge2.Models
{
    public class Articol
    {
        public bool Restrict { get; set; }
        [Key]
        public int IDArticol { get; set; }

        [Required(ErrorMessage = "Titlul articolului este camp obligatoriu!")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 caractere")]
        public string TitluArticol { get; set; }

        [Required(ErrorMessage = "Continutul articolului este obligatoriu!")]
        [DataType(DataType.MultilineText)]
        public string ContinutArticol { get; set; }

        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Categoria articolului este camp obligatoriu!")]
        public int IDDomeniu { get; set; }
        
        public int IdParent { get; set; }

        //ID-ul utilizatorului care a creat respectivul articol
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        public virtual Domeniu Domeniu { get; set; }
        public virtual ICollection<Capitol> Capitole { get; set; }

        public IEnumerable<SelectListItem> Dom { get; set; }
    }
}