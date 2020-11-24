using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SII2.Models
{
    public enum MemoryType
    {
        [Description("Оперативная память")]
        RAM,
        [Description("Графическая память")]
        GraphicMemory,
        [Description("Память микроконтроллеров")]
        MicrocontrollerRAM,
        [Description("Кэш-память")]
        Cache,
        [Description("Вторичная память")]
        SecondaryMemory,
    }
}
