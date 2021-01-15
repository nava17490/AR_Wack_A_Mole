using System;

namespace MoleRehabilitation.DAO
{
    /// <summary>
    /// Interfaz que define los metodos comunes al resto de interfaces DAO
    /// </summary>
    interface UniversalDAO
    {

        /// <summary>
        /// Inserta un elemento en el almacenamiento permanente
        /// </summary>
        /// <param name="obj">Elemento a almacenar</param>
        /// <returns></returns>
        int insert(Object obj);

        /// <summary>
        /// Actualiza uno de los datos almacenados
        /// </summary>
        /// <param name="obj">Datos a modificar</param>
        /// <returns></returns>
        int update(Object obj);

    }
}
