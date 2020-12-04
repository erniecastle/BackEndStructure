/**
 * @author: Ernesto Castillo
 * Fecha de Creación: 21/02/2018
 * Compañía: Exito
 * Descripción del programa: Entidad para HBRequest
 * -----------------------------------------------------------------------------
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Exitosw.Payroll.Hibernate.entidad
{

    public partial class CalculoUnidades
    {
    

        public CalculoUnidades()
        {
            
        }
        public CalculoUnidades(CalculoUnidades nuevoCalculoUnidad)
        {
            this.id = 0;
            this.razonesSociales = nuevoCalculoUnidad.razonesSociales;
            this.empleados = nuevoCalculoUnidad.empleados;
            this.tipoNomina = nuevoCalculoUnidad.tipoNomina;
            this.periodosNomina = nuevoCalculoUnidad.periodosNomina;
            this.tipoCorrida = nuevoCalculoUnidad.tipoCorrida;
            this.numero = nuevoCalculoUnidad.numero;
            this.ejercicio = nuevoCalculoUnidad.ejercicio;
            this.mes = nuevoCalculoUnidad.mes;
            this.plazasPorEmpleado = nuevoCalculoUnidad.plazasPorEmpleado;
            this.uso = nuevoCalculoUnidad.uso;
            this.numMovParticion = nuevoCalculoUnidad.numMovParticion;
            this.diasTrabajados = nuevoCalculoUnidad.diasTrabajados;
            this.diasRetardo = nuevoCalculoUnidad.diasRetardo;
            this.diasFalta = nuevoCalculoUnidad.diasFalta;
            this.diasAusentismo = nuevoCalculoUnidad.diasAusentismo;
            this.diasPermisoConSueldo = nuevoCalculoUnidad.diasPermisoConSueldo;
            this.diasPermisoSinSueldo = nuevoCalculoUnidad.diasPermisoSinSueldo;
            this.diasIncapacidadEnfermedad = nuevoCalculoUnidad.diasIncapacidadEnfermedad;
            this.diasIncapacidadAccidente = nuevoCalculoUnidad.diasIncapacidadAccidente;
            this.diasIncapacidadMaternidad = nuevoCalculoUnidad.diasIncapacidadMaternidad;
            this.diasOtrasIncapacidades = nuevoCalculoUnidad.diasOtrasIncapacidades;
            this.diasDescansoLaborado = nuevoCalculoUnidad.diasDescansoLaborado;
            this.diasFestivoLaborado = nuevoCalculoUnidad.diasFestivoLaborado;
            this.diasDomingoLaborado = nuevoCalculoUnidad.diasDomingoLaborado;
            this.hrsExtraTriple = nuevoCalculoUnidad.hrsExtraTriple;
            this.hrsExtraDoble = nuevoCalculoUnidad.hrsExtraDoble;
            this.hrsExtraTriple = nuevoCalculoUnidad.hrsExtraTriple;
            this.diasDescanso = nuevoCalculoUnidad.diasDescanso;
            this.diasFestivo = nuevoCalculoUnidad.diasFestivo;
            this.diasVacaciones = nuevoCalculoUnidad.diasVacaciones;
            this.diasPrimaVacacional = nuevoCalculoUnidad.diasPrimaVacacional;
            this.tiposVacaciones = nuevoCalculoUnidad.tiposVacaciones;
        }

        public virtual decimal id
        {
            get;
            set;
        }

        public virtual int? diasAusentismo
        {
            get;
            set;
        }

  
        public virtual int? diasDescanso
        {
            get;
            set;
        }

        public virtual double? diasDescansoLaborado
        {
            get;
            set;
        }

        public virtual double? diasDomingoLaborado
        {
            get;
            set;
        }

        public virtual double? diasFalta
        {
            get;
            set;
        }

        public virtual int? diasFestivo
        {
            get;
            set;
        }

        public virtual double? diasFestivoLaborado
        {
            get;
            set;
        }

        public virtual int? diasIncapacidadAccidente
        {
            get;
            set;
        }

        public virtual int? diasIncapacidadEnfermedad
        {
            get;
            set;
        }

        public virtual int? diasIncapacidadMaternidad
        {
            get;
            set;
        }

        public virtual int? diasOtrasIncapacidades
        {
            get;
            set;
        }

        public virtual double? diasPermisoConSueldo
        {
            get;
            set;
        }

        public virtual double? diasPermisoSinSueldo
        {
            get;
            set;
        }

        public virtual double? diasPrimaVacacional
        {
            get;
            set;
        }

        public virtual double? diasRetardo
        {
            get;
            set;
        }

        public virtual double? diasTrabajados
        {
            get;
            set;
        }

        public virtual int? diasVacaciones
        {
            get;
            set;
        }

        public virtual int ejercicio
        {
            get;
            set;
        }

        public virtual double? hrsExtraDoble
        {
            get;
            set;
        }

        public virtual double? hrsExtraTriple
        {
            get;
            set;
        }

        public virtual int? mes
        {
            get;
            set;
        }

        public virtual int numMovParticion
        {
            get;
            set;
        }

        public virtual int? numero
        {
            get;
            set;
        }

        public virtual int uso
        {
            get;
            set;
        }

        public virtual PlazasPorEmpleado plazasPorEmpleado
        {
            get;
            set;
        }

        public virtual TipoCorrida tipoCorrida
        {
            get;
            set;
        }

        public virtual TipoNomina tipoNomina
        {
            get;
            set;
        }

        public virtual TiposVacaciones tiposVacaciones
        {
            get;
            set;
        }

 
        public virtual PeriodosNomina periodosNomina
        {
            get;
            set;
        }

        public virtual Empleados empleados
        {
            get;
            set;
        }

        public virtual RazonesSociales razonesSociales
        {
            get;
            set;
        }
    }

}
