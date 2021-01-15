using System.Collections.Generic;

namespace MoleRehabilitation.DAO
{
    /// <summary>
    /// Interfaz SIN TERMINAR DE IMPLEMENTAR
    /// </summary>
    interface SesionDAO : UniversalDAO
    {
        List<object> getEstadisticas(string paciente, int numSesiones);
    }
}
