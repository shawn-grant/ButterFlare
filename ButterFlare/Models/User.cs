using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ButterFlare.Models
{
    public class User
    {
        [Key]
        public Guid UID { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

}