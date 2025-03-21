using AttendanceBot.Abstractions;
using AttendanceBot.Models;

namespace AttendanceBot.Services
{
    public class LocationService : ILocationService
    {
        private const string FilePath = "D:\\AttendanceLogs\\Attendance.xlsx";

        public async Task LogLocationAsync(LocationData locationData)
        {
        //     EnsureFileExists();

        //     using var package = new ExcelPackage(new FileInfo(FilePath));
        //     var worksheet = package.Workbook.Worksheets[0];

        //     int newRow = worksheet.Dimension?.Rows + 1 ?? 2;
        //     worksheet.Cells[newRow, 1].Value = locationData.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        //     worksheet.Cells[newRow, 2].Value = locationData.UserId;
        //     worksheet.Cells[newRow, 3].Value = locationData.FirstName;
        //     worksheet.Cells[newRow, 4].Value = locationData.LastName;
        //     worksheet.Cells[newRow, 5].Value = locationData.Username;
        //     worksheet.Cells[newRow, 6].Value = locationData.Latitude;
        //     worksheet.Cells[newRow, 7].Value = locationData.Longitude;

        //     await package.SaveAsync();
        // }

        // private void EnsureFileExists()
        // {
        //     if (!File.Exists(FilePath))
        //     {
        //         using var package = new ExcelPackage();
        //         var worksheet = package.Workbook.Worksheets.Add("Attendance");

        //         worksheet.Cells[1, 1].Value = "Timestamp";
        //         worksheet.Cells[1, 2].Value = "User ID";
        //         worksheet.Cells[1, 3].Value = "First Name";
        //         worksheet.Cells[1, 4].Value = "Last Name";
        //         worksheet.Cells[1, 5].Value = "Username";
        //         worksheet.Cells[1, 6].Value = "Latitude";
        //         worksheet.Cells[1, 7].Value = "Longitude";

        //         package.SaveAs(new FileInfo(FilePath));
        //     }
        }
    }
}
