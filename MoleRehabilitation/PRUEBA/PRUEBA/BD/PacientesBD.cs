using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MoleRehabilitation.DAO;

namespace MoleRehabilitation.BD
{
    /// <summary>
    /// Clase que implementa la interfaz PacientesDAO interactuando con una base de datos.
    /// </summary>
    class PacientesBD : PacientesDAO
    {

        #region Constructores

        /// <summary>
        /// Crea una instancia para poder realizar consultas y otras peticiones a la base de datos relacionadas con Pacientes
        /// </summary>
        public PacientesBD()
        {

        }
        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene una lista de pacientes dados de alta en el sistema
        /// </summary>
        /// <returns>Devuelve lista con los nombres de los pacientes</returns>
        public List<String> getPacientes()
        {
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            MySqlDataReader lector = gestorBD.query("SELECT * FROM paciente WHERE borrado=0");
            List<String> pacientes = new List<String>();
            int i = 0;
            while (lector.Read())
            {
                pacientes.Add(lector.GetString(i));
            }
            gestorBD.desconectar();
            return pacientes;
        }

        /// <summary>
        /// Inserta un paciente en la base de datos
        /// </summary>
        /// <param name="obj">Nombre del paciente a insertar</param>
        /// <returns>0 --> Inserción realizada con exito, !=0 Inserción Fallida </returns>
        public int insert(Object obj)
        {
            String s = (String)obj;
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            gestorBD.nonQuery("INSERT INTO paciente VALUES ('" + s + "', 0);");
            gestorBD.desconectar();
            return 0;
        }

        /// <summary>
        /// Método sin implementar
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int update(Object obj)
        {
            return 0;
        }

        public void delete(string paciente)
        {
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            gestorBD.nonQuery("UPDATE paciente SET borrado = 1 WHERE codigo=\""+paciente+"\";");
            gestorBD.desconectar();
        }
        #endregion


        
    }
}
