using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;


namespace CrowdKnowledge2.Models
{
    public class Capitol
    {
        [Key]
        public int IDCapitol { get; set; }

        [Required(ErrorMessage = "Titlul capitolului este camp obligatoriu!")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 20 caractere")]
        public string TitluCapitol { get; set; }

        [Required(ErrorMessage = "Continutul capitolului este obligatoriu!")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public DateTime Data { get; set; }
        public int IDArticol { get; set; }

        //ID-ul utilizatorului care a creat respectivul capitol
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual Articol Articol { get; set; }
    }
}