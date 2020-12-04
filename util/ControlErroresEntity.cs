using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace Exitosw.Payroll.Core.util
{
    public class ControlErroresEntity
    {
        //private static readonly int constraintViolationException = 1;
        //private static readonly int transactionException = 2;
        //private static readonly int propertyValueException = 3;
        //private static readonly int jdbcException = 4;
        //private static readonly int lazyInitializationException = 5;
        //private static readonly int nonUniqueObjectException = 6;
        //private static readonly int nonUniqueResultException = 7;
        //private static readonly int objectDeletedException = 8;
        //private static readonly int queryException = 9;
        //private static readonly int queryParameterException = 10;
        //private static readonly int sessionException = 11;
        //private static readonly int staleStateException = 12;
        //private static readonly int dataException = 13;
        //private static readonly int jdbcConnectionException = 14;
        //private static readonly int sqlGrammarException = 15;
        //private static readonly int exceptionXML = 16;
        //private static readonly int hibernateException = 17;
        //private static readonly int jdbcTransaction = 18;
        //private static readonly int exception = 19;

        public static int buscaNoErrorPorExcepcion(Exception ex)
        {
            //Console.WriteLine("class " +);
            // System.out.println("class " + classExeption.getSimpleName());
            //    if (classExeption.equals(ConstraintViolationException.class)) {
            //    return constraintViolationException; 
            //} else if (classExeption.equals(TransactionException.class)) {
            //    return transactionException;  
            //} else if (classExeption.equals(PropertyValueException.class)) {
            //    return propertyValueException;
            //} else if (classExeption.equals(JDBCException.class)) {
            //    return jdbcException;
            //} else if (classExeption.equals(LazyInitializationException.class)) {
            //    return lazyInitializationException;
            //} else if (classExeption.equals(NonUniqueObjectException.class)) {
            //    return nonUniqueObjectException;
            //} else if (classExeption.equals(NonUniqueResultException.class)) {
            //    return nonUniqueResultException;
            //} else if (classExeption.equals(ObjectDeletedException.class)) {
            //    return objectDeletedException;
            //} else if (classExeption.equals(QueryException.class)) {
            //    return queryException;
            //} else if (classExeption.equals(QueryParameterException.class)) {
            //    return queryParameterException;
            //} else if (classExeption.equals(SessionException.class)) {
            //    return sessionException; 
            //} else if (classExeption.equals(StaleStateException.class)) {
            //    return staleStateException; 
            //} else if (classExeption.equals(DataException.class)) {
            //    return dataException; 
            //} else if (classExeption.equals(JDBCConnectionException.class)) {
            //    return jdbcConnectionException;
            //} else if (classExeption.equals(SQLGrammarException.class)) {
            //    return sqlGrammarException;
            //} else if (classExeption.equals(Exception.class)) {
            //    return exception;
            //} else if (classExeption.equals(SAXException.class) | classExeption.equals(IOException.class) | classExeption.equals(ParserConfigurationException.class)) {
            //    return exceptionXML;
            //} else if (classExeption.equals(HibernateException.class)) {
            //    return hibernateException;
            //} else if (classExeption.equals(JDBCTransaction.class)) {
            //    return jdbcTransaction;
            //}

            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();

            return line;
    }





    }
}