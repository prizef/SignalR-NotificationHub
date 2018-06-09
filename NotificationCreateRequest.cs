using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests
{
    public class NotificationCreateRequest
    {
        [Required]
        public int? UserId { get; set; }

        [Required]
        public string NotificationJson { get; set; }
    }
}
