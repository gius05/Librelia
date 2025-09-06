using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Librelia.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.Reflection;
using static System.Reflection.Metadata.BlobBuilder;

namespace Librelia.Services
{

    public class RecordGeneratorService
    {
        public static byte[] ExportBooksToExcel(List<Book> books)
        {
            if (books == null || !books.Any())
                throw new ArgumentException("La lista è vuota o nulla.");

            using (var memoryStream = new MemoryStream())
            {
                // Creazione del workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Books");

                // Ottenere le proprietà della classe Book
                var properties = typeof(Book).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Creazione dell'intestazione
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < properties.Length; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(properties[i].Name);
                }

                // Aggiunta dei dati
                int rowIndex = 1;
                foreach (var book in books)
                {
                    IRow row = sheet.CreateRow(rowIndex++);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = properties[i].GetValue(book, null);

                        // Gestione delle liste (Tags, Authors) e di altri tipi complessi
                        if (value is IEnumerable<string> listValue)
                        {
                            row.CreateCell(i).SetCellValue(string.Join(", ", listValue));
                        }
                        else
                        {
                            row.CreateCell(i).SetCellValue(value?.ToString() ?? string.Empty);
                        }
                    }
                }

                // Scrittura nel MemoryStream
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }


        public static byte[] ExportReservationsToExcel(List<Reservation> reservations)
        {
            if (reservations == null || !reservations.Any())
                throw new ArgumentException("La lista è vuota o nulla.");

            using (var memoryStream = new MemoryStream())
            {
                // Creazione del workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Reservations");

                // Ottenere le proprietà della classe Reservation
                var properties = typeof(Reservation).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Creazione dell'intestazione
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < properties.Length; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(properties[i].Name);
                }

                // Aggiunta dei dati
                int rowIndex = 1;
                foreach (var reservation in reservations)
                {
                    IRow row = sheet.CreateRow(rowIndex++);
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = properties[i].GetValue(reservation, null);

                        // Gestione dei booleani e delle date
                        if (value is DateTime dateValue)
                        {
                            row.CreateCell(i).SetCellValue(dateValue.ToString("yyyy-MM-dd"));
                        }
                        else if (value is bool boolValue)
                        {
                            row.CreateCell(i).SetCellValue(boolValue ? "Yes" : "No");
                        }
                        else
                        {
                            row.CreateCell(i).SetCellValue(value?.ToString() ?? string.Empty);
                        }
                    }
                }

                // Scrittura nel MemoryStream
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
