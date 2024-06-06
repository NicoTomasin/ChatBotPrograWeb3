using System;
using System.Collections.Generic;

namespace BotPeliculas.Models.EF
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string Mail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }
}
