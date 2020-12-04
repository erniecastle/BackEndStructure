using Exitosw.Payroll.Hibernate.modelo;
using System;
using System.Collections.Generic;
using System.Text;
using Exitosw.Payroll.Hibernate.entidad;
using NHibernate;
using System.Reflection;
using Exitosw.Payroll.Hibernate.util;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;
using System.Linq;
using System.Drawing;
using System.IO;

namespace Exitosw.Payroll.Core.modeloHB
{
    public class ReportesDAO : NHibernateRepository<ReportesDAO>, ReportesDAOIF
    {
        private StringBuilder concatena = new StringBuilder(200);
        private StringBuilder msgError = new StringBuilder().Append("Exitosw.Payroll.Core").Append(".MSERR_C_").Append(MethodBase.GetCurrentMethod().DeclaringType.Name).Append(".");
        List<Object> listaCursos = new List<Object>();
        IQuery query;
        public Mensaje getResumenPercepDeducc(Dictionary<string, object> filtros, ISession uuidCxn)
        {
            inicializaVariableMensaje();
            StringBuilder builder = new StringBuilder();
            try
            {
                inicializaVariableMensaje();
                setSession(uuidCxn);
                getSession().BeginTransaction();
                String query = null;
                IQuery q = null;
                //int tipoConsulta = -1;

                if (filtros.ContainsKey("totales"))
                {
                    //  tipoConsulta = Convert.ToInt32(filtros["tipoArchivo"]);
                }

                #region Query Resumen Percpciones Deducciones

                builder.Append("Select ");
                builder.Append("cnc.clave as claveConcepNomDefi, cnc.descripcion as descripcionConcepNomDefi, ");
                builder.Append("cnc.naturaleza as naturalezaConcepNomDefi, reg.registroPatronal as regPatronal, ");
                builder.Append("CASE WHEN (cnc.naturaleza IS NULL) THEN 0.0 ELSE  sum(mov.resultado) END as importe ");
                builder.Append("From PlazasPorEmpleadosMov pm ");
                builder.Append("LEFT JOIN pm.plazasPorEmpleado pl LEFT OUTER JOIN pl.empleados emp, ");
                builder.Append("MovNomConcep mov RIGHT OUTER JOIN mov.concepNomDefi cnc LEFT OUTER JOIN pl.razonesSociales rs ");
                builder.Append("RIGHT OUTER JOIN pl.registroPatronal reg ");
                builder.Append("RIGHT OUTER JOIN mov.periodosNomina per ");
                builder.Append("RIGHT OUTER JOIN mov.tipoCorrida tipcorr ");
                builder.Append("WHERE 1 = 1 ");
                builder.Append("AND mov.plazasPorEmpleado.id = pm.plazasPorEmpleado.id ");


                if (filtros.ContainsKey("claveRazonSocial"))
                {
                    builder.Append("AND rs.clave = :claveRazonSocial ");
                }

                if (filtros.ContainsKey("claveTipoCorrida"))
                {
                    builder.Append("AND mov.tipoCorrida.clave = :claveTipoCorrida ");
                }

                if (filtros.ContainsKey("claveTipoNomina"))
                {
                    builder.Append("AND mov.tipoNomina.clave = :claveTipoNomina ");
                }

                if (filtros.ContainsKey("claveCentroDeCostos"))
                {
                    builder.Append("AND pm.centroDeCosto.clave = :claveCentroDeCostos ");
                }

                if (filtros.ContainsKey("idPeriodoNomina"))
                {
                    builder.Append("AND per.id  = :idPeriodoNomina ");
                }


                if (filtros.ContainsKey("claveDelEmpleado") && filtros.ContainsKey("claveAlEmpleado"))
                {
                    builder.Append("AND (emp.clave BETWEEN :claveDelEmpleado AND :claveAlEmpleado) ");
                }
                else
                {
                    if (filtros.ContainsKey("claveDelEmpleado"))
                    {
                        builder.Append("emp.clave = :claveDelEmpleado ");
                    }

                    if (filtros.ContainsKey("claveAlEmpleado"))
                    {
                        builder.Append("emp.clave = :claveAlEmpleado ");
                    }
                }

                builder.Append(" group by cnc.clave,cnc.descripcion,reg.registroPatronal, cnc.naturaleza ");

                query = builder.ToString();
                q = getSession().CreateQuery(query);

                if (filtros.ContainsKey("claveRazonSocial"))
                {
                    q.SetParameter("claveRazonSocial", filtros["claveRazonSocial"].ToString());
                }
                if (filtros.ContainsKey("claveTipoCorrida"))
                {
                    q.SetParameter("claveTipoCorrida", filtros["claveTipoCorrida"].ToString());
                }
                if (filtros.ContainsKey("claveTipoNomina"))
                {
                    q.SetParameter("claveTipoNomina", filtros["claveTipoNomina"].ToString());
                }
                if (filtros.ContainsKey("claveCentroDeCostos"))
                {
                    q.SetParameter("claveCentroDeCostos", filtros["claveCentroDeCostos"].ToString());
                }

                if (filtros.ContainsKey("idPeriodoNomina"))
                {
                    q.SetParameter("idPeriodoNomina", filtros["idPeriodoNomina"].ToString());
                }

                if (filtros.ContainsKey("claveDelEmpleado"))
                {
                    q.SetParameter("claveDelEmpleado", filtros["claveDelEmpleado"].ToString());
                }

                if (filtros.ContainsKey("claveAlEmpleado"))
                {
                    q.SetParameter("claveAlEmpleado", filtros["claveAlEmpleado"].ToString());
                }

                #endregion

                IList<object> listResult = (IList<object>)q.SetResultTransformer(new DictionaryResultTransformer()).List();
                mensajeResultado.resultado = listResult;

                if (listResult.Count > 0)
                {
                    createReportExcel(filtros, listResult);
                }
                else
                {
                    mensajeResultado.resultado = null;
                }

                mensajeResultado.noError = (0);
                mensajeResultado.error = ("");
                getSession().Transaction.Commit();
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(concatena.Remove(0, concatena.Length).Append(msgError).Append("getResumenPercepDeducc()1_Error: ").Append(ex));
                if (getSession().Transaction.IsActive)
                {
                    getSession().Transaction.Rollback();
                }
                mensajeResultado.noError = ControlErroresEntity.buscaNoErrorPorExcepcion(ex);
                mensajeResultado.error = ex.GetBaseException().ToString();
                mensajeResultado.resultado = null;
            }
            return mensajeResultado;
        }

        public Mensaje createReportExcel(Dictionary<string, object> filtros, IList<object> listData)
        {
            bool addTotals = false;
            if (filtros.ContainsKey("withTotals"))
            {
                addTotals = (bool)filtros["withTotals"];
            }

            List<List<object>> listPercepcion = new List<List<object>>();
            List<List<object>> listDeduccion = new List<List<object>>();

            foreach (var data in listData)
            {
                List<object> getData = new List<object>();
                Dictionary<string, object> values = (Dictionary<string, object>)data;
                getData.Add(values["claveConcepNomDefi"]);
                getData.Add(values["descripcionConcepNomDefi"]);
                getData.Add(values["regPatronal"]);
                getData.Add(values["importe"]);

                int naturaleza = (int)(dynamic)values["naturalezaConcepNomDefi"];
                if (naturaleza == 1)
                {
                    listPercepcion.Add(getData);
                }
                else if (naturaleza == 2)
                {
                    listDeduccion.Add(getData);
                }
            }


            //Simular mas regs de deduccion que de percepcion
            //List<object> adder = new List<object>();
            //adder.Add("1080");
            //adder.Add("ATR");
            //adder.Add("TTYPOKP88");
            //adder.Add(90.50);
            //listDeduccion.Add(adder);
            //adder = new List<object>();
            //adder.Add("1045");
            //adder.Add("JPY");
            //adder.Add("HEUITD994");
            //adder.Add(700.20);
            //listDeduccion.Add(adder);

            var groupPercepList = listPercepcion.GroupBy(x => new { claveCnc = x[0], desCnc = x[1] }).ToList();

            List<List<object>> listToPercep = new List<List<object>>();
            List<string> countRegByPercep = new List<string>();
            foreach (var dataGroupPerce in groupPercepList)
            {
                List<object> dataPerce = new List<object>();
                dataPerce.Add("CNC:KEY " + dataGroupPerce.Key.claveCnc);
                dataPerce.Add("CNC:DES " + dataGroupPerce.Key.desCnc);

                foreach (var data in listPercepcion)
                {
                    List<object> getData = data;

                    if (dataGroupPerce.Key.claveCnc.Equals(getData[0]))
                    {
                        if (!countRegByPercep.Contains("REG:NAME " + getData[2]))
                        {
                            countRegByPercep.Add("REG:NAME " + getData[2]);
                        }
                        dataPerce.Add("REG:NAME " + getData[2]);
                        dataPerce.Add("REG:VAL " + getData[3]);
                    }
                }

                listToPercep.Add(dataPerce);
            }

            var groupDeducList = listDeduccion.GroupBy(x => new { claveCnc = x[0], desCnc = x[1] }).ToList();

            List<List<object>> listToDeduc = new List<List<object>>();
            List<string> countRegByDeduc = new List<string>();
            foreach (var dataGroupDeduc in groupDeducList)
            {
                List<object> dataDeduc = new List<object>();
                dataDeduc.Add("CNC:KEY " + dataGroupDeduc.Key.claveCnc);
                dataDeduc.Add("CNC:DES " + dataGroupDeduc.Key.desCnc);

                foreach (var data in listDeduccion)
                {
                    List<object> getData = data;

                    if (dataGroupDeduc.Key.claveCnc.Equals(getData[0]))
                    {
                        if (!countRegByDeduc.Contains("REG:NAME " + getData[2]))
                        {
                            countRegByDeduc.Add("REG:NAME " + getData[2]);
                        }

                        dataDeduc.Add("REG:NAME " + getData[2]);
                        dataDeduc.Add("REG:VAL " + getData[3]);
                    }
                }

                listToDeduc.Add(dataDeduc);
            }


            //Merge Reg Pat
            List<string> uRegPat = countRegByPercep.Union(countRegByDeduc).ToList();

            Excel.Application xlApp = new
            Excel.Application();

            xlApp.DisplayAlerts = false;

            if (xlApp == null)
            {
                //MessageBox.Show("Excel is not properly installed!!");
                return null;
            }


            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);


            //********* PERCEPCIONES *********//
            //Columns
            var cellCncPer = (Excel.Range)xlWorkSheet.Cells[7, 1];
            cellCncPer.Value2 = "Concepto";
            cellCncPer.Font.Bold = true;
            cellCncPer.RowHeight = 30;
            cellCncPer.ColumnWidth = 12.2;
            cellCncPer.VerticalAlignment = XlHAlign.xlHAlignCenter;
            cellCncPer.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            cellCncPer.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            cellCncPer.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            cellCncPer.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            cellCncPer.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            cellCncPer.Interior.Color = Excel.XlRgbColor.rgbYellow;

            var cellCncDes = (Excel.Range)xlWorkSheet.Cells[7, 2];
            cellCncDes.Value2 = "Descripción";
            cellCncDes.Font.Bold = true;
            cellCncDes.RowHeight = 30;
            cellCncDes.ColumnWidth = 35.3;
            cellCncDes.VerticalAlignment = XlHAlign.xlHAlignCenter;
            cellCncDes.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            cellCncDes.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDes.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDes.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDes.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDes.Interior.Color = Excel.XlRgbColor.rgbYellow;

            int fromColRegPatPerce = 3;
            Dictionary<string, int> postRegPatronalesPerce = new Dictionary<string, int>();

            //Create titles headers Reg pat
            for (int hReg = 0; hReg < uRegPat.Count; hReg++)
            {
                var valueRegPat = uRegPat[hReg].ToString().Replace("REG:NAME ", "");
                postRegPatronalesPerce[valueRegPat] = fromColRegPatPerce;
                var cellRegsPats = (Excel.Range)xlWorkSheet.Cells[7, fromColRegPatPerce];
                cellRegsPats.Value2 = valueRegPat;
                cellRegsPats.Font.Bold = true;
                cellRegsPats.RowHeight = 40;
                cellRegsPats.ColumnWidth = 15;
                cellRegsPats.VerticalAlignment = XlHAlign.xlHAlignCenter;
                cellRegsPats.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Interior.Color = Excel.XlRgbColor.rgbYellow;

                fromColRegPatPerce++;
            }

            var cellTotalPercep = (Excel.Range)xlWorkSheet.Cells[7, fromColRegPatPerce];
            cellTotalPercep.Value2 = "Total";
            cellTotalPercep.Font.Bold = true;
            cellTotalPercep.RowHeight = 30;
            cellTotalPercep.ColumnWidth = 15;
            cellTotalPercep.VerticalAlignment = XlHAlign.xlHAlignCenter;
            cellTotalPercep.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            cellTotalPercep.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalPercep.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalPercep.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalPercep.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalPercep.Interior.Color = Excel.XlRgbColor.rgbYellow;


            //Data all Percep
            int startRowPer = 8;
            int actualRow = startRowPer;//Start Regs
            Dictionary<int, decimal> totalByColumn = new Dictionary<int, decimal>();
            List<int> valuesbyReg = new List<int>();
            for (int i = 0; i < listToPercep.Count; i++)
            {
                List<object> getData = listToPercep[i];
                int totalCol = 1;
                decimal valTotal = 0M;

                for (int g = 0; g < getData.Count; g++)
                {
                    if (getData[g].ToString().StartsWith("CNC:KEY"))
                    {
                        var cellValCncKey = (Excel.Range)xlWorkSheet.Cells[actualRow, (totalCol)];
                        cellValCncKey.Value2 = getData[g].ToString().Replace("CNC:KEY ", "");
                        cellValCncKey.ColumnWidth = 12.2;
                        cellValCncKey.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        totalCol++;
                    }

                    else if (getData[g].ToString().StartsWith("CNC:DES"))
                    {
                        var cellValCncDes = (Excel.Range)xlWorkSheet.Cells[actualRow, (totalCol)];
                        cellValCncDes.Value2 = getData[g].ToString().Replace("CNC:DES ", "");
                        cellValCncDes.ColumnWidth = 35.3;
                        cellValCncDes.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        totalCol++;
                    }
                    else if (getData[g].ToString().StartsWith("REG:VAL"))
                    {
                        int getPosValReg = postRegPatronalesPerce[getData[(g - 1)].ToString().Replace("REG:NAME ", "")];
                        var cellValRegName = (Excel.Range)xlWorkSheet.Cells[actualRow, (getPosValReg)];
                        decimal importVal = Convert.ToDecimal(getData[g].ToString().Replace("REG:VAL ", ""));
                        cellValRegName.NumberFormat = "#,##0.00";
                        cellValRegName.Value2 = importVal;
                        if (!valuesbyReg.Contains(getPosValReg))
                        {
                            valuesbyReg.Add(getPosValReg);
                        }
                        if (totalByColumn.ContainsKey(getPosValReg))
                        {
                            decimal getValCol = totalByColumn[getPosValReg];
                            getValCol += importVal;
                            totalByColumn[getPosValReg] = getValCol;
                        }
                        else
                        {
                            totalByColumn.Add(getPosValReg, importVal);
                        }

                        valTotal += importVal;
                        cellValRegName.ColumnWidth = 15;
                        cellValRegName.HorizontalAlignment = XlHAlign.xlHAlignRight;
                        totalCol++;
                    }

                }

                actualRow++;
            }

            int minCol = valuesbyReg.Min();
            int maxCol = postRegPatronalesPerce.Values.Max();
            //8: Start Regs
            for (int tRowReg = startRowPer; tRowReg < actualRow; tRowReg++)
            {
                //Total apply formula by each Cnc & Reg. pat.
                var cellTotalReg = (Excel.Range)xlWorkSheet.Cells[tRowReg, (maxCol + 1)];
                cellTotalReg.NumberFormat = "#,##0.00";
                cellTotalReg.Formula = "=Sum(" + xlWorkSheet.Cells[tRowReg, minCol].Address +
                ":" + xlWorkSheet.Cells[tRowReg, maxCol].Address + ")";
                cellTotalReg.ColumnWidth = 15;
                cellTotalReg.HorizontalAlignment = XlHAlign.xlHAlignRight;
            }

            List<int[]> globalTotalPer = new List<int[]>();

            if (addTotals)
            {
                //Total apply formula by each Reg. pat
                for (int eachColPer = minCol; eachColPer <= (maxCol + 1); eachColPer++)
                {
                    var cellTotalByEachReg = (Excel.Range)xlWorkSheet.Cells[(actualRow), (eachColPer)];
                    globalTotalPer.Add(new[] { actualRow, eachColPer });
                    cellTotalByEachReg.Font.Bold = true;
                    cellTotalByEachReg.NumberFormat = "#,##0.00";
                    cellTotalByEachReg.Formula = "=Sum(" + xlWorkSheet.Cells[startRowPer, eachColPer].Address +
                    ":" + xlWorkSheet.Cells[(actualRow - 1), eachColPer].Address + ")";
                    cellTotalByEachReg.HorizontalAlignment = XlHAlign.xlHAlignRight;
                    cellTotalByEachReg.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                    cellTotalByEachReg.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;

                }

                //Style Center values percep
                Excel.Range styleAllCenterValPer = xlWorkSheet.Range[
                    xlWorkSheet.Cells[(startRowPer), 1].Address, xlWorkSheet.Cells[(actualRow), (maxCol + 1)].Address];
                styleAllCenterValPer.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                styleAllCenterValPer.Borders.Weight = Excel.XlBorderWeight.xlThin;



                //Text of total by by each Reg. pat
                Excel.Range styleTotEachRegPer = xlWorkSheet.Range[
                    xlWorkSheet.Cells[(actualRow), 1].Address, xlWorkSheet.Cells[(actualRow), 2].Address];
                String valueDesTotal = "TOTAL PERCEPCIONES";
                styleTotEachRegPer.Value2 = valueDesTotal;
                styleTotEachRegPer.Font.Bold = true;
                styleTotEachRegPer.Select();
                styleTotEachRegPer.Merge(Missing.Value);

                Excel.Range styleAllTotEachRegPer = xlWorkSheet.Range[
                    xlWorkSheet.Cells[(actualRow), 1].Address, xlWorkSheet.Cells[(actualRow), (maxCol + 1)].Address];
                styleAllTotEachRegPer.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegPer.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegPer.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegPer.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegPer.Interior.Color = Excel.XlRgbColor.rgbYellow;
            }
            else
            {
                Excel.Range styleAllCenterValPer = xlWorkSheet.Range[
                xlWorkSheet.Cells[(startRowPer), 1].Address, xlWorkSheet.Cells[(actualRow - 1), (maxCol + 1)].Address];
                styleAllCenterValPer.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                styleAllCenterValPer.Borders.Weight = Excel.XlBorderWeight.xlThin;
            }

            //Header based on las column
            Excel.Range styleHeaderPer = xlWorkSheet.Range[
               xlWorkSheet.Cells[6, 1].Address, xlWorkSheet.Cells[6, (maxCol + 1)].Address];
            String repetitiveValue = "PERCEPCIONES";
            styleHeaderPer.Value2 = repetitiveValue;
            styleHeaderPer.Font.Bold = true;
            styleHeaderPer.Select();
            styleHeaderPer.Merge(Missing.Value);
            styleHeaderPer.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            styleHeaderPer.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            styleHeaderPer.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            styleHeaderPer.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            //Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
            //ws.Cells["A1:B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //ws.Cells["A1:B1"].Style.Fill.BackgroundColor.SetColor(colFromHex);
            styleHeaderPer.RowHeight = 12;
            // range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            styleHeaderPer.Interior.Color = Excel.XlRgbColor.rgbYellow;


            //********* DEDUCCIONES *********//
            //Columns
            int addExtraRowForHeader = 0;
            int addExtraRowForActualRow = 0;
            if (addTotals)
            {
                addExtraRowForHeader = 2;
                addExtraRowForActualRow = 3;
            }
            else
            {
                addExtraRowForHeader = 1;
                addExtraRowForActualRow = 2;
            }

            int headerDeduc = (actualRow + addExtraRowForHeader);
            actualRow = (actualRow + addExtraRowForActualRow);
            // actualRow = startRowDeduc;
            var cellCncDeduc = (Excel.Range)xlWorkSheet.Cells[actualRow, 1];
            cellCncDeduc.Value2 = "Concepto";
            cellCncDeduc.Font.Bold = true;
            cellCncDeduc.RowHeight = 30;
            cellCncDeduc.ColumnWidth = 12.2;
            cellCncDeduc.VerticalAlignment = XlHAlign.xlHAlignCenter;
            cellCncDeduc.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            cellCncDeduc.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeduc.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeduc.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeduc.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeduc.Interior.Color = Excel.XlRgbColor.rgbYellow;

            var cellCncDeducDes = (Excel.Range)xlWorkSheet.Cells[actualRow, 2];
            cellCncDeducDes.Value2 = "Descripción";
            cellCncDeducDes.Font.Bold = true;
            cellCncDeducDes.RowHeight = 30;
            cellCncDeducDes.ColumnWidth = 35.3;
            cellCncDeducDes.VerticalAlignment = XlHAlign.xlHAlignCenter;
            cellCncDeducDes.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            cellCncDeducDes.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeducDes.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeducDes.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeducDes.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            cellCncDeducDes.Interior.Color = Excel.XlRgbColor.rgbYellow;


            //Columns Reg Pat Deducciones
            int fromColRegPatDeduc = 3;
            Dictionary<string, int> postRegPatronalesDeduc = new Dictionary<string, int>();

            //Create titles headers Reg pat
            for (int hReg = 0; hReg < uRegPat.Count; hReg++)
            {
                var valueRegPat = uRegPat[hReg].ToString().Replace("REG:NAME ", "");
                postRegPatronalesDeduc[valueRegPat] = fromColRegPatDeduc;
                var cellRegsPats = (Excel.Range)xlWorkSheet.Cells[actualRow, fromColRegPatDeduc];
                cellRegsPats.Value2 = valueRegPat;
                cellRegsPats.Font.Bold = true;
                cellRegsPats.RowHeight = 40;
                cellRegsPats.ColumnWidth = 15;
                cellRegsPats.VerticalAlignment = XlHAlign.xlHAlignCenter;
                cellRegsPats.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
                cellRegsPats.Interior.Color = Excel.XlRgbColor.rgbYellow;

                fromColRegPatDeduc++;
            }

            var cellTotalDeduc = (Excel.Range)xlWorkSheet.Cells[actualRow, fromColRegPatDeduc];
            cellTotalDeduc.Value2 = "Total";
            cellTotalDeduc.Font.Bold = true;
            cellTotalDeduc.RowHeight = 30;
            cellTotalDeduc.ColumnWidth = 15;
            cellTotalDeduc.VerticalAlignment = XlHAlign.xlHAlignCenter;
            cellTotalDeduc.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            cellTotalDeduc.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalDeduc.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalDeduc.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalDeduc.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            cellTotalDeduc.Interior.Color = Excel.XlRgbColor.rgbYellow;

            //Data all Deduc
            actualRow++;
            int startRowDeduc = actualRow;
            Dictionary<int, decimal> totalByColumnDeduc = new Dictionary<int, decimal>();
            List<int> valuesbyRegDeduc = new List<int>();

            for (int i = 0; i < listToDeduc.Count; i++)
            {
                List<object> getData = listToDeduc[i];
                int totalCol = 1;
                decimal valTotal = 0M;

                for (int g = 0; g < getData.Count; g++)
                {
                    if (getData[g].ToString().StartsWith("CNC:KEY"))
                    {
                        var cellValCncKey = (Excel.Range)xlWorkSheet.Cells[actualRow, (totalCol)];
                        cellValCncKey.Value2 = getData[g].ToString().Replace("CNC:KEY ", "");
                        cellValCncKey.ColumnWidth = 12.2;
                        cellValCncKey.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        totalCol++;
                    }

                    else if (getData[g].ToString().StartsWith("CNC:DES"))
                    {
                        var cellValCncDes = (Excel.Range)xlWorkSheet.Cells[actualRow, (totalCol)];
                        cellValCncDes.Value2 = getData[g].ToString().Replace("CNC:DES ", "");
                        cellValCncDes.ColumnWidth = 35.3;
                        cellValCncDes.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        totalCol++;
                    }
                    else if (getData[g].ToString().StartsWith("REG:VAL"))
                    {
                        int getPosValReg = postRegPatronalesDeduc[getData[(g - 1)].ToString().Replace("REG:NAME ", "")];
                        var cellValRegName = (Excel.Range)xlWorkSheet.Cells[actualRow, (getPosValReg)];
                        decimal importVal = Convert.ToDecimal(getData[g].ToString().Replace("REG:VAL ", ""));
                        cellValRegName.NumberFormat = "#,##0.00";  
                        cellValRegName.Value2 = importVal;
                        if (!valuesbyRegDeduc.Contains(getPosValReg))
                        {
                            valuesbyRegDeduc.Add(getPosValReg);
                        }
                        if (totalByColumnDeduc.ContainsKey(getPosValReg))
                        {
                            decimal getValCol = totalByColumnDeduc[getPosValReg];
                            getValCol += importVal;
                            totalByColumnDeduc[getPosValReg] = getValCol;
                        }
                        else
                        {
                            totalByColumnDeduc.Add(getPosValReg, importVal);
                        }

                        valTotal += importVal;
                        cellValRegName.ColumnWidth = 15;
                        cellValRegName.HorizontalAlignment = XlHAlign.xlHAlignRight;
                        totalCol++;
                    }

                }

                actualRow++;
            }

            int minColDeduc = valuesbyRegDeduc.Min();
            int maxColDeduc = postRegPatronalesDeduc.Values.Max();

            //Total apply formula by each Cnc & Reg. pat.
            for (int tRowReg = startRowDeduc; tRowReg < actualRow; tRowReg++)
            {
                var cellTotalReg = (Excel.Range)xlWorkSheet.Cells[tRowReg, (maxColDeduc + 1)];
                cellTotalReg.NumberFormat = "#,##0.00";  
                cellTotalReg.Formula = "=Sum(" + xlWorkSheet.Cells[tRowReg, minColDeduc].Address +
                ":" + xlWorkSheet.Cells[tRowReg, maxColDeduc].Address + ")";
                cellTotalReg.ColumnWidth = 15;
                cellTotalReg.HorizontalAlignment = XlHAlign.xlHAlignRight;
            }

            //Total apply formula by each Reg. pat
            List<int[]> globalTotalDeduc = new List<int[]>();

            if (addTotals)
            {
                for (int eachColPer = minColDeduc; eachColPer <= (maxColDeduc + 1); eachColPer++)
                {
                    var cellTotalByEachReg = (Excel.Range)xlWorkSheet.Cells[(actualRow), (eachColPer)];
                    globalTotalDeduc.Add(new[] { actualRow, eachColPer });
                    cellTotalByEachReg.Font.Bold = true;
                    cellTotalByEachReg.NumberFormat = "#,##0.00";
                    cellTotalByEachReg.Formula = "=Sum(" + xlWorkSheet.Cells[startRowDeduc, eachColPer].Address +
                    ":" + xlWorkSheet.Cells[(actualRow - 1), eachColPer].Address + ")";
                    cellTotalByEachReg.HorizontalAlignment = XlHAlign.xlHAlignRight;
                    cellTotalByEachReg.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                    cellTotalByEachReg.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                }

                //Style Center values deduc
                Excel.Range styleAllCenterValDeduc = xlWorkSheet.Range[
                xlWorkSheet.Cells[(startRowDeduc), 1].Address, xlWorkSheet.Cells[(actualRow), (maxColDeduc + 1)].Address];
                styleAllCenterValDeduc.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                styleAllCenterValDeduc.Borders.Weight = Excel.XlBorderWeight.xlThin;

                //Text of total by each Reg. pat
                Excel.Range styleTotEachRegDeduc = xlWorkSheet.Range[
                    xlWorkSheet.Cells[(actualRow), 1].Address, xlWorkSheet.Cells[(actualRow), 2].Address];
                String valueDesDeducTotal = "TOTAL DEDUCCIONES";
                styleTotEachRegDeduc.Value2 = valueDesDeducTotal;
                styleTotEachRegDeduc.Font.Bold = true;
                styleTotEachRegDeduc.Select();
                styleTotEachRegDeduc.Merge(Missing.Value);


                Excel.Range styleAllTotEachRegDeduc = xlWorkSheet.Range[
                    xlWorkSheet.Cells[(actualRow), 1].Address, xlWorkSheet.Cells[(actualRow), (maxColDeduc + 1)].Address];
                styleAllTotEachRegDeduc.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegDeduc.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegDeduc.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegDeduc.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
                styleAllTotEachRegDeduc.Interior.Color = Excel.XlRgbColor.rgbYellow;

            }
            else
            {
                Excel.Range styleAllCenterValDeduc = xlWorkSheet.Range[
                xlWorkSheet.Cells[(startRowDeduc), 1].Address, xlWorkSheet.Cells[(actualRow - 1), (maxColDeduc + 1)].Address];
                styleAllCenterValDeduc.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                styleAllCenterValDeduc.Borders.Weight = Excel.XlBorderWeight.xlThin;
            }

            //Header based on last column
            Excel.Range styleHeaderDeduc = xlWorkSheet.Range[
               xlWorkSheet.Cells[headerDeduc, 1].Address, xlWorkSheet.Cells[headerDeduc, (maxColDeduc + 1)].Address];
            String repetitiveValueDeduc = "DEDUCCIONES";
            styleHeaderDeduc.Value2 = repetitiveValueDeduc;
            styleHeaderDeduc.Font.Bold = true;
            styleHeaderDeduc.Select();
            styleHeaderDeduc.Merge(Missing.Value);
            styleHeaderDeduc.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            styleHeaderDeduc.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            styleHeaderDeduc.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            styleHeaderDeduc.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            //Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
            //ws.Cells["A1:B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //ws.Cells["A1:B1"].Style.Fill.BackgroundColor.SetColor(colFromHex);
            styleHeaderDeduc.RowHeight = 12;
            // range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            styleHeaderDeduc.Interior.Color = Excel.XlRgbColor.rgbYellow;


            if (addTotals)
            {
                //Global Total
                actualRow = (actualRow + 2);
                int getGlobals = 0;
                for (int eachColGlobalTotal = minCol; eachColGlobalTotal <= (maxCol + 1); eachColGlobalTotal++)
                {
                    var cellGlobalTotal = (Excel.Range)xlWorkSheet.Cells[(actualRow), (eachColGlobalTotal)];
                    globalTotalDeduc.Add(new[] { actualRow, eachColGlobalTotal });
                    cellGlobalTotal.Font.Bold = true;
                    int[] getAddresPer = (int[])globalTotalPer[getGlobals];
                    int[] getAddresDeduc = (int[])globalTotalDeduc[getGlobals];
                    cellGlobalTotal.NumberFormat = "#,##0.00";
                    cellGlobalTotal.Formula = "=" + xlWorkSheet.Cells[getAddresPer[0], getAddresPer[1]].Address +
                    "-" + xlWorkSheet.Cells[getAddresDeduc[0], getAddresDeduc[1]].Address + " ";
                    cellGlobalTotal.HorizontalAlignment = XlHAlign.xlHAlignRight;
                    cellGlobalTotal.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                    cellGlobalTotal.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                    getGlobals++;
                }

                //Text of global total's
                Excel.Range styleGlobalTot = xlWorkSheet.Range[
                    xlWorkSheet.Cells[(actualRow), 1].Address, xlWorkSheet.Cells[(actualRow), 2].Address];
                String valueDesGlobalTotal = "TOTAL NETO";
                styleGlobalTot.Value2 = valueDesGlobalTotal;
                styleGlobalTot.Font.Bold = true;
                styleGlobalTot.Select();
                styleGlobalTot.Merge(Missing.Value);


                Excel.Range styleAllGlobalTot = xlWorkSheet.Range[
                    xlWorkSheet.Cells[(actualRow), 1].Address, xlWorkSheet.Cells[(actualRow), (maxCol + 1)].Address];
                styleAllGlobalTot.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                styleAllGlobalTot.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                styleAllGlobalTot.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
                styleAllGlobalTot.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
                styleAllGlobalTot.Interior.Color = Excel.XlRgbColor.rgbYellow;
            }

            //Create Main Headers
            //Name company
            Excel.Range styleHeaderNombreEmpresa = xlWorkSheet.Range[
              xlWorkSheet.Cells[1, 1].Address, xlWorkSheet.Cells[1, (maxCol + 1)].Address];
            String valueDesNombreEmpresa = "----";
            if (filtros.ContainsKey("NombreEmpresa"))
            {
                valueDesNombreEmpresa = (string)filtros["NombreEmpresa"];
            }
            styleHeaderNombreEmpresa.Value2 = valueDesNombreEmpresa;
            styleHeaderNombreEmpresa.Font.Bold = true;
            styleHeaderNombreEmpresa.Select();
            styleHeaderNombreEmpresa.Merge(Missing.Value);
            styleHeaderNombreEmpresa.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            styleHeaderNombreEmpresa.RowHeight = 12;
            styleHeaderNombreEmpresa.Font.Color = ColorTranslator.ToOle(Color.White);
            styleHeaderNombreEmpresa.Interior.Color = Excel.XlRgbColor.rgbDarkBlue;

            //Addres company
            Excel.Range styleHeaderDomicilioEmpresa = xlWorkSheet.Range[
              xlWorkSheet.Cells[2, 1].Address, xlWorkSheet.Cells[2, (maxCol + 1)].Address];
            String valueDesDomicilioEmpresa = "DOMICILIO EMPRESA";
            if (filtros.ContainsKey("DomicilioEmpresa"))
            {
                valueDesDomicilioEmpresa = (string)filtros["DomicilioEmpresa"];
            }
            styleHeaderDomicilioEmpresa.Value2 = valueDesDomicilioEmpresa;
            styleHeaderDomicilioEmpresa.Font.Bold = true;
            styleHeaderDomicilioEmpresa.Select();
            styleHeaderDomicilioEmpresa.Merge(Missing.Value);
            styleHeaderDomicilioEmpresa.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            styleHeaderDomicilioEmpresa.RowHeight = 12;
            styleHeaderDomicilioEmpresa.Font.Color = ColorTranslator.ToOle(Color.White);
            styleHeaderDomicilioEmpresa.Interior.Color = Excel.XlRgbColor.rgbDarkBlue;

            //Period company
            Excel.Range styleHeaderPeriodo = xlWorkSheet.Range[
              xlWorkSheet.Cells[3, 1].Address, xlWorkSheet.Cells[3, (maxCol + 1)].Address];
            String valueDesPeriodo = "";
            if (filtros.ContainsKey("DescripcionPeriodo"))
            {
                valueDesPeriodo = (string)filtros["DescripcionPeriodo"];
            }
            styleHeaderPeriodo.Value2 = valueDesPeriodo;
            styleHeaderPeriodo.Font.Bold = true;
            styleHeaderPeriodo.Select();
            styleHeaderPeriodo.Merge(Missing.Value);
            styleHeaderPeriodo.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            styleHeaderPeriodo.RowHeight = 12;
            styleHeaderPeriodo.Font.Color = ColorTranslator.ToOle(Color.White);
            styleHeaderPeriodo.Interior.Color = Excel.XlRgbColor.rgbDarkBlue;

            //Space header
            for (int spaceRowHeader = 4; spaceRowHeader < 6; spaceRowHeader++)
            {
                Excel.Range styleHeaderSpace = xlWorkSheet.Range[
                xlWorkSheet.Cells[spaceRowHeader, 1].Address, xlWorkSheet.Cells[spaceRowHeader, (maxCol + 1)].Address];
                styleHeaderSpace.Font.Bold = true;
                styleHeaderSpace.Select();
                styleHeaderSpace.Merge(Missing.Value);
                styleHeaderSpace.RowHeight = 12;
                styleHeaderSpace.Interior.Color = Excel.XlRgbColor.rgbDarkBlue;

            }

            //SET BYTES
            mensajeResultado.resultado = GetActiveWorkbook(xlApp);
            mensajeResultado.noError = 0;
            mensajeResultado.error = "";

            return mensajeResultado;



            //xlWorkBook.SaveAs("c:\\Users\\jevalenzuela\\Documents\\test2.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            //xlWorkBook.Close(true, misValue, misValue);
            //xlApp.Quit();
            //return null;

        }


        private string GetActiveWorkbook(Excel.Application app)
        {
            string path = Path.GetTempFileName();
            try
            {
                app.ActiveWorkbook.SaveCopyAs(path);
                byte[] buffer = File.ReadAllBytes(path);
                return Convert.ToBase64String(buffer);
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}


