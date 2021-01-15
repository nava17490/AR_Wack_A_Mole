using System;
using System.Collections.Generic;

namespace MoleRehabilitation.DAO
{
    /// <summary>
    /// Interfaz que define los metodos propios del almacenamiento de los datos de pacientes
    /// </summary>
    interface PacientesDAO : UniversalDAO
    {
        /// <summary>
        /// Devuelve los pacientes existentes en el sistema
        /// </summary>
        /// <returns></returns>
        List<String> getPacientes();
    }
}
