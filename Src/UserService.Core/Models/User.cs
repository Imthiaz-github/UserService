using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserService.Core.Models
{
    public class User
    {
        [SwaggerSchema(ReadOnly = true)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
