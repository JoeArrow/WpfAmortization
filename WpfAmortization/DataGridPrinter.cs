#region © 2020 JoeWare.
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System;
using System.Drawing;
using System.Windows;
using System.Collections;
using System.Drawing.Printing;
using System.Windows.Controls;

using Amortization;

namespace WpfAmortization
{
    // ----------------------------------------------------
    /// <summary>
    ///     DataGridPrinter Description
    /// </summary>

    public class DataGridPrinter
    {
        private Font _font;
        private DataGrid _dataGrid;
        private const int _cellMargin = 10;
        private PrintDocument _printDocument;

        private int _topMargin;
        private int _pageWidth;
        private int _pageHeight;
        private int _columnwidth;
        private int _bottomMargin;

        private float _location = 10.0f;
        private Color _foreColor = Color.Black;
        private Color _backColor = Color.AliceBlue;
        private Color _headerForeColor = Color.Black;
        private Color _altBackColor = Color.LightGray;
        private Color _headerBackColor = Color.LightSkyBlue;

        public int RowCount { set; get; } = 0;
        public int PageNumber { set; get; } = 1;

        // ------------------------------------------------

        public DataGridPrinter(DataGrid aGrid, PrintDocument aPrintDocument, Font font)
        {
            _font = font;
            _dataGrid = aGrid;
            _printDocument = aPrintDocument;

            _topMargin = _printDocument.DefaultPageSettings.Margins.Top;
            _bottomMargin = _printDocument.DefaultPageSettings.Margins.Bottom;
            _pageHeight = _printDocument.DefaultPageSettings.PaperSize.Height;

            _pageWidth = _printDocument.DefaultPageSettings.PaperSize.Width;
            _columnwidth = (_pageWidth / _dataGrid.Columns.Count) - (_dataGrid.Columns.Count * 2);
        }

        // ------------------------------------------------

        void DrawHorizontalLines(Graphics graphics, ArrayList lineBottoms)
        {
            var linePen = new Pen(_backColor, 1);

            foreach(float lineBottom in lineBottoms)
            {
                graphics.DrawLine(linePen, _location, lineBottom, _pageWidth, lineBottom);
            }
        }

        // ------------------------------------------------

        void DrawVerticalGridLines(Graphics graphics, Pen TheLinePen, int columnwidth, int bottom)
        {
            for(var currentColumn = 0; currentColumn < _dataGrid.Columns.Count; currentColumn++)
            {
                graphics.DrawLine(TheLinePen,
                                  _location + currentColumn * columnwidth,
                                  _location + _topMargin,
                                  _location + currentColumn * columnwidth,
                                  bottom);
            }
        }

        // ------------------------------------------------

        public bool DrawDataGrid(Graphics graphics)
        {
            bool bContinue;

            try
            {
                DrawHeader(graphics);
                bContinue = DrawRows(graphics);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                bContinue = false;
            }

            return bContinue;
        }

        // ------------------------------------------------

        public void DrawHeader(Graphics graphics)
        {
            var initialRowCount = RowCount;
            var cellFormat = new StringFormat();
            var linePen = new Pen(_backColor, 1);
            var foreBrush = new SolidBrush(_headerForeColor);
            var backBrush = new SolidBrush(_headerBackColor);

            cellFormat.Trimming = StringTrimming.EllipsisCharacter;
            cellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

            // ---------------------
            // draw the table header

            var startXPosition = _location;

            var headerBounds = new RectangleF(0, 0, 0, 0);
            var nextCellBounds = new RectangleF(0, 0, 0, 0);

            headerBounds.X = _location;
            headerBounds.Y = _location + _topMargin +
                             (RowCount - initialRowCount) * (_font.SizeInPoints + _cellMargin);

            headerBounds.Height = _font.SizeInPoints + _cellMargin;
            headerBounds.Width = _pageWidth;

            graphics.FillRectangle(backBrush, headerBounds);

            foreach(var property in new PaymentDetail().GetProperties())
            {
                var cellbounds = new RectangleF(startXPosition,
                                                _location + _topMargin +
                                                (RowCount - initialRowCount) * (_font.SizeInPoints + _cellMargin),
                                                _columnwidth,
                                                _font.SizeInPoints + _cellMargin);
                nextCellBounds = cellbounds;

                if(startXPosition + _columnwidth <= _pageWidth)
                {
                    graphics.DrawString(property.Name, _font, foreBrush, cellbounds, cellFormat);
                }

                startXPosition += _columnwidth;
            }

            graphics.DrawLine(linePen, _location, nextCellBounds.Bottom, _pageWidth, nextCellBounds.Bottom);
        }

        // ------------------------------------------------

        public bool DrawRows(Graphics graphics)
        {
            var lastRowBottom = _topMargin;
            var lineBottoms = new ArrayList();

            try
            {
                var theLinePen = new Pen(_backColor, 1);
                var foreBrush = new SolidBrush(_foreColor);
                var backBrush = new SolidBrush(_backColor);
                var alternatingBackBrush = new SolidBrush(_altBackColor);

                var cellFormat = new StringFormat()
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit,
                };

                var initialRowCount = RowCount;

                var rowBounds = new RectangleF(0, 0, 0, 0);

                // -------------------
                // draw vertical lines
                // draw the rows of the table

                for(int index = initialRowCount; index < _dataGrid.Items.Count; index++)
                {
                    var paymentDetail = _dataGrid.Items[index] as Amortization.PaymentDetail;

                    if(paymentDetail != null)
                    {
                        var startXPosition = _location;

                        rowBounds.X = _location;

                        rowBounds.Y = _location + _topMargin +
                                      ((RowCount - initialRowCount) + 1) * (_font.SizeInPoints + _cellMargin);

                        rowBounds.Width = _pageWidth;
                        rowBounds.Height = _font.SizeInPoints + _cellMargin;

                        lineBottoms.Add(rowBounds.Bottom);

                        graphics.FillRectangle((index % 2 == 0) ? alternatingBackBrush : backBrush, rowBounds);

                        foreach(var property in paymentDetail.GetProperties())
                        {
                            var cellBounds = new RectangleF(startXPosition,
                                                            _location + _topMargin +
                                                            ((RowCount - initialRowCount) + 1) * (_font.SizeInPoints + _cellMargin),
                                                            _columnwidth,
                                                            _font.SizeInPoints + _cellMargin);

                            if(startXPosition + _columnwidth <= _pageWidth)
                            {
                                graphics.DrawString(property.Value, _font, foreBrush, cellBounds, cellFormat);
                                lastRowBottom = (int)cellBounds.Bottom;
                            }

                            startXPosition += _columnwidth;
                        }

                        RowCount++;

                        if(RowCount * (_font.SizeInPoints + _cellMargin) > (_pageHeight * PageNumber) - (_bottomMargin + _topMargin))
                        {
                            DrawHorizontalLines(graphics, lineBottoms);
                            DrawVerticalGridLines(graphics, theLinePen, _columnwidth, lastRowBottom);
                            return true;
                        }
                    }
                }

                DrawHorizontalLines(graphics, lineBottoms);
                DrawVerticalGridLines(graphics, theLinePen, _columnwidth, lastRowBottom);

                return false; 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }
    }
}
