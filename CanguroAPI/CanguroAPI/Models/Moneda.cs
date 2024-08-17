using System;
using System.Collections.Generic;

namespace CanguroAPI.Server.Models;

public partial class Moneda
{
    public int IdMoneda { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Sucursal> Sucursals { get; set; } = new List<Sucursal>();
}
