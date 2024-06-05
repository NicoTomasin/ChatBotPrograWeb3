using System;
using System.Collections.Generic;

namespace BotPeliculas.Bot.EF
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
