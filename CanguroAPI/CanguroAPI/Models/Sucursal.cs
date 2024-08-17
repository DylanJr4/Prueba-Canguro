using System;
using System.Collections.Generic;

namespace CanguroAPI.Server.Models;

public partial class Sucursal
{
    public int IdSucursal { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public int? IdMoneda { get; set; }

    public virtual Moneda? IdMonedaNavigation { get; set; }
}
