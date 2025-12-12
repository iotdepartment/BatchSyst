using System.Collections.Generic;

namespace Batch.ViewModels
{
    public class UsuariosViewModel
    {
        public List<UsuarioItemViewModel> Usuarios { get; set; }
        public UsuarioFormViewModel UsuarioForm { get; set; } = new UsuarioFormViewModel();
    }
}