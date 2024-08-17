using System;
using System.Collections.Generic;

namespace CanguroAPI.Server.Models;

public partial class Log
{
    public int IdLog { get; set; }

    public DateTime LogDate { get; set; }

    public string Description { get; set; } = null!;

    public string? LogLevel { get; set; }

    public int? UserId { get; set; }

    public string? ControllerName { get; set; }

    public string? ActionName { get; set; }

    public string? RequestUrl { get; set; }
}
