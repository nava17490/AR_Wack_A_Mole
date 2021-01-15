using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoleRehabilitation.DAO;

namespace MoleRehabilitation.BD
{
    /// <summary>
    /// Clase que implementa la interfaz PosicionDAO interactuando con una base de datos.
    /// </summary>
    class PosicionBD : PosicionDAO
    {
        #region Constructores

        /// <summary>
        /// Crea una instancia de la clase para tratar el almacenamiento de las posiciones en la Base de datos 
        /// </summary>
        public PosicionBD()
        {
            //TODO
        }

        #endregion

        #region metodos publicos
        
        /// <summary>
        /// Inserta una posición en la base de datos dados.
        /// </summary>
        /// <param name="obj">Lista de objetos a almacenar con este formato (long instanteTiempo, String Extremidad, float x, float y, float z, int codigoEjercicio)</param>
        /// <returns>0 --> Inserción realizada con exito, !=0 Inserción Fallida </returns>
        public int insert(object obj)
        {
            List<Object> l = (List<Object>)obj;
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            gestorBD.nonQuery("INSERT INTO posicion VALUES (null, '" + (long)l[0] + "','" + l[1].ToString() + "','" + (float)l[2] + "','" + (float)l[3] + "','" + (float)l[4] + "'," + (int)l[5] + ");");
            gestorBD.desconectar();
            return 0;
        }

        /// <summary>
        /// Metodo sin implementar
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int update(object obj)
        {
            return 0;
        }
        #endregion
        
    }
}
