using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using RequisitionPortal.BL.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Logic
{
    public class FileService: IFileService
    {
        public static ICellStyle getStyle(HSSFWorkbook workbook)
        {
            // Create font to use.
            NPOI.SS.UserModel.Font font = (NPOI.SS.UserModel.Font)workbook.CreateFont();
            font.Color = IndexedColors.White.Index;
            font.Boldweight = (short)50;
            font.FontName = "Univers 45 Light";
            // font.FontHeight = 60;
            //font.Color = IndexedColors.WHITE.Index;
            // Create cell style.
            CellStyle cellstyle = (NPOI.SS.UserModel.CellStyle)workbook.CreateCellStyle();
            cellstyle.FillBackgroundColor = IndexedColors.Brown.Index;
            cellstyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellstyle.FillForegroundColor = IndexedColors.Brown.Index;
            cellstyle.WrapText = true;
            cellstyle.SetFont(font);
            cellstyle.Alignment = HorizontalAlignment.Center;
            return cellstyle;
        }
    }
}
