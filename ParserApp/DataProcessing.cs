using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using OfficeOpenXml;

namespace ParserApp
{
    class DataProcessing : IDownloadable, IUpdatable
    {
        public static string pathForThrList = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Lab2", "ParserApp", "ParserApp", "bin", "Debug", "thrlist.xlsx"); //путь для скачивания базы данных угроз безопасности
        public static string pathForRefreshedThrList = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "thrlist.xlsx"); //путь для скачивания обновлённой базы данных угроз безопасности

        public static List<Threat> listOfThreats = GetListOfThreats(); //список угроз безопасности
        public static List<Threat> listOfThreatsBefore = new List<Threat>(); //список угроз безопасности до обвноления, которые в результате обновления были изменены
        public static List<Threat> listOfThreatsAfter = new List<Threat>(); //список изменённых во время обновления угроз

        public void DownloadTableWithThreats(string fileName) //метод загрузки данных с сайта ФСТЭК РФ
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://bdu.fstec.ru/documents/files/thrlist.xlsx", fileName);
            }
        }

        public static List<Threat> GetListOfThreats() //метод получения списка угроз
        {
            if (File.Exists(pathForThrList))
            {
                listOfThreats = ParserOfThreats(pathForThrList);
            }
            else
            {
                if (MessageBox.Show("Файл \"thrlist.xlsx\" с локальной базой данных не найден.", "Провести первичную загрузку данных?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        new DataProcessing().DownloadTableWithThreats("thrlist.xlsx");
                        MessageBox.Show("Файл успешно загружен в " + pathForThrList, "Уведомление");
                        listOfThreats = ParserOfThreats(pathForThrList);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.ToString() + "\nПроверьте Интернет-соединение и повторите попытку.\n", "Ошибка");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            return listOfThreats;
        }

        public static List<Threat> ParserOfThreats(string fileName) //парсер угроз
        {
            List<Threat> list = new List<Threat>();
            FileInfo fileInfo = new FileInfo(fileName);

            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                if (File.Exists(pathForRefreshedThrList))
                {
                    File.Delete(pathForThrList);
                    FileInfo fl = new FileInfo(pathForRefreshedThrList);
                    fl.CopyTo(pathForThrList);
                    File.Delete(pathForRefreshedThrList);
                }

                int rowCount = workSheet.Dimension.End.Row - workSheet.Dimension.Start.Row - 1;

                for (int i = 3; i < rowCount + 3; i++)
                {
                    if (workSheet.Cells[i, 1].Value == null && workSheet.Cells[i, 2].Value == null && workSheet.Cells[i, 3].Value == null && workSheet.Cells[i, 4].Value == null && workSheet.Cells[i, 5].Value == null && workSheet.Cells[i, 6].Value == null && workSheet.Cells[i, 7].Value == null && workSheet.Cells[i, 8].Value == null)
                        continue;
                    else
                    {
                        try
                        {
                            //заполнение одной строки таблицы: в случае, если какое-длибо поле пустое, туда помещается знак "-"
                            list.Add(new Threat(workSheet.Cells[i, 1].Value == null ? "-" : workSheet.Cells[i, 1].Value.ToString(),
                            workSheet.Cells[i, 2].Value == null ? "-" : workSheet.Cells[i, 2].Value.ToString(),
                            workSheet.Cells[i, 3].Value == null ? "-" : workSheet.Cells[i, 3].Value.ToString(),
                            workSheet.Cells[i, 4].Value == null ? "-" : workSheet.Cells[i, 4].Value.ToString(),
                            workSheet.Cells[i, 5].Value == null ? "-" : workSheet.Cells[i, 5].Value.ToString(),
                            Convert.ToBoolean(workSheet.Cells[i, 6].Value),
                            Convert.ToBoolean(workSheet.Cells[i, 7].Value),
                            Convert.ToBoolean(workSheet.Cells[i, 8].Value)));
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Поле/поля угроз(ы) не заполнено(ы). Проверьте правильность заполнения полей и повторите попытку.", "Предупреждение");
                            Environment.Exit(0);
                        }
                    }
                }

                foreach (var s in list)
                {
                    s.Name = s.Name.Replace("\r", "");
                    s.Name = s.Name.Replace("_x000d_", "");
                    s.Description = s.Description.Replace("\r", "");
                    s.Description = s.Description.Replace("_x000d_", "");
                    s.ThreatSource = s.ThreatSource.Replace("\r", "");
                    s.ThreatSource = s.ThreatSource.Replace("_x000d_", "");
                    s.InteractionObject = s.InteractionObject.Replace("\r", "");
                    s.InteractionObject = s.InteractionObject.Replace("_x000d_", "");
                }
            }
            return list;
        }

        public List<Threat> RefreshTableWithThreats(out string refreshedInfo) //метод обновления списка угроз
        {
            List<Threat> refreshedListOfThreats = new List<Threat>();
            bool isTableRefreshed = false;
            refreshedInfo = "";

            try
            {
                DownloadTableWithThreats(pathForRefreshedThrList);
                isTableRefreshed = true;
                refreshedInfo += "Cтатус обновления: Успешно\n";
                MessageBox.Show("Файл успешно обновлён.", "Уведомление");
            }
            catch (Exception ex)
            {
                isTableRefreshed = false;
                refreshedInfo += "Cтатус обновления: Ошибка\n" + ex.ToString() + "\nПроверьте Интернет-соединение и повторите попытку.\n";
            }

            if (isTableRefreshed == true)
            {
                refreshedListOfThreats = ParserOfThreats(pathForRefreshedThrList);

                listOfThreatsBefore = listOfThreats.Except(refreshedListOfThreats).ToList(); //получение списка со старыми угрозами, которые притерпели изменения при обновлении
                listOfThreatsAfter = refreshedListOfThreats.Except(listOfThreats).ToList(); //получение списка с обновленными угрозами

                listOfThreats = refreshedListOfThreats;
            }
            else
                refreshedListOfThreats = listOfThreats;

            return refreshedListOfThreats;
        }

        public static string GetDiffrentThreatProperties(out int count) //получение строки, содержащей информацию о всех обновлённых угрозах в формате БЫЛО/СТАЛО
        {
            string s = "";
            count = 0;
            string subRefreshedInfoBebore = "\n\nБЫЛО:\n";
            string subRefreshedInfoAfter = "\nСТАЛО:\n";

            if (listOfThreatsBefore.Count == listOfThreatsAfter.Count)
            {
                for (int i = 0; i < listOfThreatsBefore.Count; i++)
                {
                    s += GetOneChangedStringThreat(i, subRefreshedInfoBebore, subRefreshedInfoAfter);
                    count++;
                }

            }

            else if(listOfThreatsBefore.Count < listOfThreatsAfter.Count)
            {
                if (listOfThreatsBefore.Count == 0)
                {
                    foreach (Threat thr in listOfThreatsAfter)
                    {
                        s += "***\n" + thr.Id;
                        subRefreshedInfoBebore += " - \n";
                        subRefreshedInfoAfter += thr.ToString();
                        s += subRefreshedInfoBebore + subRefreshedInfoAfter + "***\n";
                        count++;
                        subRefreshedInfoBebore = "\n\nБЫЛО:\n";
                        subRefreshedInfoAfter = "\nСТАЛО:\n";
                    }
                }
                else
                {
                    for(int i = 0; i < listOfThreatsAfter.Count; i++)
                    {
                        if(listOfThreatsBefore.Any(thr => thr.Id == listOfThreatsAfter[i].Id))
                        {
                            s += GetOneChangedStringThreat(i, subRefreshedInfoBebore, subRefreshedInfoAfter);
                            count++;
                        }
                        else
                        {
                            s += "***\n" + listOfThreatsAfter[i].Id;
                            subRefreshedInfoBebore += " - \n";
                            subRefreshedInfoAfter += listOfThreatsAfter[i].ToString();
                            s += subRefreshedInfoBebore + subRefreshedInfoAfter + "***\n";
                            count++;
                            subRefreshedInfoBebore = "\n\nБЫЛО:\n";
                            subRefreshedInfoAfter = "\nСТАЛО:\n";
                        }
                    }
                }
            }
            else
            {
                if (listOfThreatsAfter.Count == 0)
                {
                    foreach (Threat thr in listOfThreatsBefore)
                    {
                        s += "***\n" + thr.Id;
                        subRefreshedInfoBebore += thr.ToString();
                        subRefreshedInfoAfter += " - \n";
                        s += subRefreshedInfoBebore + subRefreshedInfoAfter + "***\n";
                        count++;
                        subRefreshedInfoBebore = "\n\nБЫЛО:\n";
                        subRefreshedInfoAfter = "\nСТАЛО:\n";
                    }
                }
                else
                {
                    for (int i = 0; i < listOfThreatsBefore.Count; i++)
                    {
                        if (listOfThreatsAfter.Any(thr => thr.Id == listOfThreatsBefore[i].Id))
                        {
                            s += GetOneChangedStringThreat(i, subRefreshedInfoBebore, subRefreshedInfoAfter);
                            count++;
                        }
                        else
                        {
                            s += "***\n" + listOfThreatsAfter[i].Id;
                            subRefreshedInfoBebore += " - \n";
                            subRefreshedInfoAfter += listOfThreatsAfter[i].ToString();
                            s += subRefreshedInfoBebore + subRefreshedInfoAfter + "***\n";
                            count++;
                            subRefreshedInfoBebore = "\n\nБЫЛО:\n";
                            subRefreshedInfoAfter = "\nСТАЛО:\n";
                        }
                    }
                }
            }
            return s;
        }

        public static string GetOneChangedStringThreat(int i, string subRefreshedInfoBebore, string subRefreshedInfoAfter) //получение строки, содержащей информацию 
        {                                                                                                                  //об одной обновлённой угрозе в формате БЫЛО/СТАЛО
            string s = null;

            if (listOfThreatsBefore.Contains(listOfThreatsBefore[i]))                                                                                                           //об одной обновлённой угрозе в формате БЫЛО/СТАЛО
                s = "***\n" + listOfThreatsBefore[i].Id;
            else
            {
                for(int j = 0; j < listOfThreatsAfter.Count; j++)
                {
                    for (int k = i; k < listOfThreatsAfter.Count; k++)
                    {
                        if (listOfThreatsBefore[j] == listOfThreatsAfter[k])
                        {
                            s = "***\n" + listOfThreatsBefore[j].Id;
                        }
                    }
                }
            }

            if (listOfThreatsBefore[i].Id != listOfThreatsAfter[i].Id)
            {
                subRefreshedInfoBebore += "  Идентификатор УБИ: " + listOfThreatsBefore[i].Id + "\n";
                subRefreshedInfoAfter += "  Идентификатор УБИ: " + listOfThreatsAfter[i].Id + "\n";
            }
            if (listOfThreatsBefore[i].Name != listOfThreatsAfter[i].Name)
            {
                subRefreshedInfoBebore += "  Наименование УБИ: " + listOfThreatsBefore[i].Name + "\n";
                subRefreshedInfoAfter += "  Наименование УБИ: " + listOfThreatsAfter[i].Name + "\n";
            }
            if (listOfThreatsBefore[i].Description != listOfThreatsAfter[i].Description)
            {
                subRefreshedInfoBebore += "  Описание: " + listOfThreatsBefore[i].Description + "\n";
                subRefreshedInfoAfter += "  Описание: " + listOfThreatsAfter[i].Description + "\n";
            }
            if (listOfThreatsBefore[i].ThreatSource != listOfThreatsAfter[i].ThreatSource)
            {
                subRefreshedInfoBebore += "  Источник угрозы: " + listOfThreatsBefore[i].ThreatSource + "\n";
                subRefreshedInfoAfter += "  Источник угрозы: " + listOfThreatsAfter[i].ThreatSource + "\n";
            }
            if (listOfThreatsBefore[i].InteractionObject != listOfThreatsAfter[i].InteractionObject)
            {
                subRefreshedInfoBebore += "  Объект воздействия: " + listOfThreatsBefore[i].InteractionObject + "\n";
                subRefreshedInfoAfter += "  Объект воздействия: " + listOfThreatsAfter[i].InteractionObject + "\n";
            }
            if (listOfThreatsBefore[i].ConfidentialityBreach != listOfThreatsAfter[i].ConfidentialityBreach)
            {
                subRefreshedInfoBebore += "  Нарушение конфиденциальности: " + (listOfThreatsBefore[i].ConfidentialityBreach == true ? "да" : "нет") + "\n";
                subRefreshedInfoAfter += "  Нарушение конфиденциальности: " + (listOfThreatsAfter[i].ConfidentialityBreach == true ? "да" : "нет") + "\n";
            }
            if (listOfThreatsBefore[i].IntegrityBreach != listOfThreatsAfter[i].IntegrityBreach)
            {
                subRefreshedInfoBebore += "  Нарушение целостности: " + (listOfThreatsBefore[i].IntegrityBreach == true ? "да" : "нет") + "\n";
                subRefreshedInfoAfter += "  Нарушение целостности: " + (listOfThreatsAfter[i].IntegrityBreach == true ? "да" : "нет") + "\n";
            }
            if (listOfThreatsBefore[i].AccessBreach != listOfThreatsAfter[i].AccessBreach)
            {
                subRefreshedInfoBebore += "  Нарушение доступности: " + (listOfThreatsBefore[i].AccessBreach == true ? "да" : "нет") + "\n";
                subRefreshedInfoAfter += "  Нарушение доступности: " + (listOfThreatsAfter[i].AccessBreach == true ? "да" : "нет") + "\n";
            }
            s += subRefreshedInfoBebore + subRefreshedInfoAfter + "***\n";
            return s;
        }

        public static int PageCount() //метод, подсчитывающий общее количество страниц для вывода списка угроз по 15 штук на каждой
        {
            int pageCount = 0;
            int numberOfThreats = listOfThreats.Count;

            if (numberOfThreats % 15 == 0)
            {
                pageCount = numberOfThreats / 15;
            }

            else
            {
                pageCount = numberOfThreats / 15 + 1;
            }
            return pageCount;
        }

    }
}
