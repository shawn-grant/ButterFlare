using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ButterFlare.Models
{
    public class Post
    {
        [Key]
        public int id { get; set; }
        public Guid UID { get; set; }
        public string caption { get; set; }
        public byte[] image { get; set; }

    }
}