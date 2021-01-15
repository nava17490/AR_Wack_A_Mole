using System;
using System.Collections.Generic;

namespace MoleRehabilitation.DAO
{
    /// <summary>
    /// Interfaz que define los metodos propios del almacenamiento de la configuración
    /// </summary>
    interface ConfiguracionDAO : UniversalDAO
    {
        /// <summary>
        /// Devuelve la ultima configuración de un paciente
        /// </summary>
        /// <param name="paciente"></param>
        /// <returns></returns>
        List<int> getConfiguracion(String paciente);
    }
}
