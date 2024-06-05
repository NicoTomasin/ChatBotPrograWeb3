using BotPeliculas.Bot.EF;

namespace BotPelicualas.Logica;

public class UsuarioServicio
{
    private BotPeliculasContext _context;

    public UsuarioServicio(BotPeliculasContext context)
    {
        this._context = context;
    }

    public void AgregarUsuario(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        _context.SaveChanges();
    }

    public Usuario BuscarPorNombreDeUsuario(string nombreDeUsuario)
    {
        return _context.Usuarios.FirstOrDefault(u => u.UserName == nombreDeUsuario);
    }
}